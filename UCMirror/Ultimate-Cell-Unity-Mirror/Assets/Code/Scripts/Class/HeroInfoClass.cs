
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroInfoClass
{
    /// <summary>
    /// 英雄配置ID
    /// </summary>
    /// 3001 爆爆
    /// 3002 找找
    /// 3003 维克
    public int HeroConfigId;

    /// <summary>
    /// 全局唯一Id
    /// </summary>
    public int IndexId;

    /// <summary>
    /// 英雄名称
    /// </summary>
    public string HeroName;

    /// <summary>
    /// 英雄信息
    /// </summary>
    public Dictionary<int, TetrisClass> HeroInfo;

    /// <summary>
    /// 英雄位置
    /// </summary>
    public Vector3 HeroPos;

}

