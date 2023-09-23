using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using PathCreation;
public class UnitFear : Unit,IBossUnit    
{
#region 数据对象
      [HideInInspector]
      
      public List<Unit> cells = new();
// 路径
      public PathCreator pathCreator;
      public EndOfPathInstruction endOfPathInstruction;
      float distanceTravelled;
      public float speed = 0.5f;
// 路径
      public  Dictionary<Unit,AICommand>  virusCreatedByFears = new();
      private int skill0AttackPower = 30;
      private int skill1AttackPower = 1000;
      
      int idTemp = 10000;
      // private StateMachineManager stateMachineManager;
      bool  m_CR_attack_running;
      Unit targetCell;
      Vector3 tempPosition;
      public GameObject skill0Prefab;
      public ParticleSystem skill2ParticleSystem;
      public GameObject skateboard;
      public Unit self => transform.GetComponent<UnitFear>();
      public Unit virusUnit;
      bool skill3Done = false;
// 通讯对象
    private GameObject sceneLoader;
    private CommunicationInteractionManager CommunicationManager;
    private BroadcastClass broadcastClass;

#endregion 数据对象
#region 数据关系
      public override void Start()
      {
            base.Start();
            if (GameObject.Find("CanvasManager_StayMachine") == null || GameObject.Find("CanvasManager_StayMachine(Clone)") == null)
            {
                  return;
            }
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }

            stateMachineManager = GameObject.Find("CanvasManager_StayMachine(Clone)").gameObject.GetComponent<StateMachineManager>();
            skill0Prefab = transform.Find("FearSkill0").gameObject;
            skill2ParticleSystem = transform.Find("FearSkill2").gameObject.GetComponent<ParticleSystem>();
            skill2ParticleSystem.gameObject.SetActive(false);
            skill0Prefab.SetActive(false);
            m_CR_attack_running = false;
            
            // BossMove();
            targetCell = GetNearestHostileUnit();
            // 通讯
            // 暂时获取方式
            if (GameObject.Find("LanNetWorkManager") == null)
            {
                  return;
            }
            sceneLoader = GameObject.Find("LanNetWorkManager").gameObject;
            // 全局通信方法管理
            CommunicationManager = sceneLoader.GetComponent<CommunicationInteractionManager>();
            // 全局通信事件注册类
            broadcastClass = sceneLoader.GetComponent<BroadcastClass>();
          
            
      }
      public override void Update()
      {
            base.Update();
            
            distanceTravelled += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            Vector3 lookAtPostition = new Vector3(this.skateboard.transform.position.x, Camera.main.transform.position.y, this.skateboard.transform.position.z - 0.1f);
        skateboard.transform.LookAt(lookAtPostition);
        if(!navMeshAgent)return;
	  SetFlip(flip?-navMeshAgent.velocity.x:navMeshAgent.velocity.x);
            if(targetCell == null)return;
            SetFlip(transform.position.x-targetCell.transform.position.x);

            
      }
      // 技能：
      // 0 蔑视一击：当兵线上为1个兵时，普通攻击一个兵
      // 1 重拳出击：当兵线上为2个兵时，吞食一个兵
      // 2 摇人：当兵线上为3-5个兵时，生成两个外尔战士
      // 3 讨厌没有边界感：当兵线上的士兵大于5个时，boss将生成一排士兵，将我方全体士兵往后推一个格
      // 4 割韭菜：当自身血量为50%时，吃掉兵线上的一个兵，恢复20%血量，可以用3次
      // 5 人血馒头：当自身血量为20%时，吃掉自己的两个兵，恢复25%血量，可以用2次
      // 当被兵线推动时消耗5%的血量
      protected override void BossAttack()
      {
            targetCell = GetNearestHostileUnit();
            // --- 假数据
            cells = GetInRangeHostileUnits();
            // --- 假数据
            int num = cells.Count;
            // Debug.LogError(cells.Count);
            skill2ParticleSystem.gameObject.SetActive(false);
            if(num == 1)
            {
                  Skills(0);

            }else if(num == 2)
            {
                  Skills(1);
                  
            }else if(num > 2 && num <= 6)
            {     
                  Skills(2);
            }
            else if( num > 6)
            {

                  if(!skill3Done)
                  {
                        Skills(3);
                  }else
                  {
                        Skills(2);
                  }
                  
            }
            // boss 回血技能
            if(shaderHP > 0.2f && shaderHP <= 0.5f)
            {

            }else if(shaderHP <= 0.2f )
            {

            }
            
            
      }
