using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Spine.Unity;
using DG.Tweening;
using Mirror;
using UC_PlayerData;
public class Unit : NetworkBehaviour
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
    public Vector2 idV2;
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
    public SkeletonRenderer skeletonRenderer;
    public SkeletonRenderer SkeletonRenderer
    {
        get
        {
            if(!skeletonRenderer)skeletonRenderer = transform.Find("Spine").GetComponent<SkeletonRenderer>();
            return skeletonRenderer;
        }
    }
    [HideInInspector]
    public SpriteRenderer selectionCircle;
    Color selectionCircleColor;
    [HideInInspector]
    public Weapon weapon = null;
    public Weapon Weapon
    {
        get
        {
            if(!weapon)weapon = transform.Find("Spine").GetComponent<Weapon>();
            return weapon;
        }
    }
    SpriteRenderer speechless;
    public SpriteRenderer Speechless
    {
        get
        {
            if(!speechless)speechless = transform.Find("Spine").Find("Speechless").GetComponent<SpriteRenderer>();
            return speechless;
        }
    }
    private Renderer spineRenderer;
    private MaterialPropertyBlock spinePropertyBlock_beenAttacked;
    private MaterialPropertyBlock spinePropertyBlock_hp;
    private MaterialPropertyBlock spinePropertyBlock_foreshadow;
    private MaterialPropertyBlock spinePropertyBlock_alpha;
    private MaterialPropertyBlock spinePropertyBlock_SelectEffect;
    private MaterialPropertyBlock spinePropertyBlock_color;
    public bool isReady = false;
    public float lastGuardCheckTime, guardCheckInterval = 1f;
    private Unit[] hostiles;
    [HideInInspector]
    public Unit targetOfAttack;
    [HideInInspector]
    public TetriAttackable_Attribute targetOfAttackableProp;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public UnityAction OnAttacking; // 攻击事件
    public UnityAction<int> OnBeenAttacked; // 被攻击
    public UnityAction OnAttackFinish; //攻击结束事件
    public UnityAction<Unit> OnDie; // 死亡事件
    public UnityAction<Unit> OnStartCollect; //收集开始
    public UnityAction<Unit> OnCollect; // 收集事件
    public UnityAction<Unit> OnArrive; // 到达事件
    public UnityAction<Unit> OnInitFinish; // 初始化完成事件

    public Vector3 blockPos;

    [HideInInspector]
    public float maxHealth, currentHP, shaderHP = 1f;
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
        AttackProp,
        AttackingProp,
        Dead,// 死亡
        MovingToSpotWithGuard,// 移动伴随侦察
    }
    public Vector3 rightAngle = new(0, 0, 0);
    #endregion 数据对象
    #region 数据关系
    public virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = transform.Find("Spine").GetComponent<Animator>();
        selectionCircle = transform.Find("SelectionCircle").GetComponent<SpriteRenderer>();
        spineRenderer = transform.Find("Spine").GetComponent<Renderer>();
        unitTemplate = Instantiate(unitTemplate); // 复制细胞模板 防止重写
    }

    public virtual void Start()
    {
        canHandleAttackCount = 0;
        maxHealth = unitTemplate.health;
        currentHP = maxHealth;
        targetPos = Vector3.zero;
        SetSelected(false);
        Guard();
        InvokeRepeating(nameof(BackToPosition), 0.5f, unitTemplate.chaseDuration);
		stateMachineManager = FindObjectOfType<StateMachineManager>();
		if(!stateMachineManager)return;
        rightAngle = new Vector3(80f, 0f, 0f);
        // 武器	
        SkeletonRenderer.transform.TryGetComponent(out weapon);
        Invoke(nameof(UnitSetSkin), 0.1f);
		if (!TryGetComponent<Effect>(out Effect effect))return;
        this.effect = effect;
        RunEffect(EffectTemplate.EffectType.Start);
		// 等级相关
        unitTemplate.health *= level;
        unitTemplate.attackPower *= level;
        unitTemplate.attackSpeed *= level;
        // if (!unitTemplate.levelSprite)return;
        // for (int i = 1; i < level + 1; i++)
        // {
        //     skeletonRenderer.transform.Find("Level_" + i).GetComponent<SpriteRenderer>().sprite = unitTemplate.levelSprite;
        // }
        if(!navMeshAgent)return;
        if(!navMeshAgent.enabled)return;
        // 团结但是性格各异的 塞尔战士 所以走路速度略有不同 :)
        float rndmFactor = navMeshAgent.speed * .15f;
        navMeshAgent.speed += Random.Range(-rndmFactor, rndmFactor);
        // animator.speed += Random.Range(-rndmFactor, rndmFactor);
    }
    void OnDisable()
    {
        if (spinePropertyBlock_hp == null)spinePropertyBlock_hp = new MaterialPropertyBlock();
        spineRenderer.GetPropertyBlock(spinePropertyBlock_hp);
        spinePropertyBlock_hp.SetFloat("_Porcess", 1f);
        spineRenderer.SetPropertyBlock(spinePropertyBlock_hp);
    }
    public virtual void Update()
    {
        // 面向摄像机
        animator.transform.rotation = Quaternion.Euler(rightAngle);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        if (controled)return;
        if (!navMeshAgent || !animator)return;
        SetFlip(flip ? navMeshAgent.velocity.x : -navMeshAgent.velocity.x);
        // 给 NavMesh 代理时间设置其目的地的小技巧。
        if (!isReady){isReady = true;return;}
        switch (state)
        {
            case UnitState.MovingToSpotIdle:
                if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance + .1f)
                {
                    Stop();
                }
                break;
            case UnitState.MovingToSpotGuard:
                if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance + .1f)
                {
                    Guard();
                    ArrivedDisPlay();
                }
                break;
            case UnitState.MovingToTarget:
                if (IsDeadOrNull(targetOfAttack)){Guard();}
                else if (navMeshAgent.remainingDistance < unitTemplate.engageDistance)
                {
                    navMeshAgent.velocity = Vector3.zero;
                    StartAttacking();
                }
                else
                {
                    navMeshAgent.SetDestination(targetOfAttack.transform.position);
                }
                
                break;
            case UnitState.Guarding:
                if (Time.time > lastGuardCheckTime + guardCheckInterval)
                {
                    lastGuardCheckTime = Time.time;
                    Unit t = GetNearestHostileUnit();

                    if (t != null)
                    {
                        MoveToAttack(t);
                    }
                    else if ((targetPos != Vector3.zero && t == null) || (targetPos != Vector3.zero && targetOfAttack == null))
                    {
                        AICommand stop = new(AICommand.CommandType.Stop, targetPos);
                        this.ExecuteCommand(stop);
                        // 塞尔回到之前位置
                        if (unitTemplate.unitType == UnitTemplate.UnitType.Cell)
                        {
                            AICommand newCommand = new(AICommand.CommandType.GoToAndGuard, targetPos);
                            this.ExecuteCommand(newCommand);
                        }

                        // 外尔回到之前位置	
                        if (unitTemplate.unitType == UnitTemplate.UnitType.Virus)
                        {
                            AICommand newCommand = new(AICommand.CommandType.GoToAndGuard, targetPos);
                            this.ExecuteCommand(newCommand);
                        }
                    }
                }
                break;
            case UnitState.Attacking:
                if(IsDeadOrNull(targetOfAttack)){Guard();return;}
                break;
            case UnitState.MovingToSpotWithGuard:

                Unit nearestEnemy = GetNearestHostileUnit();
                if (nearestEnemy != null)
                {
                    MoveToAttack(nearestEnemy);
                }
                if (targetPos.x + 0.1f < transform.position.x || targetPos.x - 0.1f > transform.position.x)
                {
                    ArrivedDisPlay();
                }

            break;
        }

        float navMeshAgentSpeed = navMeshAgent.velocity.magnitude;
        animator.SetFloat("Speed", navMeshAgentSpeed * .05f);
        if (animator.GetFloat("Speed") > 0.1f)
        {
            SetSelected(true);
        }
        else
        {
            SetSelected(false);
        }


    }
    /// <summary>
    /// 单位AI接受指令
    /// </summary>
    public void ExecuteCommand(AICommand c)
    {
        if (state == UnitState.Dead)
        {
            // 已经死了 无法接受命令
            return;
        }
        switch (c.commandType)
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
        Gizmos.color = new Vector4(Color.red.r, Color.red.g, Color.red.b, 0.2f);
        Gizmos.DrawSphere(this.transform.position, unitTemplate.engageDistance);
        Gizmos.color = new Vector4(Color.green.r, Color.green.g, Color.green.b, 0.2f);
        Gizmos.DrawSphere(this.transform.position, unitTemplate.guardDistance);
    }
    #endregion 数据关系
    #region 数据操作

    public void UnitSetSkin()
    {
        if(!SkeletonRenderer){Awake();Start();}
        skinName = SkeletonRenderer.Skeleton.Skin.Name;
        switch (skinName)
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
        // SetFlip(flip ? 1 : -1);
        if (weapon)
        {
            weaponType = weapon.weaponTemplate.weaponType;
        }
        OnInitFinish?.Invoke(this);

    }
    public void UpdateMatSelectEffect(Color SetColor)
    {
        if (spinePropertyBlock_SelectEffect == null)
        {
            spinePropertyBlock_SelectEffect = new MaterialPropertyBlock();
        }
        if(SetColor == spinePropertyBlock_SelectEffect.GetColor("_SelectOutlineColor"))return;
        spineRenderer.GetPropertyBlock(spinePropertyBlock_SelectEffect);
        spinePropertyBlock_SelectEffect.SetColor("_SelectOutlineColor", SetColor);
        spineRenderer.SetPropertyBlock(spinePropertyBlock_SelectEffect);
    }
    public void UpdateBeenAttackedEffect(Color SetColor)
    {
        if (spinePropertyBlock_beenAttacked == null)
        {
            spinePropertyBlock_beenAttacked = new MaterialPropertyBlock();
        }
        if(SetColor == spinePropertyBlock_beenAttacked.GetColor("_BeenAttackedColor"))return;
        spineRenderer.GetPropertyBlock(spinePropertyBlock_beenAttacked);
        spinePropertyBlock_beenAttacked.SetColor("_BeenAttackedColor", SetColor);
        spineRenderer.SetPropertyBlock(spinePropertyBlock_beenAttacked);
    }
    public void UpdateMatHealth(float SetFloat)
    {
        if (spinePropertyBlock_hp == null)
        {
            spinePropertyBlock_hp = new MaterialPropertyBlock();
        }
        if(SetFloat == spinePropertyBlock_hp.GetFloat("_Porcess"))return;
        spineRenderer.GetPropertyBlock(spinePropertyBlock_hp);
        spinePropertyBlock_hp.SetFloat("_Porcess", SetFloat);
        spineRenderer.SetPropertyBlock(spinePropertyBlock_hp);
    }
    public void UpdateColorMultiplication(Color SetColor)
    {
        if  (spinePropertyBlock_color == null)
        {
            spinePropertyBlock_color = new MaterialPropertyBlock();
        }
        if(SetColor == spinePropertyBlock_color.GetColor("_Color"))return;
        spineRenderer.GetPropertyBlock(spinePropertyBlock_color);
        spinePropertyBlock_color.SetColor("_Color", SetColor);
        spineRenderer.SetPropertyBlock(spinePropertyBlock_color);
    }
    public void UpdateMatForeshadow(float SetFloat)
    {
        if (spinePropertyBlock_foreshadow == null)
        {
            spinePropertyBlock_foreshadow = new MaterialPropertyBlock();
        }
        if(SetFloat == spinePropertyBlock_foreshadow.GetFloat("_Foreshadow"))return;
        spineRenderer.GetPropertyBlock(spinePropertyBlock_foreshadow);
        spinePropertyBlock_foreshadow.SetFloat("_Foreshadow", SetFloat);
        spineRenderer.SetPropertyBlock(spinePropertyBlock_foreshadow);
    }
    public void UpdateMatAlpha(float SetFloat)
    {
        if (spinePropertyBlock_alpha == null)
        {
            spinePropertyBlock_alpha = new MaterialPropertyBlock();
        }
        if(SetFloat == spinePropertyBlock_alpha.GetFloat("_Alpha"))return;
        spineRenderer.GetPropertyBlock(spinePropertyBlock_alpha);
        spinePropertyBlock_alpha.SetFloat("_Alpha", SetFloat);
        spineRenderer.SetPropertyBlock(spinePropertyBlock_alpha);
    }
    /// <summary>
    /// 数字范围映射
    /// </summary>
    static float Remap(float input, float oldLow, float oldHigh, float newLow, float newHigh)
    {
        float t = Mathf.InverseLerp(oldLow, oldHigh, input);
        return Mathf.Lerp(newLow, newHigh, t);
    }

    /// <summary>
    /// spine 左右翻转 
    /// </summary>
    /// <param name="horizontal"></param>
    protected void SetFlip(float horizontal)
    {
        if (horizontal != 0)
        {
            SkeletonRenderer.Skeleton.ScaleX = horizontal > 0 ? -1f : 1f;

        }
    }
    /// <summary>
    /// 移动到一个位置 并侦察
    /// </summary>
	private void GoToAndGuard(Vector3 location)
    {
        if (!navMeshAgent)
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
        for (int i = 0; i < hostiles.Count(); i++)
        {
            if (IsDeadOrNull(hostiles[i]))continue;
            float distanceFromHostile = Vector3.Distance(hostiles[i].transform.position, transform.position);
            if (distanceFromHostile <= unitTemplate.guardDistance)
            {
                if (distanceFromHostile < nearestEnemyDistance)
                {
                    nearestEnemy = hostiles[i];
                    nearestEnemyDistance = distanceFromHostile;
                }
            }
        }

        return nearestEnemy;
    }
    protected Unit GetNearestPVPHostileUnit()
    {
        hostiles = GameObject.FindGameObjectsWithTag(unitTemplate.GetOtherPlayerType().ToString()).Select(x => x.GetComponent<Unit>()).ToArray();
        Unit nearestEnemy = null;
        float nearestEnemyDistance = 1000f;
        for (int i = 0; i < hostiles.Count(); i++)
        {
            if (IsDeadOrNull(hostiles[i]))continue;
            float distanceFromHostile = Vector3.Distance(hostiles[i].transform.position, transform.position);
            if (distanceFromHostile <= unitTemplate.guardDistance)
            {
                if (distanceFromHostile < nearestEnemyDistance)
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
        if (!navMeshAgent)return;
        if (!IsDeadOrNull(target))
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
        float destoryTime = 0.5f;
        // 删除不需要的组件
        if (unitTemplate.levelSprite)
        {
            for (int i = 1; i < level + 1; i++)
            {
                Destroy(SkeletonRenderer.transform.Find("Level_" + i).gameObject);
            }

        }
        effect.RemoveAllEffect(destoryTime - 1f);
        Destroy(selectionCircle, destoryTime);
        Destroy(navMeshAgent, destoryTime);
        Destroy(GetComponent<Collider>()); // 将使其在点击时无法选择
        Destroy(animator.transform.GetComponent<SkeletonMecanim>(), destoryTime);// 给它一些时间来完成动画
        Destroy(animator, destoryTime); // 给它一些时间来完成动画
        Destroy(this, destoryTime);
        Destroy(this.transform.gameObject, destoryTime + 2f);
    }
    /// <summary>
    /// 加载收集特效
    /// </summary>
    /// <returns></returns>
    public void RunEffect(EffectTemplate.EffectType type)
    {
        if (!effect) return;

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
        if (transform.TryGetComponent(out SoldierBehaviors s))
        {
            stateMachineManager.OnUnitDied(s);
        };

        // 避免对象参与任何 Raycast 或标签搜索

        gameObject.tag = "Untagged";
        gameObject.layer = 0;
        // 删除不需要的组件
        if (effect) effect.RemoveAllEffect(destoryTime - 1f);


        Destroy(selectionCircle);
        Destroy(navMeshAgent);
        Destroy(GetComponent<Collider>()); // 将使其在点击时无法选择
        Destroy(animator.transform.GetComponent<SkeletonMecanim>(), destoryTime);// 给它一些时间来完成动画
        Destroy(animator, destoryTime); // 给它一些时间来完成动画
        StartCoroutine(DieDispaly(animator.transform.localScale.y, 0.1f, destoryTime - 1f));
        Destroy(this, destoryTime);
        Destroy(this.transform.gameObject, destoryTime + 2f);

    }
    public virtual void DieSimple(float destoryTime)
    {
        if(Local())
        {
            Sound_Died();
        }else
        {
            if(isServer)Server_Sound_Died();
        }
        animator.speed = 3f;
        state = UnitState.Dead;
        animator.SetTrigger("DoDeath");
        OnDie?.Invoke(this);
        // if (transform.TryGetComponent(out Soldier s))
        // {
        //     stateMachineManager.OnUnitDied(s);
        // };
        gameObject.tag = "Untagged";
        gameObject.layer = 0;
        if (effect) effect.RemoveAllEffect(destoryTime - 1f);
        Destroy(selectionCircle);
        Destroy(navMeshAgent);
        Destroy(GetComponent<Collider>()); // 将使其在点击时无法选择
        Destroy(animator.transform.GetComponent<SkeletonMecanim>(), destoryTime);// 给它一些时间来完成动画
        Destroy(animator, destoryTime); // 给它一些时间来完成动画
        StartCoroutine(DieDispaly(animator.transform.localScale.y, 0.1f, destoryTime - 1f));
        Destroy(this, destoryTime);
        Destroy(this.transform.gameObject, destoryTime + 2f);
        
    }
    protected virtual IEnumerator DieDispaly(float v_start, float v_end, float duration)
    {
        float elapsed = 0.0f;
        float value;
        while (elapsed < duration)
        {
            value = Mathf.Lerp(v_start, v_end, elapsed / duration);
            elapsed += Time.deltaTime;
            animator.transform.localScale = new Vector3(animator.transform.localScale.x, value, animator.transform.localScale.z);
            yield return null;
        }
    }
    /// <summary>
    /// 生成鬼魂
    /// </summary>
    public virtual void CreatSoul()
    {
        if (unitSoul == null) return;

        if (unitTemplate.unitType == UnitTemplate.UnitType.Virus)
        {
            stateMachineManager.exTextUI.text = (int.Parse(stateMachineManager.exTextUI.text) + unitSoul.unitSoulTemplate.value).ToString();
        }

        for (int i = 0; i < level; i++)
        {
            UnitSoul soul = Instantiate(unitSoul, transform.position, Quaternion.identity);
            if (unitTemplate.unitType == UnitTemplate.UnitType.Cell)
            {
                skinName = SkeletonRenderer.Skeleton.Skin.Name;
                if (skinName == "")
                {
                    skinName = "red";
                }
                soul.SkeletonRenderer.Skeleton.SetSkin(skinName);
            }
        }



    }
    /// <summary>
    /// 受到攻击
    /// </summary>
	public void SufferAttack(int damage)
    {
        if (state == UnitState.Dead)
        {
            // 已经死了
            return;
        }
        // 濒死
        if (damage * 2 >= unitTemplate.health || unitTemplate.health / maxHealth == 0.2f)
        {
            if (transform.TryGetComponent(out SoldierBehaviors s))
            {
                stateMachineManager.OnUnitDying(s, damage);
            };
        }

        unitTemplate.health -= damage;
        // 血条 着色器
        currentHP = unitTemplate.health;
        shaderHP = Remap(currentHP, 0, maxHealth, 0, 1);
        if(Local())
        {
            UpdateMatHealth(shaderHP);
        }else
        {
            Server_UpdateMatHealth(shaderHP);
        }


        if (unitTemplate.health <= 0)
        {
            unitTemplate.health = 0;
            RunEffect(EffectTemplate.EffectType.End);
            Die(unitTemplate.destoryTime);
        }
    }
    public void SufferAttackSimple(int damage)
    {
        if (state == UnitState.Dead)
        {
            // 已经死了
            return;
        }
        // 濒死
        // if (damage * 2 >= unitTemplate.health || unitTemplate.health / maxHealth == 0.2f)
        // {
        //     if (transform.TryGetComponent(out Soldier s))
        //     {
        //         stateMachineManager.OnUnitDying(s, damage);
        //     };
        // }
        
        switch(Weapon.thisWeaponType)
        {
            case WeaponTemplate.WeaponType.Shield:
                if(Local())
                {
                    Sound_BeenAttacked_Shield();
                }else
                {
                    Server_Sound_BeenAttacked_Shield();
                }
                
                break;
        }
        OnBeenAttacked?.Invoke(damage);
        unitTemplate.health -= damage;
        // 血条 着色器
        currentHP = unitTemplate.health;
        shaderHP = Remap(currentHP, 0, maxHealth, 0, 1);
        if(Local())
        {
            UpdateMatHealth(shaderHP);
        }else
        {
            Server_UpdateMatHealth(shaderHP);
        }
        if (unitTemplate.health <= 0)
        {
            unitTemplate.health = 0;
            RunEffect(EffectTemplate.EffectType.End);
            DieSimple(unitTemplate.destoryTime);
        }
    }
    public void SufferAddHealthSimple(int numb)
    {
        if (state == UnitState.Dead)
        {
            // 已经死了
            return;
        }
        // 濒死
        // if (damage * 2 >= unitTemplate.health || unitTemplate.health / maxHealth == 0.2f)
        // {
        //     if (transform.TryGetComponent(out Soldier s))
        //     {
        //         stateMachineManager.OnUnitDying(s, damage);
        //     };
        // }

        unitTemplate.health += numb;
        // 血条 着色器
        currentHP = unitTemplate.health;
        shaderHP = Remap(currentHP, 0, maxHealth, 0, 1);
        if(Local())
        {
            UpdateMatHealth(shaderHP);
        }else
        {
            Server_UpdateMatHealth(shaderHP);
        }
        if (unitTemplate.health >= 0)
        {
            unitTemplate.health = (int)maxHealth;
            RunEffect(EffectTemplate.EffectType.Start);
        }
    }

    /// <summary>
    /// 在主动攻击范围内，开始攻击
    /// </summary>
    protected virtual void StartAttacking()
    {
        if (!navMeshAgent)
        {
            return;
        }
        if (!IsDeadOrNull(targetOfAttack))
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

        while (targetOfAttack != null)
        {

            RunEffect(EffectTemplate.EffectType.Attacking);
            // BossAttack();
            animator.SetTrigger("DoAttack");
            OnAttacking?.Invoke();
            WeaponDisplay();
            targetOfAttack.SufferAttack(unitTemplate.attackPower);

            yield return new WaitForSeconds(1f / unitTemplate.attackSpeed);

            if (IsDeadOrNull(targetOfAttack))
            {
                animator.SetTrigger("InterruptAttack");
                OnAttackFinish?.Invoke();
                break;

            }

            if (state == UnitState.Dead)
            {
                yield break;
            }

            if (Vector3.Distance(targetOfAttack.transform.position, transform.position) > unitTemplate.engageDistance)
            {
                MoveToAttack(targetOfAttack);
            }
        }

        if (state == UnitState.Attacking)
        {
            Guard();
        }
    }
    public virtual IEnumerator DealAttackSimple()
    {
        while (targetOfAttack != null)
        {
            if(!animator)break;
            RunEffect(EffectTemplate.EffectType.Attacking);
            animator.SetTrigger("DoAttack");
            switch(Weapon.thisWeaponType)
            {
                case WeaponTemplate.WeaponType.Bow:
                    Sound_Attack_Remote();
                    break;
                case WeaponTemplate.WeaponType.Sword:
                    Sound_Attack_Melee();
                    break;
                case WeaponTemplate.WeaponType.Spear:
                    Sound_Attack_Mid();
                    break;
            }
            OnAttacking?.Invoke();
            WeaponDisplay();
            targetOfAttack.SufferAttackSimple(unitTemplate.attackPower);
            yield return new WaitForSeconds(1f / unitTemplate.attackSpeed);
            if (IsDeadOrNull(targetOfAttack))
            {
                animator.SetTrigger("InterruptAttack");
                OnAttackFinish?.Invoke();
                break;
            }
            if (state == UnitState.Dead)
            {
                yield break;
            }
        }

        if (state == UnitState.Attacking)
        {
            GuardSimple();
        }
    }
    
    public void WeaponDisplay()
    {
        if (weaponType == WeaponTemplate.WeaponType.Null) return;
        switch (weaponType)
        {
            case WeaponTemplate.WeaponType.Sword:

                break;
            case WeaponTemplate.WeaponType.Bow:
                // weapon.Shoot(targetOfAttack.transform.position);
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
        if (selectionCircle)
        {
            Color newColor = selectionCircleColor;
            newColor.a = (selected) ? 1f : .6f;
            selectionCircle.color = newColor;
        }

    }
    /// <summary>
    /// 停下来注意附近的敌人
    /// </summary>
    public void Guard()
    {
        if(!navMeshAgent)return;
        if(!navMeshAgent.enabled)return;
        // 返回 梅尔指挥 的指定区域
        RunEffect(EffectTemplate.EffectType.Active);
        state = UnitState.Guarding;
        targetOfAttack = null;
        isReady = false;
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
    }
    public void GuardSimple()
    {
        state = UnitState.Guarding;
        targetOfAttack = null;
        isReady = false;
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
    
    void Sound_Died()
    {
        string Sound_Died = "Sound_Died";
        AudioSystemManager.Instance.PlaySoundSimpleTemp(Sound_Died,1.0f);
    }
    void Sound_BeenAttacked_Shield()
    {
        float soundVolume = Random.Range(0.1f,0.5f);
        string Sound_BeenAttacked_Shield = "Sound_BeenAttacked_Shield";
        AudioSystemManager.Instance.PlaySoundSimpleTemp(Sound_BeenAttacked_Shield,soundVolume);
    }
    public void Sound_Attack_Remote()
    {
        float soundVolume = Random.Range(0.1f,0.5f);
        string Sound_Attack_Remote = "Sound_Attack_Remote";
        AudioSystemManager.Instance.PlaySoundSimpleTemp(Sound_Attack_Remote,soundVolume);
    }
    public void Sound_Attack_Melee()
    {
        float soundVolume = Random.Range(0.1f,0.5f);
        string Sound_Attack_Melee = "Sound_Attack_Melee";
        AudioSystemManager.Instance.PlaySoundSimpleTemp(Sound_Attack_Melee,soundVolume);
    }
    public void Sound_Attack_Mid()
    {
        float soundVolume = Random.Range(0.1f,0.5f);
        string Sound_Attack_Mid = "Sound_Attack_Mid";
        AudioSystemManager.Instance.PlaySoundSimpleTemp(Sound_Attack_Mid,soundVolume);
    }
    public void Sound_FrequentBubbles()
    {
        string Sound_FrequentBubbles = "Sound_FrequentBubbles";
        AudioSystemManager.Instance.PlaySoundSimpleTemp(Sound_FrequentBubbles,0.1f);
    }
    public void Sound_Bite_Swallow_Enhancement()
    {
        string Sound_Bite = "Sound_Bite";
        AudioSystemManager.Instance.PlaySoundSimpleTemp(Sound_Bite,15.5f);
        string Sound_Swallow = "Sound_Swallow";
        AudioSystemManager.Instance.PlaySoundSimpleTemp(Sound_Swallow,5.5f,2.0f);
        string Sound_Enhancement = "Sound_Enhancement";
        AudioSystemManager.Instance.PlaySoundSimpleTemp(Sound_Enhancement,1.5f,0.5f);
    }
    public void Sound_Mechanism_WeakAss()
    {
        string Sound_Mechanism_WeakAss = "Sound_Mechanism_WeakAss";
        float randomVolum = Random.Range(0.1f,0.7f);
        float randomDelay = Random.Range(0.0f,0.7f);
        AudioSystemManager.Instance.PlaySoundSimpleTemp(Sound_Mechanism_WeakAss,randomVolum,randomDelay);
    }
    #endregion 数据操作
    #region 联网数据操作
    public bool Local()
    {
        if(RunModeData.CurrentRunMode == RunMode.Local)return true;
        return false;
    }
    [Server]
    public void Server_UpdateMatHealth(float SetFloat)
    {
        UpdateMatHealth(SetFloat);
        Client_UpdateMatHealth(SetFloat);
    }
    [ClientRpc]
    public void Client_UpdateMatHealth(float SetFloat)
    {
        UpdateMatHealth(SetFloat);
    }
    [Server]
    public void Server_Sound_Mechanism_WeakAss()
    {
        Sound_Mechanism_WeakAss();
        Client_Sound_Mechanism_WeakAss();
    }
    [ClientRpc]
    public void Client_Sound_Mechanism_WeakAss()
    {
        if(ServerLogic.Local_palayer != unitTemplate.player)return;
        Sound_Mechanism_WeakAss();
    }
    [Server]
    public void Server_Sound_Bite_Swallow_Enhancement()
    {
        Sound_Bite_Swallow_Enhancement();
        Client_Sound_Bite_Swallow_Enhancement();
    }
    [ClientRpc]
    public void Client_Sound_Bite_Swallow_Enhancement()
    {
        if(ServerLogic.Local_palayer != unitTemplate.player)return;
        Sound_Bite_Swallow_Enhancement();
    }
    [ClientRpc]
    public void Client_Sound_Attack_Mid()
    {
        if(ServerLogic.Local_palayer != unitTemplate.player)return;
        Sound_Attack_Mid();
    }
    [ClientRpc]
    public void Client_Sound_Attack_Melee()
    {
        if(ServerLogic.Local_palayer != unitTemplate.player)return;
        Sound_Attack_Melee();
    }
    [ClientRpc]
    public void Client_Sound_Attack_Remote()
    {
        if(ServerLogic.Local_palayer != unitTemplate.player)return;
        Sound_Attack_Remote();
    }
    [Server]
    void Server_Sound_BeenAttacked_Shield()
    {
        Sound_BeenAttacked_Shield();
        Client_Sound_BeenAttacked_Shield();
    }
    [ClientRpc]
    void Client_Sound_BeenAttacked_Shield()
    {
        if(ServerLogic.Local_palayer != unitTemplate.player)return;
        Sound_BeenAttacked_Shield();
    }
    [Server]
    void Server_Sound_Died()
    {
        Sound_Died();
        Client_Sound_Died();
    }
    [ClientRpc]
    void Client_Sound_Died()
    {
        if(ServerLogic.Local_palayer != unitTemplate.player)return;
        Sound_Died();
    }
    [Server]
    public virtual IEnumerator Server_DealAttackSimple()
    {
        while (targetOfAttack != null)
        {
            if(!animator)break;
            RunEffect(EffectTemplate.EffectType.Attacking);
            animator.SetTrigger("DoAttack");
            switch(Weapon.thisWeaponType)
            {
                case WeaponTemplate.WeaponType.Bow:
                    Sound_Attack_Remote();
                    Client_Sound_Attack_Remote();
                    break;
                case WeaponTemplate.WeaponType.Sword:
                    Sound_Attack_Melee();
                    Client_Sound_Attack_Melee();
                    break;
                case WeaponTemplate.WeaponType.Spear:
                    Sound_Attack_Mid();
                    Client_Sound_Attack_Mid();
                    break;
            }
            Client_DealAttackSimple_DoAttackAnimation();
            OnAttacking?.Invoke();
            WeaponDisplay();
            targetOfAttack.SufferAttackSimple(unitTemplate.attackPower);
            yield return new WaitForSeconds(1f / unitTemplate.attackSpeed);
            if (IsDeadOrNull(targetOfAttack))
            {
                animator.SetTrigger("InterruptAttack");
                Client_DealAttackSimple_InterruptAttackAnimation();
                OnAttackFinish?.Invoke();
                break;
            }
            if (state == UnitState.Dead)
            {
                yield break;
            }
        }

        if (state == UnitState.Attacking)
        {
            GuardSimple();
        }
    }
    [ClientRpc]
    public void Client_DealAttackSimple_DoAttackAnimation()
    {
        animator.SetTrigger("DoAttack");
    }
    [ClientRpc]
    public void Client_DealAttackSimple_InterruptAttackAnimation()
    {
        animator.SetTrigger("InterruptAttack");
    }
    
    #endregion 联网数据操作
}
