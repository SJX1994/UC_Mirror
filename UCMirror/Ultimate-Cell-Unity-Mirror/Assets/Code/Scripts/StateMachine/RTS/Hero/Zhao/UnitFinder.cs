using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using Spine.Unity;
public class UnitFinder : Unit, IHeroUnit
{
#region 数据对象


private List<UnitSoul> soulUnits = new();
public UnitHeroTemplate unitHeroTemplate;
public int soulCount_Cell;
public int soulCount_Virus;



#endregion 数据对象
#region 数据关系
	public override void Start()
	{
		base.Start();
		unitHeroTemplate = Instantiate<UnitHeroTemplate>(unitHeroTemplate);
		soulCount_Cell = 0;
		InvokeRepeating("GetInRangeSoulUnits", 0, 0.5f);
		// StartCoroutine(WhenCreatMoveTo(new Vector3(7.0f,0.6f,-2.0f+Random.Range(-4.5f,4.5f))));
		InvokeRepeating("MoveToNearestSoul", 1f, 0.5f);
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
            // yield return new WaitForSeconds(1);
            // // 生成后的移动位置
            AICommand newCommand = new AICommand(AICommand.CommandType.GoToAndGuard, position);
            this.ExecuteCommand(newCommand);
            yield return new WaitForSeconds(0.1f);
		// 找找去寻找灵魂
		InvokeRepeating("MoveToNearestSoul", 1f, 1.5f);
		yield return new WaitForSeconds(0.1f);
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

private void MoveToNearestSoul()
{
	soulUnits = GameObject.FindGameObjectsWithTag("Soul_Cell").Select(x => x.GetComponent<UnitSoul>()).ToList();
	if(soulUnits.Count() == 0)
	{
		return;
	}else
	{
		// 找到最近的灵魂
		UnitSoul nearestSoul = null;
		float nearestSoulDistance = 1000f;
		for(int i=0; i<soulUnits.Count(); i++)
		{

			float distanceFromSoul = Vector3.Distance(soulUnits[i].transform.position, transform.position);
			
			if(distanceFromSoul < nearestSoulDistance)
			{
					nearestSoul = soulUnits[i];
					nearestSoulDistance = distanceFromSoul;
			}
			
		}
		if(nearestSoul == null)
		{
			return;
		}
		// 移动到最近的灵魂
		AICommand newCommand = new AICommand(AICommand.CommandType.GoToAndGuard, nearestSoul.transform.position);
		this.ExecuteCommand(newCommand);
	}
	

}
private List<UnitSoul> GetInRangeSoulUnits()
{
  
	soulUnits = GameObject.FindGameObjectsWithTag("Soul_Cell").Select(x => x.GetComponent<UnitSoul>()).ToList();

	for(int i=0; i<soulUnits.Count(); i++)
	{
		if(IsDeadOrNull(soulUnits[i]))
		{
			continue;
		}

		float distanceFromHostile = Vector3.Distance(soulUnits[i].transform.position, transform.position);
		if(distanceFromHostile <= unitTemplate.engageDistance)
		{
				
				// 收集计数
				if(soulUnits[i].OnCollection == null)
				{
					soulUnits[i].OnCollection += Collecting;
					// 收集表现
					Vector2 temp_from = new Vector2(soulUnits[i].transform.position.x,soulUnits[i].transform.position.z) ;
					Vector2 temp_to = new Vector2(transform.position.x,transform.position.z);
					// StartCoroutine(soulUnits[i].CollectingDispaly(temp_from,temp_to,0.5f,this));
					StartCoroutine(soulUnits[i].CollectingDispaly(temp_from,temp_to,0.5f,soulUnits[i].gameObject));
				}
				
				
				
		}
	}
	return soulUnits;
}
public void Collecting(UnitSoul soul)
{
	bool isAdded = false ;
	if(soul!=null)
	{
		if(soul.unitSoulTemplate.unitType == UnitSoulTemplate.UnitSoulType.Soul_Cell)
		{
			if(!isAdded)
			{
				soul.soulSlider.value += soul.unitSoulTemplate.value;
				soul.OnCollection -= Collecting;
				isAdded = true;
			}
			
		}
	}
	
	
}
#endregion 数据操作
}