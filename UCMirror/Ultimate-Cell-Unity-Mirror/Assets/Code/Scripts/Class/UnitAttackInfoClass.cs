using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色数值信息
/// </summary>
public class UnitAttackInfoClass
{
    /// <summary>
    /// Int - 角色ID
    /// </summary>
    public int UnitId;

    /// <summary>
    /// Int - 角色血量
    /// </summary>
    public int Health;

    /// <summary>
    /// Int - 角色攻击力
    /// </summary>
    public int Aggressivity;

    /// <summary>
    /// Float - 角色速度
    /// </summary>
    public float Speed;

    /// <summary>
    /// Float - 角色攻击速度
    /// </summary>
    public float AttackSpeed;

    /// <summary>
    /// Float - 角色攻击范围
    /// </summary>
    public float AttackRange;

    /// <summary>
    /// Float - 角色巡逻攻击范围
    /// </summary>
    public float GuardDistance;

    /// <summary>
    /// Int[] - 角色技能ID
    /// </summary>
    public int[] SkillId;
}


