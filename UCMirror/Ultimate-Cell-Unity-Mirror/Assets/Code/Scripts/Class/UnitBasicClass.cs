using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色基础信息
/// </summary>
public class UnitBasicClass
{
    /// <summary>
    /// Int - 角色ID
    /// </summary>
    public int UnitId;

    /// <summary>
    /// GameObject - 角色图片样式 
    /// </summary>
    public GameObject CharacterStyle;

    /// <summary>
    /// GameObject - 角色背景特效
    /// </summary>
    public GameObject CharacterEffect;

    /// <summary>
    /// String - 角色年龄
    /// </summary>
    public string Age;

    /// <summary>
    /// Enum - 角色性别
    /// </summary>
    public EventType.Gender gender;

    /// <summary>
    /// String[] - 角色爱好
    /// </summary>
    public string[] Hobby;

    /// <summary>
    /// String[] - 角色标签
    /// </summary>
    public string[] Label;

    /// <summary>
    /// String - 角色背景故事
    /// </summary>
    public string BackGroundStory;

    /// <summary>
    /// DicTionary<角色ID, 文字描述> - 角色关系网
    /// </summary>
    public Dictionary<int, string> NetWork;

    /// <summary>
    /// String - 角色名称
    /// </summary>
    public string UnitName;

    /// <summary>
    /// Enum - 角色阵营
    /// </summary>
    public EventType.UnitCamp unitcamp;

    /// <summary>
    /// 角色技能信息
    /// </summary>
    public List<SkillViewClass> SkillViewInfo;

    /// <summary>
    /// exccel 临时数据
    /// </summary>
    public HeroInfoConfigCategory.HeroInfoCategory excelTemp;
}

