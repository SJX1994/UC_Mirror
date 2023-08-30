using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Spine.Unity;
using DG.Tweening;

public class Unit : MonoBehaviour
{
	
#region 数据对象
    
    [HideInInspector]
    public WeaponTemplate.WeaponType weaponType = WeaponTemplate.WeaponType.Null;
    protected StateMachineManager stateMachineManager;
    public UnitTemplate unitTemplate;   
    public UnitState state = UnitState.Idle;
    public bool flip = false;
    public UnitSoul unitSoul;
    [HideInInspector]
    public int id;
    [HideInInspector]
    public int level = 1;
    [HideInInspector]
    public int canHandleAttackCount = 0;
  
    [HideInInspector]
    public string skinName = "red";
    [HideInInspector]
    public Vector3 targetPos;
    [HideInInspector]
    public NavMeshAgent navMeshAgent;
    [HideInInspector]
    public  SkeletonRenderer skeletonRenderer;
    [HideInInspector]
    public SpriteRenderer selectionCircle;
    Color selectionCircleColor;
    [HideInInspector]
    public Weapon weapon = null;
    private Renderer spineRenderer;
    private MaterialPropertyBlock spinePropertyBlock;
    
    protected bool isReady = false;
    protected float lastGuardCheckTime, guardCheckInterval= 1f;
    private Unit[] hostiles;
    [HideInInspector]
    public Unit targetOfAttack;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public UnityAction OnAttacking; // 攻击事件
    public UnityAction OnAttackFinish; //攻击结束事件
    public UnityAction<Unit> OnDie; // 死亡事件
    public UnityAction<Unit> OnStartCollect; //收集开始
    public UnityAction<Unit> OnCollect; // 收集事件
    public UnityAction<Unit> OnArrive; // 到达事件
    public UnityAction<Unit> OnInitFinish; // 初始化完成事件

    public Vector3 blockPos;

