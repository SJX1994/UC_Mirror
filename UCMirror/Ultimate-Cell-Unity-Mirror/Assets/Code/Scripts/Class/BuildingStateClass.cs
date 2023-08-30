using Mirror;
using UnityEngine;

public class BuildingStateClass : MonoBehaviour
{
    // 负值 位置不变
    // 正值 位置改变为当前值
    public float BuildingPosX;

    // 负值 位置不变
    // 正值 位置改变为当前值
    public float BuildingPosY;

    // 0 状态不变
    // 1 触发编组
    // 2 取消编组
    public int FunctionHitState;

    // 玩家区分
    public NetworkConnection conn;

}