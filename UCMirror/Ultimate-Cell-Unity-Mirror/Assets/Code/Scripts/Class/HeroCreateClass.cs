using System.Collections.Generic;
using UnityEngine;

public class HeroCreateClass
{
    /// <summary>
    /// 生成英雄所需的细胞
    /// </summary>
    public List<UnitInfoClass> CellIndexList = new();

    /// <summary>
    /// 英雄生成位置
    /// </summary>
    public Vector3 CreatePos = new Vector3();

    /// <summary>
    /// 英雄配置信息
    /// </summary>
    public HeroInfoClass HeroInfoClass = null;

}