    [HideInInspector]
    public float maxHealth,currentHP,shaderHP = 1f;
    protected float destoryTime = 8f;
    protected Effect effect;
    [HideInInspector]
    public bool controled = false;
    public enum UnitState
	{
		Idle, // 待机
		Guarding, // 侦察
		Attacking,// 进攻
		MovingToTarget,// 向目标移动
		MovingToSpotIdle,// 移动然后待机
		MovingToSpotGuard,// 移动然后侦察
		Dead,// 死亡
		MovingToSpotWithGuard,// 移动伴随侦察
	}
     public Vector3 rightAngle = new(0,0,0);
#endregion 数据对象
#region 数据关系
    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
	  animator = transform.Find("Spine").GetComponent<Animator>();
        skeletonRenderer = transform.Find("Spine").GetComponent<SkeletonRenderer>();
        selectionCircle = transform.Find("SelectionCircle").GetComponent<SpriteRenderer>();
        spineRenderer = transform.Find("Spine").GetComponent<Renderer>();
	  unitTemplate = Instantiate(unitTemplate); // 复制细胞模板 防止重写
	  
    }

    public virtual void Start()
    {
	  
        // 团结但是性格各异的 塞尔战士 所以走路速度略有不同 :)
		float rndmFactor = navMeshAgent.speed * .15f;
		navMeshAgent.speed += Random.Range(-rndmFactor, rndmFactor);
		animator.speed += Random.Range(-rndmFactor, rndmFactor);

	  canHandleAttackCount = 0;
		
	  if (TryGetComponent<Effect>(out Effect effect))
        {
            this.effect = effect;
		RunEffect(EffectTemplate.EffectType.Start);
        }
	  // 等级相关
	  unitTemplate.health *= level;
	  unitTemplate.attackPower *= level;
	  unitTemplate.attackSpeed *= level;
	  
	  if(unitTemplate.levelSprite)
	  {
		for(int i = 1; i < level+1; i++)
		{
			skeletonRenderer.transform.Find("Level_"+i).GetComponent<SpriteRenderer>().sprite = unitTemplate.levelSprite;
		}
		
	  }
	  // --- 
        maxHealth = unitTemplate.health;
        currentHP = maxHealth;
	  targetPos = Vector3.zero;
	  
        UpdateMatHealth(1);
        SetSelected(false);
        Guard();
	  
	  InvokeRepeating(nameof(BackToPosition), 0.5f,unitTemplate.chaseDuration);
	  
	  if(GameObject.Find("CanvasManager_StayMachine(Clone)"))
            {
                  stateMachineManager = GameObject.Find("CanvasManager_StayMachine(Clone)").GetComponent<StateMachineManager>();
            }
        if(GameObject.Find("CanvasManager_StayMachine"))
            {
                  stateMachineManager = GameObject.Find("CanvasManager_StayMachine").GetComponent<StateMachineManager>();
            }
	
	rightAngle = new Vector3(80f, 0f, 0f);
	// 武器	
	skeletonRenderer.transform.TryGetComponent(out weapon);
	
	Invoke(nameof(UnitSetSkin), 0.1f);
	  	
    }
    void OnDisable()
    {
	if (spinePropertyBlock == null)
      {
          spinePropertyBlock = new MaterialPropertyBlock();
      }
	spineRenderer .GetPropertyBlock(spinePropertyBlock);
      spinePropertyBlock.SetFloat("_Porcess", 1f);
      spineRenderer.SetPropertyBlock(spinePropertyBlock);
    }
    public virtual void Update()
    {
	 // 面向摄像机
	  animator.transform.rotation = Quaternion.Euler(rightAngle);
	  transform.rotation = Quaternion.Euler(Vector3.zero);

	  if(controled)
	  {
		return;
	  }
	  if(!navMeshAgent || !animator)
	  {
		return;
	  }
	  
	 
	  SetFlip(flip?navMeshAgent.velocity.x:-navMeshAgent.velocity.x);
	  
       
        // 给 NavMesh 代理时间设置其目的地的小技巧。
        if(!isReady)
	  {
	  	isReady = true;
	  	return;
	  }
	  
        switch(state)
        {
            case UnitState.MovingToSpotIdle:
                if(navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance + .1f)
                {
                    Stop();
                }
                break;
            case UnitState.MovingToSpotGuard:
                if(navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance + .1f)
			{
					
					Guard();
					ArrivedDisPlay();
			}
                break;
            case UnitState.MovingToTarget:
		    
                if(IsDeadOrNull(targetOfAttack))
				{
						
					Guard();
				}
                else
				{
					//检查是否在 主动攻击范围 内
					if(navMeshAgent.remainingDistance < unitTemplate.engageDistance)
					{
                        		//Debug.LogWarning(transform.name + navMeshAgent.remainingDistance + "进入攻击范围" + unitTemplate.engageDistance);
						navMeshAgent.velocity = Vector3.zero;
						StartAttacking();
					}
					else
					{
						navMeshAgent.SetDestination(targetOfAttack.transform.position); 
					}
				}
                break;
            case UnitState.Guarding:
                if(Time.time > lastGuardCheckTime + guardCheckInterval)
				{
					lastGuardCheckTime = Time.time;
					Unit t = GetNearestHostileUnit();
					
					if(t != null)
					{
						MoveToAttack(t);
					}
					else if( (targetPos!=Vector3.zero && t == null) || (targetPos!=Vector3.zero && targetOfAttack == null))
					{
						AICommand stop = new(AICommand.CommandType.Stop,targetPos);
        					this.ExecuteCommand(stop);
						// 塞尔回到之前位置
						if(unitTemplate.unitType == UnitTemplate.UnitType.Cell )
						{
							AICommand newCommand = new(AICommand.CommandType.GoToAndGuard,targetPos);
        						this.ExecuteCommand(newCommand);
						}
						
						// 外尔回到之前位置	
						if(unitTemplate.unitType == UnitTemplate.UnitType.Virus )
						{
							AICommand newCommand = new(AICommand.CommandType.GoToAndGuard,targetPos);
        						this.ExecuteCommand(newCommand);
						}
					}
				}
                break;
            case UnitState.Attacking:

				if(IsDeadOrNull(targetOfAttack))
				{
					Guard();
				}
				else
				{
                    // 看向目标
					// Vector3 desiredForward = (targetOfAttack.transform.position - transform.position).normalized;
					// transform.forward = Vector3.Lerp(transform.forward, desiredForward, Time.deltaTime * 10f);
				}
                break;
		case UnitState.MovingToSpotWithGuard:
			
			Unit nearestEnemy = GetNearestHostileUnit();
			if(nearestEnemy != null)
			{
				MoveToAttack(nearestEnemy);
			}
			if( targetPos.x + 0.1f < transform.position.x || targetPos.x - 0.1f > transform.position.x)
			{
				ArrivedDisPlay();
			}
			
		    break;
          
            
        }
       
	  float navMeshAgentSpeed = navMeshAgent.velocity.magnitude;
	  animator.SetFloat("Speed", navMeshAgentSpeed * .05f);
	  
        
		
		if(animator.GetFloat("Speed") > 0.1f)
		{
			SetSelected(true);
		}else
		{
			SetSelected(false);
		}
	  
        
    }
    /// <summary>
    /// 单位AI接受指令
    /// </summary>
    public void ExecuteCommand(AICommand c)
	{
        if(state == UnitState.Dead)
		{
			// 已经死了 无法接受命令
			return;
		}
        switch(c.commandType)
		{
			case AICommand.CommandType.GoToAndIdle:
				RunEffect(EffectTemplate.EffectType.Collect);
				GoToAndIdle(c.destination);
				break;

			case AICommand.CommandType.GoToAndGuard:
				GoToAndGuard(c.destination);
				break;

			case AICommand.CommandType.Stop:
				Stop();
				break;

			case AICommand.CommandType.AttackTarget:
				MoveToAttack(c.destination);
				break;
			
			case AICommand.CommandType.Die:
				Die(unitTemplate.destoryTime);
				break;
			case AICommand.CommandType.Collecting:
				Collecting();
				break;
		
		}
    }
    void OnDrawGizmos()
    {
	Gizmos.color = new Vector4(Color.red.r,Color.red.g,Color.red.b,0.2f) ;
	Gizmos.DrawSphere(this.transform.position, unitTemplate.engageDistance);
	Gizmos.color = new Vector4(Color.green.r,Color.green.g,Color.green.b,0.2f) ;
	Gizmos.DrawSphere(this.transform.position, unitTemplate.guardDistance);
    }
