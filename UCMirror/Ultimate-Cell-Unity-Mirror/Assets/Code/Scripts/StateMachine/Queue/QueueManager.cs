using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class UnitGroupInfoClass 
{
    // 总单位数
    public int UnitNumber;
    
}
public class QueueManager : MonoBehaviour
{
#region 数据对象
    public Queue<UnitGroup> unitGroupsQueue= new();
    public UnitGroup unitGroup;
    List<QueueSeat> queueSeats = new();
#endregion 数据对象
#region 数据关系
    void Start()
    {
        // 假数据 UnitGroupInfoClass List
        UnitGroupInfoClass infoList = new();
        infoList.UnitNumber = 15;
        
        
        SeatIndexAssignment();

        for(int i = 0 ; i < infoList.UnitNumber ; i++)
        {
            CreatUnitGroups(i);
        }
        m_CreatTest();
    }
    public void m_CreatTest()
    {
        UpdateQueueIndex();
        InvokeRepeating("UpdateQueueIndex",0.5f,2f);
    }
    public void m_PushTest()
    {
        PopUnitGroup();
        UpdateQueueIndex(); 
    }
    public void m_BackTest()
    {
        CreatUnitGroups(unitGroupsQueue.Count);
        UpdateQueueIndex(); 
    }
#endregion 数据关系
#region 数据操作
    /// <summary>
    /// 更新队列位置信息
    /// </summary>
    void UpdateQueueIndex()
    {
        foreach(UnitGroup unit in unitGroupsQueue)
        {
            StartCoroutine(UpdateUnit(unit));
        }    
    }
    IEnumerator UpdateUnit(UnitGroup unit)
    {
         Vector3 pos = queueSeats.Find((index) => index.SeatIndex == unit.unitGroupID).transform.position;
        // 让排队位置/骚动 增加一点随机性 更有趣
        pos.x += Random.Range(-0.5f,0.5f);
        pos.z += Random.Range(-0.5f,0.5f);
        yield return new WaitForSeconds(Random.Range(0.1f,1.5f));
        MoveTo(pos,unit);
    }
    /// <summary>
    /// Unit AI 移动位置
    /// </summary>
    /// <param name="pos"></param>
    void MoveTo(Vector3 pos, UnitGroup unit)
    {
        AICommand newCommand = new AICommand(AICommand.CommandType.GoToAndGuard, pos);
       
        unit.ExecuteCommand(newCommand);
    }
    /// <summary>
    /// 排序路径上的座位的编号 0-N 。0 最接近队伍的前端 
    /// </summary>
    void SeatIndexAssignment()
    {
        ReverseOrderOfChildren();
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<QueueSeat>().SeatIndex = i;
            queueSeats.Add(transform.GetChild(i).GetComponent<QueueSeat>());
        }
    }
    public void ReverseOrderOfChildren()
    {
        for (var i = 0; i < transform.childCount - 1; i++)
        {
            transform.GetChild(0).SetSiblingIndex(transform.childCount - 1 - i);
        }
    }
    /// <summary>
    /// 生成排队的单位
    /// </summary>
    /// <param name="info"> 传入多少个info创建多少个单位 </param>
    void CreatUnitGroups(int index)
    {
        Vector3 pos = queueSeats.Last().transform.position;
        // 让生成位置增加一点随机性 更有趣
        pos.x += Random.Range(-1f,1f);
        pos.z += Random.Range(-1f,1f);
        UnitGroup temp_unitGroup = Instantiate(unitGroup,pos, Quaternion.identity);
        temp_unitGroup.transform.GetComponent<UnitGroup>().unitGroupID = index;
        // 推入 队列 
        AddUnitGroup(temp_unitGroup);
        UpdateQueueIndex(); 
    }
    /// <summary>
    /// 推入排队的队列（ 队列 ）
    /// </summary>
    /// <param name="unitGroup"></param>
    void AddUnitGroup(UnitGroup unitGroup)
    {
        unitGroupsQueue.Enqueue(unitGroup);
        // 更新 队列 信息
    }
    /// <summary>
    /// 推出 队列 
    /// </summary>
    void PopUnitGroup()
    {
        AICommand newCommand = new AICommand(AICommand.CommandType.Die);
        unitGroupsQueue.First().ExecuteCommand(newCommand);
        Destroy(unitGroupsQueue.First().gameObject,1.5f);
        unitGroupsQueue.Dequeue();
        // 更新 队列 信息
       foreach(UnitGroup unit in unitGroupsQueue)
       {
            unit.unitGroupID-= 1;
       }
    }
#endregion 数据操作
}

