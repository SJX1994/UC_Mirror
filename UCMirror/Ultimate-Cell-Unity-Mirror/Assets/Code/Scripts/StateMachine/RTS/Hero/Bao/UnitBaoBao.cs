using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using Spine.Unity;
public class UnitBaoBao : Unit, IHeroUnit
{
#region 数据对象
     

      private List<Unit> virusUnits;
      public UnitHeroTemplate unitHeroTemplate;
#endregion 数据对象
#region 数据关系
      public override void Start()
      {
            base.Start();
            unitHeroTemplate = Instantiate<UnitHeroTemplate>(unitHeroTemplate);
            // StartCoroutine(WhenCreatMoveTo(new Vector3(7.0f,0.6f,-2.0f+Random.Range(-4.5f,4.5f))));
            InvokeRepeating("GetInRangeHostileUnits", 0, 0.5f);
      }
      public override void Update()
      {
            base.Update();
            
      }
      public UnitHeroTemplate OnCreating()
	{
		return unitHeroTemplate;
	}
	public UnitHeroTemplate OnDefeated()
	{
		return unitHeroTemplate;
	}
      public IEnumerator WhenCreatMoveTo(Vector3 position)
      {
            yield return new WaitForSeconds(1);
            // 生成后的移动位置
            AICommand newCommand = new AICommand(AICommand.CommandType.GoToAndGuard, position);
            this.ExecuteCommand(newCommand);
            yield return new WaitForSeconds(1);
      }
      public override void CreatSoul()
      {
            if(unitSoul==null)return;
		UnitSoul soul = Instantiate(unitSoul, transform.position, Quaternion.identity);
      }
      public override void Die(float destoryTime)
	{
		state = UnitState.Dead; // 使其不再参与任何状态机逻辑
		animator.SetTrigger("DoDeath");
		SetSelected(false);
		// 触发一个事件，以便通知 订阅 该单位的任何 侦听器
		if(OnDie != null)
		{
			OnDie(this);
		}
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
#endregion 数据关系
#region 数据操作
      public override IEnumerator DealAttack()
      {
            
            while(targetOfAttack != null)
            {
                this.SufferAttack(this.unitTemplate.attackPower); // 自爆伤害
                RunEffect(EffectTemplate.EffectType.Attacking);
                animator.SetTrigger("DoAttack");

                
                virusUnits = GetInRangeHostileUnits();

                foreach (Unit virusUnit in virusUnits)
                {
                    virusUnit.SufferAttack(unitTemplate.attackPower);
                }
                yield return new WaitForSeconds(1f / unitTemplate.attackSpeed);

                if(IsDeadOrNull(targetOfAttack))
                {
                    //animator.SetTrigger("InterruptAttack");
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
    private List<Unit> GetInRangeHostileUnits()
	{
        
		virusUnits = GameObject.FindGameObjectsWithTag(unitTemplate.GetOtherUnitType().ToString()).Select(x => x.GetComponent<Unit>()).ToList();

		List<Unit> nearestEnemys = new List<Unit>();
		for(int i=0; i<virusUnits.Count(); i++)
		{
			if(IsDeadOrNull(virusUnits[i]))
			{
				continue;
			}

			float distanceFromHostile = Vector3.Distance(virusUnits[i].transform.position, transform.position);
			if(distanceFromHostile <= unitTemplate.engageDistance)
			{
					nearestEnemys .Add(virusUnits[i]) ;
					
			}
		}

		return nearestEnemys;
	}
#endregion 数据操作
}
