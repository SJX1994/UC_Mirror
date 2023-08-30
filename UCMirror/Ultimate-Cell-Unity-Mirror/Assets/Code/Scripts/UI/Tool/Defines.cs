/// **************************************
/// 全局常量类
/// **************************************

namespace GameFrameWork
{
    //UI状态委托
    public delegate void UIStateDelegate(object obj,UIState OldState,UIState NewState);

    /// <summary>
    /// 这里存放所有UI界面枚举
    /// </summary>
    public enum UIType
    {
        //测试
        None = -1,

        //首页
        HomePage2D = 0,
        HomePage3D = 1,

        //首页子页面
        HeroDetail = 2,//英雄详情页面
        HeroStory = 3,

        SubHeroItem = 4,
        SubConfigItem = 5,
        HeroUpgradePage = 6,
        SubHeroTag = 7,

        //英雄
        Hero_Ultimate = 8,

        //战斗
        FightUi = 20,
        LegionUi = 21,
        IdelBox = 22,
        IdelUi = 23,
        StarItem = 24,
        VictoryPage = 25,
        FailPage = 26,

    }

    /// <summary>
    /// UI页面状态
    /// 初始化
    /// 加载
    /// 准备完毕
    /// 关闭
    /// 销毁
    /// </summary>
    public enum UIState
    {
        None,
        Init,
        Loading,
        Ready,
        Disable,
        Closing,
    }

    /// <summary>
    /// 数据加载
    /// 这里用于维护所有UI路径
    /// 包括加载路径
    /// 和返回路径
    /// </summary>
    public class PathDefines
    {
        //加载路径
        public const string UI_PREFABS = "Prefabs/UIPage/";
        public const string UI_ICON = "Prefabs/UIPage/Icon/";
        public const string UI_SUBUI = "Prefabs/UIPage/SubUI/";
        public const string UI_HERO = "Prefabs/UIPage/Hero/";

        /// <summary>
        /// 返回一个路径
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public static string GetUIPathWithUIType(UIType _type)
        {
            string path = string.Empty;
            switch (_type)
            {
                //主页
                case UIType.HomePage2D:
                    path = "HomePage2D";
                    break;
                //英雄配置页
                case UIType.HomePage3D:
                    path = "HomePage3D";
                    break;
                //关卡选择页
                case UIType.HeroDetail:
                    path = "HeroDetail";
                    break;
                //（二级）关卡细节页面
                case UIType.HeroStory:
                    path = "HeroStory";
                    break;
                //（二级）英雄对象页
                case UIType.SubHeroItem:
                    path = "SubHeroItem";
                    break;
                //（二级）配置英雄对象页
                case UIType.SubConfigItem:
                    path = "SubConfigItem";
                    break;
                //英雄升级页
                case UIType.HeroUpgradePage:
                    path = "HeroUpgradePage";
                    break;
                case UIType.SubHeroTag:
                    path = "SubHeroTag";
                    break;

                //英雄UI
                case UIType.Hero_Ultimate:
                    path = "Hero_Ultimate";
                    break;

                //战斗UI
                case UIType.FightUi:
                    path = "FightUi";
                    break;
                case UIType.LegionUi:
                    path = "LegionUi";
                    break;
                case UIType.IdelBox:
                    path = "IdelBox";
                    break;
                case UIType.IdelUi:
                    path = "IdelUi";
                    break;
                case UIType.StarItem:
                    path = "Star";
                    break;
                case UIType.VictoryPage:
                    path = "VictoryPage";
                    break;
                case UIType.FailPage:
                    path = "FailPage";
                    break;

                default:
                    //null
                    break;
            }
            return path;
        }
        
        /// <summary>
        /// 返回一个脚本类型
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public static System.Type GetScriptsWithUIType(UIType _type)
        {
            System.Type _stype = null;
            switch (_type)
            {
                case UIType.HomePage2D:
                    _stype = typeof(HomePage2D);
                    break;
                case UIType.HomePage3D:
                    _stype = typeof(HomePage3D);
                    break;
                case UIType.HeroDetail:
                    _stype = typeof(HeroDetail);
                    break;
                case UIType.HeroStory:
                    _stype = typeof(HeroStory);
                    break;
                case UIType.SubHeroItem:
                    _stype = typeof(SubHeroItem);
                    break;
                case UIType.SubConfigItem:
                    _stype = typeof(SubConfigItem);
                    break;
                case UIType.HeroUpgradePage:
                    _stype = typeof(HeroUpgradePage);
                    break;
                case UIType.SubHeroTag:
                    _stype = typeof(SubHeroTag);
                    break;

                //英雄UI
                case UIType.Hero_Ultimate:
                    _stype = typeof(Hero_Ultimate);
                    break;

                //战斗UI
                case UIType.FightUi:
                    _stype = typeof(FightUi);
                    break;
                case UIType.LegionUi:
                    _stype = typeof(LegionUi);
                    break;
                case UIType.IdelBox:
                    _stype = typeof(IdelBox);
                    break;
                case UIType.IdelUi:
                    _stype = typeof(IdelUI);
                    break;
                case UIType.StarItem:
                    _stype = typeof(StarItem);
                    break;
                case UIType.VictoryPage:
                    _stype = typeof(VictoryPage);
                    break;
                case UIType.FailPage:
                    _stype = typeof(FailPage);
                    break;

                default:
                    //null
                    break;
            }
            return _stype;
        }
    }

    public class Defines
    {

    }
}
