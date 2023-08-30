using System.Collections.Generic;
using UnityEngine;

public class SkillInfoManager : SingTon<SkillInfoManager>
{
    /// <summary>
    /// 根据技能Id获取技能基础信息
    /// </summary>
    /// <param name="SkillId"></param>
    /// <returns></returns>
    public static SkillViewClass GetSkillViewInfoBySkillId(int SkillId)
    {
        return UnitInfoService.GetSkillViewInfoBySkillId(SkillId);
    }

    /// <summary>
    /// 根据技能Id获取技能数值信息
    /// </summary>
    /// <param name="SkillId"></param>
    /// <returns></returns>
    public static SkillActionClass GetSkillActionInfoBySkillId(int SkillId)
    {
        return UnitInfoService.GetSkillActionInfoBySkillId(SkillId);
    }

    /// <summary>
    /// 获取随机的想法技能
    /// </summary>
    /// <returns></returns>
    public SkillViewClass GetBattlefiledSkillByRandom()
    {
        var SkillId = Random.Range(1000, 2000);

        return UnitInfoService.GetSkillViewInfoBySkillId(SkillId);
    }

}
