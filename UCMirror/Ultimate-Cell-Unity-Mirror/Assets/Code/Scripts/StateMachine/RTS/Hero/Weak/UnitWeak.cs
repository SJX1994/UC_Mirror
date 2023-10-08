using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using Spine.Unity;
public class UnitWeak : Unit, IHeroUnit
{
#region 数据对象
public UnitHeroTemplate unitHeroTemplate;
public RopeBridge weakRope;
private List<RopeBridge> weakRopes = new();
private Unit[] virusUnits;
private List<Unit> controledUnits = new();
private Dictionary<Unit,RopeBridge> rope_virus_dic = new();

const int MAX_CONTROL = 3;

      

      #endregion 数据对象
      #region 数据关系
      public override void Start()
	{
		base.Start();
		ControlCreat();
		InvokeRepeating("ControlingUpdate", 0, 0.5f);
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
         
            AICommand newCommand = new AICommand(AICommand.CommandType.GoToAndGuard, position);
            this.ExecuteCommand(newCommand);
            yield return new WaitForSeconds(0.1f);
      }
	public override void CreatSoul()
      {
            if(unitSoul==null)return;
		UnitSoul soul = Instantiate(unitSoul, transform.position, Quaternion.identity);
      }
	// TODO 不能动
	public override void Die(float destoryTime)
	{
		CancelInvoke("ControlingUpdate");
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
	private void ControlCreat()
	{
		for(int i = 0;i<MAX_CONTROL;i++)
		{
			RopeBridge rope = Instantiate<RopeBridge>(weakRope,SkeletonRenderer.transform);
			rope.gameObject.SetActive(false);
			weakRopes.Add(rope);
		}
	}
	private void ControlingUpdate()
	{
			foreach(RopeBridge rope in weakRopes)
			{
				if(!rope.targetUnit)
				{
					rope.gameObject.SetActive(false);
					// 寻找新对象
					Unit t = GetNearestHostileUnitWeak();
					if(t != null)
					{
						t.controled = true;
						rope.targetUnit = t;
						rope.gameObject.SetActive(true);
						RopeDisplay(t,rope);
						if(!rope_virus_dic.ContainsKey(t))
						{
							rope_virus_dic.Add(t,rope);
						}
						
					}
				}
			}
		
	}
	private void RopeDisplay(Unit virusUnit,RopeBridge weakRope)
	{
		if(!virusUnit.gameObject.TryGetComponent(out IBossUnit iboss))
		{
			AICommand stop = new AICommand(AICommand.CommandType.Stop);
			virusUnit.ExecuteCommand(stop);
			Vector2 from = new Vector2(virusUnit.transform.position.x,virusUnit.transform.position.z);
			Vector3 center = GetBetweenPoint(virusUnit.transform.position,transform.position);
			Vector2 to = new Vector2(center.x,center.z);
			float duration = 1f;
			weakRope.gameObject.SetActive(true);
			weakRope.targetUnit = virusUnit;
			weakRope.StartPoint.position = new Vector3(transform.position.x,transform.position.y+3f,transform.position.z);
			weakRope.EndPoint.position = virusUnit.transform.position;
			
			if(!rope_virus_dic.ContainsKey(virusUnit))
			{
			rope_virus_dic.Add(virusUnit,weakRope);
			weakRope.EndPoint.SetParent(virusUnit.transform);
			virusUnit.OnDie += UnitDeadHandler;
			weakRope.EndPoint.localPosition = Vector3.zero;
			StartCoroutine(DragUnit(virusUnit.gameObject,from,to,duration));
			}else
			{
				return;
			}
		}
		
		
		
	}
	
	void UnitDeadHandler(Unit whoDied)
	{
		if(rope_virus_dic.TryGetValue(whoDied,out RopeBridge rope))
		{
			rope.gameObject.SetActive(false);
			rope.EndPoint.SetParent(rope.transform);
			rope.EndPoint.localPosition = Vector3.zero;
			rope.targetUnit = null;
		}
		whoDied.OnDie -= UnitDeadHandler;
	}
	private Vector3 GetBetweenPoint(Vector3 start, Vector3 end, float percent=0.5f)
	{
		Vector3 normal = (end - start).normalized;
		float distance = Vector3.Distance(start, end);
		return normal * (distance * percent) + start;
	}


	private IEnumerator DragUnit(GameObject target,Vector2 v_start, Vector2 v_end, float duration)
      {
		
            float elapsed = 0.0f;
            Vector2 value = v_start;
            while (elapsed < duration )
            {
                  value.x = Mathf.Lerp( v_start.x, v_end.x, elapsed / duration );
                  value.y = Mathf.Lerp( v_start.y, v_end.y, elapsed / duration );
                  elapsed += Time.deltaTime;
                  target.transform.position = new Vector3(value.x,transform.position.y,value.y);
                  yield return null;
            }
      }
	protected Unit GetNearestHostileUnitWeak()
	{
		
		virusUnits = GameObject.FindGameObjectsWithTag(unitTemplate.GetOtherUnitType().ToString()).Select(x => x.GetComponent<Unit>()).ToArray();
		foreach(Unit hostile in virusUnits)
		{
			if(hostile.controled == true)
			{
				virusUnits = virusUnits.Except(new Unit[]{hostile}).ToArray();
			}
		}
		Unit nearestEnemy = null;
		float nearestEnemyDistance = 1000f;
		for(int i=0; i<virusUnits.Count(); i++)
		{
			if(IsDeadOrNull(virusUnits[i]))
			{
				continue;
			}

			float distanceFromHostile = Vector3.Distance(virusUnits[i].transform.position, transform.position);
			if(distanceFromHostile <= unitTemplate.guardDistance)
			{
				if(distanceFromHostile < nearestEnemyDistance)
				{
					nearestEnemy = virusUnits[i];
					nearestEnemyDistance = distanceFromHostile;
				}
			}
		}

		return nearestEnemy;
	}
#endregion 数据操作
}