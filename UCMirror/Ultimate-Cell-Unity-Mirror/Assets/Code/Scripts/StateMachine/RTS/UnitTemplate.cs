
using UnityEngine;
using UC_PlayerData;
[CreateAssetMenu(fileName = "SpecificUnit", menuName = "状态机/创建单位模板", order = 1)]

public class UnitTemplate: ScriptableObject
{
    public UnitType unitType = UnitType.Cell;
	public Player player = Player.NotReady;
    [Tooltip("血量")]
	public int health = 10;
    [Tooltip("攻击力")]
	public int attackPower = 2;
    [Tooltip("攻速")]
	public float attackSpeed = 1f;
    [Tooltip("攻击范围")]
	public float engageDistance = 1f;
    [Tooltip("巡逻范围")]
	public float guardDistance = 5f;
    [Tooltip("遇敌追击时长")]
	public float chaseDuration = 1f;
    [Tooltip("死亡消除时间")]
	public float destoryTime = 8f;
    [Tooltip("等级表现")]
	public Sprite levelSprite = null;
    public enum UnitType
	{
		Cell,
		Virus,
		NoType
	}
	public Player GetOtherPlayerType()
	{
		if(player == Player.Player1)
		{
			return Player.Player2;
		}else if(player == Player.Player2)
		{
			return Player.Player1;
		}else	
		{
			return Player.NotReady;
		}
	}
    public UnitType GetOtherUnitType()
	{
		if(unitType == UnitType.Cell)
		{
			return UnitType.Virus;
			
		}else if(unitType == UnitType.Virus)
		{
			return UnitType.Cell;
		}else
		{
			return UnitType.NoType;
		}
	}
}
