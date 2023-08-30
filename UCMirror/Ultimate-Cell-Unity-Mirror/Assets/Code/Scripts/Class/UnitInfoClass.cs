using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfoClass
{
    // Unit全局唯一Id
    public int UnitIndexId;

    // Unit逻辑位置
    public Vector2 UnitPos;

    // Unit实际偏移位置
    public Vector3 UnitPosUse;

    // 是否为新建Unit
    public bool CreateUnit;

    //随机细胞颜色
    public EventType.UnitColor color;

    // Unit等级
    public int UnitLevel;

    // 增加小人速度
}
