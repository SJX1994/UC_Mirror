/// **************************************
/// ????????
/// **************************************

namespace GameFrameWork
{
    //UI?????
    public delegate void UIStateDelegate(object obj,UIState OldState,UIState NewState);

    /// <summary>
    /// ??????????UI???????
    /// </summary>
    public enum UIType
    {
        //????
        None = -1,

        //???
        HomePage2D = 0,
        HomePage3D = 1,

        //????????
        HeroDetail = 2,//??????????
        HeroStory = 3,

        SubHeroItem = 4,
        SubConfigItem = 5,
        HeroUpgradePage = 6,
        SubHeroTag = 7,

        //???
        Hero_Ultimate = 8,

        //???
        FightUi = 20,
        LegionUi = 21,
        IdelBox = 22,
        IdelUi = 23,
        StarItem = 24,
        VictoryPage = 25,
        FailPage = 26,

    }

    /// <summary>
    /// UI?????
    /// ?????
    /// ????
    /// ??????
    /// ???
    /// ????
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
    /// ???????
    /// ???????????????UI，??
    /// ????????，??
    /// ?????，??
    /// </summary>
    public class PathDefines
    {
        //????，??
        public const string UI_PREFABS = "Prefabs/UIPage/";
        public const string UI_ICON = "Prefabs/UIPage/Icon/";
        public const string UI_SUBUI = "Prefabs/UIPage/SubUI/";
        public const string UI_HERO = "Prefabs/UIPage/Hero/";

        /// <summary>
        /// ???????，??
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public static string GetUIPathWithUIType(UIType _type)
        {
            string path = string.Empty;
            switch (_type)
            {
                //???
                case UIType.HomePage2D:
                    path = "HomePage2D";
                    break;
                //????????
                case UIType.HomePage3D:
                    path = "HomePage3D";
                    break;
                //???????
                case UIType.HeroDetail:
                    path = "HeroDetail";
                    break;
                //?????????????????
                case UIType.HeroStory:
                    path = "HeroStory";
                    break;
                //???????????????
                case UIType.SubHeroItem:
                    path = "SubHeroItem";
                    break;
                //???????????????????
                case UIType.SubConfigItem:
                    path = "SubConfigItem";
                    break;
                //????????
                case UIType.HeroUpgradePage:
                    path = "HeroUpgradePage";
                    break;
                case UIType.SubHeroTag:
                    path = "SubHeroTag";
                    break;

                //???UI
                case UIType.Hero_Ultimate:
                    path = "Hero_Ultimate";
                    break;

                //???UI
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
                    path = "UI/Star";
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
        /// ??????????????
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

                //???UI
                case UIType.Hero_Ultimate:
                    _stype = typeof(Hero_Ultimate);
                    break;

                //???UI
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