#endregion 数据关系
#region 数据操作
    
    void UnitSetSkin()
    {
	skinName = skeletonRenderer.Skeleton.Skin.Name;
	switch(skinName)
	{
		case "red":
			selectionCircleColor = Color.red;
		break;
		case "blue":
			selectionCircleColor = Color.blue;
		break;
		case "green":
			selectionCircleColor = Color.green;
		break;
		case "purple":
			selectionCircleColor = Color.magenta;
		break;
	}
	SetFlip(flip?1:-1);
	if(weapon)
	{
		weaponType = weapon.weaponTemplate.weaponType;
	}
      OnInitFinish?.Invoke(this);

    }
    void UpdateMatHealth(float SetFloat)
    {
        if (spinePropertyBlock == null)
        {
            spinePropertyBlock = new MaterialPropertyBlock();
        }
        spineRenderer .GetPropertyBlock(spinePropertyBlock);
        spinePropertyBlock.SetFloat("_Porcess", SetFloat);
        spineRenderer.SetPropertyBlock(spinePropertyBlock);
    }
    /// <summary>
    /// 数字范围映射
    /// </summary>
    static float Remap(float input, float oldLow, float oldHigh, float newLow, float newHigh) {
        float t = Mathf.InverseLerp(oldLow, oldHigh, input);
        return Mathf.Lerp(newLow, newHigh, t);
    }

    /// <summary>
    /// spine 左右翻转 
    /// </summary>
    /// <param name="horizontal"></param>
    protected void SetFlip (float horizontal) {
			if (horizontal != 0) {
				skeletonRenderer.Skeleton.ScaleX = horizontal > 0 ? -1f : 1f;

			}
	}
    /// <summary>
    /// 移动到一个位置 并侦察
    /// </summary>
	private void GoToAndGuard(Vector3 location)
	{
		if(!navMeshAgent)
		{
			return;
		}
		state = UnitState.MovingToSpotGuard;
		targetOfAttack = null;
		isReady = false;

		navMeshAgent.isStopped = false;
		navMeshAgent.SetDestination(location);
	}
    /// <summary>
    /// 移动到一个位置 并待机
    /// </summary>
	private void GoToAndIdle(Vector3 location)
	{
		
		state = UnitState.MovingToSpotIdle;
		targetOfAttack = null;
		isReady = false;

		navMeshAgent.isStopped = false;
		navMeshAgent.SetDestination(location);
	}
    /// <summary>
    /// 获取最近的敌对单位
    /// </summary>
    /// <returns>最近的敌方Unit</returns>
    	protected Unit GetNearestHostileUnit()
	{
		
		hostiles = GameObject.FindGameObjectsWithTag(unitTemplate.GetOtherUnitType().ToString()).Select(x => x.GetComponent<Unit>()).ToArray();
		
		Unit nearestEnemy = null;
		float nearestEnemyDistance = 1000f;
		for(int i=0; i<hostiles.Count(); i++)
		{
			if(IsDeadOrNull(hostiles[i]))
			{
				continue;
			}

			float distanceFromHostile = Vector3.Distance(hostiles[i].transform.position, transform.position);
			if(distanceFromHostile <= unitTemplate.guardDistance)
			{
				if(distanceFromHostile < nearestEnemyDistance)
				{
					nearestEnemy = hostiles[i];
					nearestEnemyDistance = distanceFromHostile;
				}
			}
		}

		return nearestEnemy;
	}
    /// <summary>
    /// 在移动的过程中侦察
    /// </summary>
    /// <param name="targetLocation"></param>
	protected void MoveToAttack(Vector3 targetLocation)
	{
		state = UnitState.MovingToSpotWithGuard;
		targetPos = targetLocation;
		navMeshAgent.isStopped = false;
		navMeshAgent.SetDestination(targetLocation);
		
	}
    /// <summary>
    /// 单位到达时的表现
    /// </summary>
    void ArrivedDisPlay()
    {
			Guard();
			blockPos = transform.position;
			// 检测位置变化
			OnArrive?.Invoke(this);

    }

    /// <summary>
    /// 向目标移动以攻击它
    /// </summar>
	protected void MoveToAttack(Unit target)
	{
		if(!navMeshAgent)
		{
			return;
		}
		if(!IsDeadOrNull(target))
		{
			state = UnitState.MovingToTarget;
			targetOfAttack = target;
			isReady = false;

			navMeshAgent.isStopped = false;
			navMeshAgent.SetDestination(target.transform.position);
		}
		else
		{
			Guard();
		}
	}
	protected virtual void Collecting()
	{
		OnCollect?.Invoke(this);
		state = UnitState.Dead; // 使其不再参与任何状态机逻辑
		SetSelected(false);
		// 避免对象参与任何 Raycast 或标签搜索
		gameObject.tag = "Untagged";
		gameObject.layer = 0;
		float destoryTime= 0.5f;
		// 删除不需要的组件
		if(unitTemplate.levelSprite)
		{
			for(int i = 1; i < level+1; i++)
			{
				Destroy(skeletonRenderer.transform.Find("Level_"+i).gameObject);
			}
			
		}
		effect.RemoveAllEffect(destoryTime-1f);
		Destroy(selectionCircle,destoryTime);
		Destroy(navMeshAgent,destoryTime);
		Destroy(GetComponent<Collider>()); // 将使其在点击时无法选择
        	Destroy(animator.transform.GetComponent<SkeletonMecanim>(), destoryTime);// 给它一些时间来完成动画
		Destroy(animator, destoryTime); // 给它一些时间来完成动画
		Destroy(this, destoryTime);
		Destroy(this.transform.gameObject, destoryTime+2f);
	}
	/// <summary>
	/// 加载收集特效
	/// </summary>
	/// <returns></returns>
	protected void RunEffect(EffectTemplate.EffectType type)
	{
		if(!effect)return;
		
		effect.effectType = type;

		effect.AddEffect();
	}

    /// <summary>
    /// 单位死亡
    /// </summary>
    public virtual void Die(float destoryTime)
	{
		// if(level>1)
		// {
		// 	CreatSoul();
		// }	
		CreatSoul();
		state = UnitState.Dead; // 使其不再参与任何状态机逻辑
		animator.SetTrigger("DoDeath");

		SetSelected(false);

		// 触发一个事件，以便通知 订阅 该单位的任何 侦听器
		OnDie?.Invoke(this);
		if(transform.TryGetComponent(out Soldier s))
		{
			stateMachineManager.OnUnitDied(s);
		};
		
		// 避免对象参与任何 Raycast 或标签搜索
		
		gameObject.tag = "Untagged";
		gameObject.layer = 0;
		// 删除不需要的组件
		if(effect)effect.RemoveAllEffect(destoryTime-1f);

		
		Destroy(selectionCircle);
		Destroy(navMeshAgent);
		Destroy(GetComponent<Collider>()); // 将使其在点击时无法选择
        	Destroy(animator.transform.GetComponent<SkeletonMecanim>(), destoryTime);// 给它一些时间来完成动画
		Destroy(animator, destoryTime); // 给它一些时间来完成动画
		StartCoroutine(DieDispaly(animator.transform.localScale.y,0.1f,destoryTime-1f));
		Destroy(this, destoryTime);
		Destroy(this.transform.gameObject, destoryTime+2f);
        
	}
	protected virtual IEnumerator DieDispaly(float v_start, float v_end, float duration )
	{
		float elapsed = 0.0f;
		float value;
		while (elapsed < duration )
		{
			value = Mathf.Lerp( v_start, v_end, elapsed / duration );
			elapsed += Time.deltaTime;
			animator.transform.localScale =new Vector3(animator.transform.localScale.x,value,animator.transform.localScale.z);
			yield return null;
		}
    }
    /// <summary>
    /// 生成鬼魂
    /// </summary>
    public virtual void CreatSoul()
	{
		if(unitSoul==null)return;

		if(unitTemplate.unitType == UnitTemplate.UnitType.Virus)
		{
			stateMachineManager.exTextUI.text = (int.Parse(stateMachineManager.exTextUI.text)+unitSoul.unitSoulTemplate.value).ToString();
		}
		
		for(int i = 0; i <level; i++)
		{
			UnitSoul soul = Instantiate(unitSoul, transform.position, Quaternion.identity);
			if(unitTemplate.unitType == UnitTemplate.UnitType.Cell)
			{
				skinName = skeletonRenderer.Skeleton.Skin.Name;
				if(skinName == "")
				{
					skinName = "red";
				}
				soul.skeletonRenderer.Skeleton.SetSkin(skinName);
			}
		}
		
		
		
	}
    /// <summary>
    /// 受到攻击
    /// </summary>
	public void SufferAttack(int damage)
	{
		if(state == UnitState.Dead)
		{
			// 已经死了
			return;
		}
	  // 濒死
        if( damage*2 >= unitTemplate.health || unitTemplate.health/maxHealth == 0.2f )
	  {
	  	if(transform.TryGetComponent(out Soldier s))
	  	{
	  		stateMachineManager.OnUnitDying(s,damage);
	  	};
	  }
        
        unitTemplate.health -= damage;
        // 血条 着色器
        currentHP = unitTemplate.health;
        shaderHP = Remap(currentHP,0,maxHealth,0,1);
        UpdateMatHealth(shaderHP);
        
		
		if(unitTemplate.health <= 0)
		{
			unitTemplate.health = 0;
			RunEffect(EffectTemplate.EffectType.End);	
			Die(unitTemplate.destoryTime);
		}
	}
    
    /// <summary>
    /// 在主动攻击范围内，开始攻击
    /// </summary>
	protected virtual void StartAttacking()
	{
		if(!navMeshAgent)
		{
			return;
		}
		if(!IsDeadOrNull(targetOfAttack))
		{
			state = UnitState.Attacking;
			isReady = false;
			navMeshAgent.isStopped = true;
			
			StartCoroutine(DealAttack());
		}
		else
		{
			Guard();
		}
	}
    /// <summary>
    /// 单次击打动作
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator DealAttack()
	{
        
		while(targetOfAttack != null)
		{

			RunEffect(EffectTemplate.EffectType.Attacking);
			BossAttack();
			animator.SetTrigger("DoAttack");
			if(OnAttacking != null)
			{
				OnAttacking();
			}
			WeaponDisplay();
			targetOfAttack.SufferAttack(unitTemplate.attackPower);
			
			yield return new WaitForSeconds(1f / unitTemplate.attackSpeed);

			if(IsDeadOrNull(targetOfAttack))
			{
				animator.SetTrigger("InterruptAttack");
				OnAttackFinish?.Invoke();
				break;

			}

			if(state == UnitState.Dead)
			{
				yield break;
			}

			if(Vector3.Distance(targetOfAttack.transform.position, transform.position) > unitTemplate.engageDistance)
			{
				MoveToAttack(targetOfAttack);
			}
		}

		if(state == UnitState.Attacking)
		{
			Guard();
		}
	}
	protected void WeaponDisplay()
	{
		if(weaponType == WeaponTemplate.WeaponType.Null)return;
		switch(weaponType)
		{
			case WeaponTemplate.WeaponType.Sword:
				
				break;
			case WeaponTemplate.WeaponType.Bow:
				weapon.Shoot(targetOfAttack.transform.position);
				break;
			case WeaponTemplate.WeaponType.Spear:
				
				break;
			case WeaponTemplate.WeaponType.Shield:
				
				break;
		}
	}
	protected virtual void BossAttack()
	{

	}
    /// <summary>
    /// 检查目标是否被其他人杀死
    /// </summary>
    public bool IsDeadOrNull(Unit u)
	{
		return (u == null || u.state == UnitState.Dead);
	}
    /// <summary>
    /// 停止并进入待机状态
    /// </summary>
	private void Stop()
	{
		state = UnitState.Idle;
		targetOfAttack = null;
		isReady = false;
		navMeshAgent.isStopped = true;
		navMeshAgent.velocity = Vector3.zero;
	}
    /// <summary>
    /// 是否被 梅尔 指挥
    /// </summary>
    protected virtual void SetSelected(bool selected)
	{
		// 根据 是否被梅尔指挥 设置透明度
		if(selectionCircle)
		{
			Color newColor = selectionCircleColor;
			newColor.a = (selected) ? 1f : .6f;
			selectionCircle.color = newColor;
		}
		
	}
    /// <summary>
    /// 停下来注意附近的敌人
    /// </summary>
    protected void Guard()
	{
		if(!navMeshAgent)
		{
			return;
		}
        // 返回 梅尔指挥 的指定区域
	  	RunEffect(EffectTemplate.EffectType.Active);
		state = UnitState.Guarding;
		targetOfAttack = null;
		isReady = false;

		navMeshAgent.isStopped = true;
		navMeshAgent.velocity = Vector3.zero;
		
	  
	}
	/// <summary>
	/// 追踪几秒
	/// </summary>
	/// <param name="pos"></param>
	/// <returns></returns>
	void BackToPosition()
	{
		targetOfAttack = null;
	}
#endregion 数据操作
}
