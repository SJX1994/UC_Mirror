using System.Collections.Generic;
using UnityEngine;

public class UnitInfoManager : SingTon<UnitInfoManager>
{
    /// <summary>
    /// 获取已配置的四个英雄
    /// </summary>
    /// <returns></returns>
    public static List<UnitViewClass> GetBeConfigUnit()
    {
        return UnitInfoService.GetBeConfigUnit();
    }

    /// <summary>
    /// 配置顶部四个英雄的数据
    /// </summary>
    /// <param name="HeroId"></param>
    /// <param name="ChangeHeroId"></param>
    public static void SetBeConfigHero(int HeroId, int ChangeHeroId) 
    {
        Debug.Log("正在开发");
    }

    /// <summary>
    /// 获取用户现有灵魂数量
    /// </summary>
    /// <returns></returns>
    public static int GetSoulNumic() 
    {
        return 100;
    }

    /// <summary>
    /// 获取主页面用户基础信息
    /// </summary>
    /// <returns></returns>
    public static UserInfoClass GetUserInfo ()
    {
        return UnitInfoService.GetPageNumicInfo();
    }

    /// <summary>
    /// 获取军团数量
    /// </summary>
    /// <returns></returns>
    public static int GetLegionNum() 
    {
        return 100;
    }


    /// <summary>
    /// 初始化获取全部Unit数据
    /// </summary>
    /// <returns></returns>
    public static List<UnitViewClass> GetAllUnitInfo()
    {
        return UnitInfoService.GetAllUnitInfo();
    }

    /// <summary>
    /// 根据UnitId获取Unit基础信息
    /// </summary>
    /// <param name="UnitId"></param>
    /// <returns></returns>
    public static UnitBasicClass GetUnitBasicInfoByUnitId(int UnitId) 
    {
        return UnitInfoService.GetUnitBasicInfoByUnitId(UnitId);
    }

    /// <summary>
    /// 根据UnitId获取Unit数值信息
    /// </summary>
    /// <param name="UnitId"></param>
    /// <returns></returns>
    public static UnitAttackInfoClass GetUnitAttackInfoByUnitId(int UnitId) 
    {
        return UnitInfoService.GetUnitAttackInfoByUnitId(UnitId);
    }

    // TODO 与策划商议升级数值信息的方式
    /// <summary>
    /// 根据UnitId更新Unit数据
    /// </summary>
    /// <param name="UnitId"></param>
    /// <param name="UpdateInfo"></param>
    /*public static void SetUnitAttackInfoByUnitId(int UnitId, UnitAttackInfoClass UpdateInfo) 
    {
        Debug.Log("正在开发");
    }*/

    /// <summary>
    /// 根据UnitId获取Unit基础信息 UnitTemplate
    /// </summary>
    /// <param name="UnitId"></param>
    /// <returns></returns>
    public static UnitTemplate GetUnitTemplateByUnitId(int UnitId)
    {
        return UnitInfoService.GetUnitTemplateByUnitId(UnitId);
    }

    /// <summary>
    /// 初始化获得全部状态机数据
    /// </summary>
    /// <returns></returns>
    public static List<UnitTemplate> GetAllUnitTemplate()
    {
        return UnitInfoService.GetAllUnitTemplate();
    }
}
