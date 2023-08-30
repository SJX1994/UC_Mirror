using Common;
using System.Collections.Generic;
using UnityEngine;
using static UIManager;

public class UnitInfoService : SingTon<UnitInfoService>
{
    public static List<UnitViewClass> GetBeConfigUnit() 
    {
        List<UnitViewClass> unitViewClasses= new List<UnitViewClass>();

        var AllHeroInfo = HeroInfoConfigCategory.Instance.GetAll();

        int sum = 0;

        foreach (int key in AllHeroInfo.Keys) 
        {
            UnitViewClass unitViewClass = new UnitViewClass();

            var info = AllHeroInfo[key];

            unitViewClass.ConfigId = info.ConfigID;

            unitViewClass.TopConfig = sum;

            sum++;

            unitViewClass.LockType = false;

            UnitBasicClass unitBasicClass = GetUnitBasicInfoByUnitId(key);

            unitViewClass.BasicInfo = unitBasicClass;

            UnitAttackInfoClass UnitAttackInfoClass = GetUnitAttackInfoByUnitId(key);

            unitViewClass.AttackInfo = UnitAttackInfoClass;

            unitViewClass.ExcelTemp = info;

            unitViewClasses.Add(unitViewClass);
        }

        return unitViewClasses;
        
    }

    /// <summary>
    /// 获取页面数据
    /// </summary>
    /// <returns></returns>
    public static UserInfoClass GetPageNumicInfo() {

        UserInfoClass userInfoClass = new UserInfoClass();

        userInfoClass.FastTime = "17 : 50";

        userInfoClass.Hero = 8;

        userInfoClass.Dead = 503;

        return userInfoClass;
    }

    /// <summary>
    /// 根据UnitId获取Unit基础信息
    /// </summary>
    /// <param name="UnitId"></param>
    /// <returns></returns>
    public static UnitBasicClass GetUnitBasicInfoByUnitId(int UnitId) 
    {
        UnitBasicClass unitBasicClass = new UnitBasicClass();

        var AllHeroInfo = HeroInfoConfigCategory.Instance.GetAll();

        //TODO delete Mock Data
        unitBasicClass.UnitId = UnitId;

        string heropic = "Hero_Ultimate";

        if (UnitId == 1000)
        {
            heropic = "Hero_Bao";
        }
        else if (UnitId == 1001)
        {
            heropic = "Hero_Zhao";
        }
        else if (UnitId == 1002)
        {
            heropic = "Hero_Week";
        }
        else if (UnitId == 1003)
        {
            heropic = "Hero_Ultimate";
        }

        unitBasicClass.CharacterStyle = ABManager.Instance.LoadResource<GameObject>("gameobj", heropic);

        unitBasicClass.CharacterEffect = ABManager.Instance.LoadResource<GameObject>("gameobj", "Ulimate_BgEffect");

        unitBasicClass.Age = "10";

        unitBasicClass.gender = EventType.Gender.Male;

        unitBasicClass.Hobby = new string[] { "唱歌", "跳舞"};

        unitBasicClass.Label = new string[] { "盾兵", "善战者" };

        unitBasicClass.BackGroundStory = "角色背景故事还没配置";

        var dic = new Dictionary<int, string>() { };

        dic.Add(1001, "他们对爆爆有点崇敬");

        unitBasicClass.NetWork = dic;

        unitBasicClass.UnitName = UnitId + "角色名称";

        unitBasicClass.unitcamp = EventType.UnitCamp.Cell;

        List<SkillViewClass> SkillViewClassList = new();

        SkillViewClass skillViewClass = GetSkillViewInfoBySkillId(1001);

        SkillViewClassList.Add(skillViewClass);

        unitBasicClass.SkillViewInfo = SkillViewClassList;

        unitBasicClass.excelTemp = AllHeroInfo.ContainsKey(UnitId)? AllHeroInfo[UnitId]: null;

        return unitBasicClass;
    }

