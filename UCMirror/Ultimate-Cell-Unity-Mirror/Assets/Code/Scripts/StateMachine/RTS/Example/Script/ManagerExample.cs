using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerExample : MonoBehaviour
{
    public List<Unit> heroUnits;
    public List<Transform> heroTargetPositions;
    public AICommand.CommandType heroCommandType;
    public List<Unit> cellUnits;
    public List<Transform> cellTargetPositions;
    public AICommand.CommandType cellCommandType;
    public List<Unit> virusUnits;
    public List<Transform> virusTargetPositions;
    public AICommand.CommandType virusCommandType;
    void Start()
    {
        // InvokeRepeating("m_ExcutingCommand", 0, 1);

        // 死亡事件监听
        for(int i = 0; i < cellUnits.Count; i++)
        {
            cellUnits[i].OnDie += UnitDeadHandler;
        }
        for(int i = 0; i < virusUnits.Count; i++)
        {
            virusUnits[i].OnDie += UnitDeadHandler;
        }
    }
    /// <summary>
    /// 获取单位信息 例子
    /// 包含：单位名字，单位类型，单位血量，单位攻击力，单位攻速，单位主动攻击范围，单位巡逻攻击范围
    /// </summary>
    void GetUnitInfo()
    {
        for(int i = 0; i < cellUnits.Count; i++)
        {
            Debug.Log($"cell {i} : {cellUnits[i].name} {cellUnits[i].unitTemplate.unitType.ToString()} {cellUnits[i].unitTemplate.health.ToString()}...");
        }
        for(int i = 0; i < virusUnits.Count; i++)
        {
            Debug.Log($"virus {i} : {virusUnits[i].name} {virusUnits[i].unitTemplate.unitType.ToString()} {virusUnits[i].unitTemplate.health.ToString()}...");
        }
    }

    /// <summary>
    /// 给单位下达指令 例子
    /// </summary>
    public void m_ExcutingCommand()
    {
        if(heroTargetPositions.Count == heroUnits.Count )
        {
            for(int i = 0; i < heroUnits.Count; i++)
            {
                AICommand newCommand = new AICommand(heroCommandType, heroTargetPositions[i].transform.position);
                heroUnits[i].ExecuteCommand(newCommand);
            }
        }else
        {
            Debug.LogWarning("hero 人数不对等");
        }

        if(cellTargetPositions.Count == cellUnits.Count)
        {
            for(int i = 0; i < cellUnits.Count; i++)
            {
                AICommand newCommand = new AICommand(cellCommandType, cellTargetPositions[i].transform.position);
                cellUnits[i].ExecuteCommand(newCommand);
            }
        }else
        {
            Debug.LogWarning("cell 人数不对等");
        }

        if(virusTargetPositions.Count == virusUnits.Count)
        {
            for(int i = 0; i < virusUnits.Count; i++)
            {
                AICommand newCommand = new AICommand(virusCommandType, virusTargetPositions[i].transform.position);
                virusUnits[i].ExecuteCommand(newCommand);
            }
        }else
        {
            Debug.LogWarning("virus 人数不对等");
        }
         
        
        
    }

    /// <summary>
    /// 死亡事件监听 例子
    /// </summary>
    /// <param name="whoDied"> 作战单位 </param>
    private void UnitDeadHandler(Unit whoDied)
	{
        Debug.Log("！！死亡提示！！");
		Debug.Log( $"{whoDied.name} 已死亡.属于：{whoDied.unitTemplate.unitType.ToString()} 阵营");
        whoDied.OnDie -= UnitDeadHandler;
	}
    

}