#endregion 数据关系
#region 数据操作
      /// <summary>
      /// 如果路径在游戏过程中发生变化，更新行进的距离，使跟随者在新路径上的位置尽可能接近其在旧路径上的位置
      /// </summary>
      void OnPathChanged() {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
      }
      private void BossMove()
      {
            Invoke("BossStop",1f / unitTemplate.attackSpeed*2-0.3f);
            Vector3 nextPos = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            // AICommand c2 = new AICommand(AICommand.CommandType.GoToAndGuard,nextPos);
            transform.position = nextPos;
            // this.ExecuteCommand(c2);
            
      }
      private void BossStop()
      {
            Invoke("BossMove",1f / unitTemplate.attackSpeed*2-0.3f);
            // AICommand c2 = new AICommand(AICommand.CommandType.AttackTarget,transform.position);
            // this.ExecuteCommand(c2);
            
      }
      private void Skills(int skillId)
      {
            switch(skillId)
            {
                  case 0:
                        float duration0 = 0.5f;
                        targetCell = cells[0];
                        tempPosition = transform.position;
                        skill0Prefab.SetActive(true);
                        skill0Prefab.transform.localScale *= 2.0f;
                        skill0Prefab.transform.DOScale(skill0Prefab.transform.localScale*0.5f,duration0).SetEase(Ease.InBounce);
                        skill0Prefab.transform.DOMove(targetCell.transform.position,duration0).SetEase(Ease.OutCubic);
                        // 面向摄像机
                        Vector3 targetPostition = new Vector3(this.targetCell.transform.position.x, Camera.main.transform.position.y, this.targetCell.transform.position.z - 0.1f);
                        skill0Prefab.transform.LookAt(targetPostition);
                        Invoke("Skill0",0.5f);
                  break;
                  case 1:
                        float duration1 = 0.5f;
                        targetCell = cells[Random.Range(0, cells.Count)];
                        targetCell.OnDie += FearCellDie;
                        tempPosition = transform.position;
                        transform.DOMove(targetCell.transform.position,duration1).SetEase(Ease.OutBounce);
                        Invoke("Skill1",0.5f);
                  break;
                  case 2:
                        // --- 假数据
                        UnitInfoClass info = new UnitInfoClass();
                        info.UnitPosUse = new Vector3(transform.position.x-1f,transform.position.y,transform.position.z-1f);
                        idTemp += 1;
                        info.UnitIndexId = idTemp;
                        UnitInfoClass info2 = new UnitInfoClass();
                        info2.UnitPosUse = new Vector3(transform.position.x-1f,transform.position.y,transform.position.z+1f);
                        idTemp += 1;
                        info2.UnitIndexId = idTemp;
                        AICommand c1 = new AICommand(AICommand.CommandType.GoToAndGuard,cells[0].transform.position);
                        AICommand c2 = new AICommand(AICommand.CommandType.GoToAndGuard,cells[2].transform.position);
                        // --- 假数据
                        Unit unit1 =  Instantiate(virusUnit, info.UnitPosUse, Quaternion.identity);
                        Unit unit2 =  Instantiate(virusUnit, info.UnitPosUse, Quaternion.identity);

                        unit1.transform.Find("Spine").transform.localScale *= 0.5f;
                        unit2.transform.Find("Spine").transform.localScale *= 0.5f;

                        unit1.OnDie += FearVirusDie;
                        unit2.OnDie += FearVirusDie;
                        unit1.unitTemplate.health = 60;
                        unit2.unitTemplate.health = 60;
                        virusCreatedByFears.Add(unit1,c1);
                        virusCreatedByFears.Add(unit2,c2);
                        Invoke("Skill2",0.1f);
                  break;
                  case 3:
                        // 生成一条外尔战士
                        if (CommunicationManager == null) 
                        {
                            CommunicationManager = GameObject.Find("LanNetWorkManager").gameObject.GetComponent<CommunicationInteractionManager>();
                        }
                        CommunicationManager.FireSkillBoss(3);

                        skill3Done = true;
                        // 先撒钱
                        skill2ParticleSystem.gameObject.SetActive(true);
                        // Invoke("Skill3",0.1f);
                  break;
            }
      }
      private void Skill0()
      {
            targetCell.SufferAttack(skill0AttackPower);
            skill0Prefab.transform.DOMove(tempPosition,1f).SetEase(Ease.InOutBack).onComplete = () => {
                skill0Prefab.SetActive(false);
            };
      }
      private void Skill1()
      {
            targetCell.SufferAttack(skill1AttackPower);
            transform.DOMove(tempPosition,0.5f).SetEase(Ease.OutElastic);
      }
      private void Skill2()
      {
            List<Unit> needRemoves = new();
            skill2ParticleSystem.gameObject.SetActive(true);
            foreach(var virusCreatedByFear in virusCreatedByFears)
            {
                  virusCreatedByFear.Key.ExecuteCommand(virusCreatedByFear.Value);
                  Destroy(virusCreatedByFear.Key.gameObject,21f);
                  needRemoves.Add(virusCreatedByFear.Key);
            }
            foreach(var needRemove in needRemoves )
            {
                  FearVirusDie(needRemove);
            }
      }
      private void Skill3()
      {
            
            Debug.LogError("Boss技能3！");
            
           
      }
      private void FearCellDie(Unit unit)
      {
            unit.OnDie -= FearCellDie;
            cells.Remove(unit);
      }
      private void FearVirusDie(Unit unit)
      {
            unit.OnDie -= FearVirusDie;
            virusCreatedByFears.Remove(unit);
      }
      protected override void StartAttacking()
	{
            
		if(!navMeshAgent)return;
		if(!IsDeadOrNull(targetOfAttack))
		{
			state = UnitState.Attacking;
			isReady = false;
			navMeshAgent.isStopped = true;
			if(! m_CR_attack_running)
                  {     
                    StartCoroutine(DealAttack());
                  }
			
		}
		else
		{
			Guard();
		}
	}
      public override IEnumerator DealAttack()
      {
            
            // yield return new WaitForSeconds(1f / unitTemplate.attackSpeed);

            while(targetOfAttack != null)
		{
                  m_CR_attack_running = true;
                  RunEffect(EffectTemplate.EffectType.Attacking);
                  BossAttack();
                  animator.SetTrigger("DoAttack");
                        
                  yield return new WaitForSeconds(1f / unitTemplate.attackSpeed);

                  if(IsDeadOrNull(targetOfAttack))
                  {
                        animator.SetTrigger("InterruptAttack");
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
             m_CR_attack_running = false;
		if(state == UnitState.Attacking)
		{
			Guard();
		}
      }

       private List<Unit> GetInRangeHostileUnits()
	{
        
		cells = GameObject.FindGameObjectsWithTag(unitTemplate.GetOtherUnitType().ToString()).Select(x => x.GetComponent<Unit>()).ToList();

		List<Unit> nearestEnemys = new List<Unit>();
		for(int i=0; i<cells.Count; i++)
		{
			if(IsDeadOrNull(cells[i]))
			{
				continue;
			}

			float distanceFromHostile = Vector3.Distance(cells[i].transform.position, transform.position);
			if(distanceFromHostile <= unitTemplate.engageDistance)
			{
					nearestEnemys.Add(cells[i]);
					
			}
		}

		return nearestEnemys;
	}
#endregion 数据操作
}
