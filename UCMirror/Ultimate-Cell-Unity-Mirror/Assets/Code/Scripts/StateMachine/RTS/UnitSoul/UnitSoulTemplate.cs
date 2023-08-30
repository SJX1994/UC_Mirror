
using UnityEngine;

[CreateAssetMenu(fileName = "SpecificUnitSoul", menuName = "状态机/创建单位灵魂模板", order = 1)]

public class UnitSoulTemplate: ScriptableObject
{
    public UnitSoulType unitType = UnitSoulType.Soul_Cell;
    [Tooltip("持续时间")]
	public int duration = 10;
    
    [Tooltip("价值")]
	public int value = 2;
    [Tooltip("游荡范围")]
	public float wanderDistance = 3f;
    public enum UnitSoulType
      {
		Soul_Cell,
		Soul_Virus
		
	}
   
    public UnitSoulType GetOtherUnitType()
	{
		if(unitType == UnitSoulType.Soul_Cell)
		{
			return UnitSoulType.Soul_Virus;
		}
		else
		{
			return UnitSoulType.Soul_Cell;
		}
	}
}
