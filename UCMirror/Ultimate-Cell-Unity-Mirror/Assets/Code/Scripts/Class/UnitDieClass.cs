using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色死亡信息传输
/// </summary>
public class UnitDieClass
{
    /// <summary>
    /// Int - 角色ID
    /// </summary>
    public int UnitId;

    /// <summary>
    /// String - 角色名称
    /// </summary>
    public string UnitName;

    /// <summary>
    /// Enum - 角色阵营
    /// </summary>
    public EventType.UnitCamp unitCamp;
}