    /// <summary>
    /// 根据UnitId获取Unit数值信息
    /// </summary>
    /// <param name="UnitId"></param>
    /// <returns></returns>
    public static UnitAttackInfoClass GetUnitAttackInfoByUnitId(int UnitId) 
    {
        UnitAttackInfoClass unitAttackInfoClass = new UnitAttackInfoClass();

        unitAttackInfoClass.UnitId = UnitId;

        unitAttackInfoClass.Health = 50;

        unitAttackInfoClass.Aggressivity = 5;

        unitAttackInfoClass.Speed = 10;

        unitAttackInfoClass.AttackRange = 1f;

        unitAttackInfoClass.GuardDistance = 5f;

        unitAttackInfoClass.SkillId = new int[] { 1001, 1002};

        return unitAttackInfoClass;
    }

    /// <summary>
    /// 根据UnitId获取Unit基础信息 UnitTemplate
    /// </summary>
    /// <param name="UnitId"></param>
    /// <returns></returns>
    public static UnitTemplate GetUnitTemplateByUnitId(int UnitId) 
    {
        UnitTemplate unitTemplate = new UnitTemplate();

        unitTemplate.unitType = UnitTemplate.UnitType.Cell;

        unitTemplate.health = 10;

        unitTemplate.attackPower = 2;

        unitTemplate.attackSpeed = 1f;

        unitTemplate.engageDistance = 1f;

        unitTemplate.guardDistance = 5f;

        return unitTemplate;
    }

    /// <summary>
    /// 根据技能Id获取技能基础信息
    /// </summary>
    /// <param name="SkillId"></param>
    /// <returns></returns>
    public static SkillViewClass GetSkillViewInfoBySkillId(int SkillId) 
    {
        SkillViewClass skillViewClass = new SkillViewClass();

        skillViewClass.SkillId = 1001;

        skillViewClass.SkillIcon = ABManager.Instance.LoadResource<Sprite>("spritepreferb", "SkillIcon_0");

        skillViewClass.SkillName = "同归于尽";

        skillViewClass.SkillInfo = "你要抱着这个心态去跟外尔们拼命，但不是要你真的跟他们同归于尽";

        skillViewClass.SkillLevel = 1;

        skillViewClass.SkillNumeric = 0.1f;

        return skillViewClass;
    }

    /// <summary>
    /// 根据技能Id获取技能数值信息
    /// </summary>
    /// <param name="SkillId"></param>
    /// <returns></returns>
    public static SkillActionClass GetSkillActionInfoBySkillId(int SkillId) 
    {
        SkillActionClass SkillActionClass = new SkillActionClass();

        return SkillActionClass;
    }

    /// <summary>
    /// 初始化获取全部Unit数据
    /// </summary>
    /// <returns></returns>
    public static List<UnitViewClass> GetAllUnitInfo()
    {
        List<UnitViewClass> unitViewClasses = new List<UnitViewClass>();

        for (int i = 0; i < 7; i++)
        {
            UnitViewClass unitViewClass = new UnitViewClass();

            unitViewClass.ConfigId = i;

            unitViewClass.TopConfig = i;

            unitViewClass.LockType = false;

            UnitBasicClass UnitBasicClass = new UnitBasicClass();

            unitViewClass.BasicInfo = UnitBasicClass;

            UnitAttackInfoClass UnitAttackInfoClass = new UnitAttackInfoClass();

            unitViewClass.AttackInfo = UnitAttackInfoClass;

            unitViewClasses.Add(unitViewClass);
        }

        return unitViewClasses;
    }

    /// <summary>
    /// 初始化获得全部状态机数据
    /// </summary>
    /// <returns></returns>
    public static List<UnitTemplate> GetAllUnitTemplate() 
    {
        List<UnitTemplate> UnitTemplateList = new();

        UnitTemplate UnitTemplate = new UnitTemplate();

        UnitTemplateList.Add(UnitTemplate);

        return UnitTemplateList;
    }
}
