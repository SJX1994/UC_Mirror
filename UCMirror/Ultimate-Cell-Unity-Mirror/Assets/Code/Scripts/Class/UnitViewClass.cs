using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色显示信息
/// </summary>
public class UnitViewClass
{
    /// <summary>
    /// Int - 角色Id
    /// </summary>
    public int ConfigId;

    /// <summary>
    /// Int - 顶部UI配置信息 
    /// ( -1 ) 未配置
    /// (0, 1, 2, 3) 第1，2，3，4位
    /// </summary>
    public int TopConfig;

    /// <summary>
    /// Bool - 锁定状态 
    ///     true - 已锁定
    ///     false - 未锁定
    /// </summary>
    public bool LockType;

    /// <summary>
    /// 角色基础信息
    /// </summary>
    public UnitBasicClass BasicInfo;

    /// <summary>
    /// 角色数值信息
    /// </summary>
    public UnitAttackInfoClass AttackInfo;

    /// <summary>
    /// 临时数据
    /// </summary>
    public HeroInfoConfigCategory.HeroInfoCategory ExcelTemp;
}




