using GameFrameWork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static HeroSelect;

public class HomePage2D : BaseUI
{
    /// <summary>
    /// 首页上的2D交互UI
    /// </summary>
    /// <returns></returns>
    public override UIType GetUIType()
    {
        return UIType.HomePage2D;
    }

    #region 交互组件
    [Header("按钮组件")]
    public Button StartGameBtn;//开始游戏
    public Button HeroToLeftBtn;//向左滑动按钮
    public Button HeroToRightBtn;//向右滑动按钮
    public Button UserBtn;//用户界面按钮
    public Button CoinsBtn;//金币获取按钮
    public Button ExperienceBtn;//经验获取按钮
    public Button LanNetButton; //局域网对战按钮
    public Button StartMatchingButton;//开始匹配按钮

    [Space(20)]
    [Header("英雄滑动组件")]
    public HeroSelect heroSelect;//英雄滑动组件

    public HeroSelect.ItemInfo[] itemInfos;

    #endregion

    #region 业务逻辑

    public override void OnStart()
    {
        #region 相机逻辑
        //这里需要把UI设定为3DUI
        Camera uiCam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        Canvas canvas = this.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = uiCam;
        canvas.sortingOrder = 21;
        canvas.planeDistance = 5;
        #endregion

        //绑定点击监听事件
        BtnEvent.RigisterButtonClickEvent(StartGameBtn.transform.gameObject, p => { StartGame(); });
        BtnEvent.RigisterButtonClickEvent(HeroToLeftBtn.transform.gameObject, p => { LeftBtn(); });
        BtnEvent.RigisterButtonClickEvent(HeroToRightBtn.transform.gameObject, p => { RightBtn(); });
        BtnEvent.RigisterButtonClickEvent(UserBtn.transform.gameObject, p => { });
        BtnEvent.RigisterButtonClickEvent(CoinsBtn.transform.gameObject, p => { });
        BtnEvent.RigisterButtonClickEvent(ExperienceBtn.transform.gameObject, p => { });
        BtnEvent.RigisterButtonClickEvent(LanNetButton.transform.gameObject, p => { ChooseLanNet(); });
        BtnEvent.RigisterButtonClickEvent(StartMatchingButton.transform.gameObject, p => { StartMatching(); });

    }

    //加载英雄列表
    private void LoadHeroItems()
    {
        if (heroSelect != null)
        {
            int num = itemInfos.Length;
            string[] names = new string[num];
            GameObject[] heros = new GameObject[num];
            int[] heroIndexs = new int[num];
            for (int i = 0; i < num; i++)
            {
                names[i] = itemInfos[i].name;
                heros[i] = itemInfos[i].hero;
                heroIndexs[i] = itemInfos[i].heroIndex;
            }
            heroSelect.SetItemsInfo(names, heros, heroIndexs);
            heroSelect.OnStart();
        }
        heroSelect.SelectAction += (index) =>
        {
            //要通过ID获取到英雄信息，返回一个Static class
            UnitBasicClass heroInfo = UnitInfoManager.GetUnitBasicInfoByUnitId(index);
            //打开英雄细节界面，将class传入新打开的界面
            object[] data = new object[] { heroInfo };
            UIManager.Instance.OpenUI(UIType.HeroDetail, data);
        };
    }
    #endregion

    #region 数据加载

    public override void OnLoadData(params object[] param)
    {

        //获取所有英雄，并且将英雄数据放入itemInfos里面去
        List<UnitViewClass> _beConfigHerosInfo = (List<UnitViewClass>)param[0];
        if (_beConfigHerosInfo.Count == 0)
        {
            Debug.Log("首页英雄配置未收集到任何数据！");
            return;
        }
        else
        {
            for (int i = 0; i < _beConfigHerosInfo.Count; i++)
            {
                if (_beConfigHerosInfo[i] == null) return;
            }
            itemInfos = new ItemInfo[_beConfigHerosInfo.Count];
            for (int i = 0; i < _beConfigHerosInfo.Count; i++)
            {
                itemInfos[i] = new ItemInfo(_beConfigHerosInfo[i].ExcelTemp.Name, _beConfigHerosInfo[i].BasicInfo.CharacterStyle, _beConfigHerosInfo[i].ConfigId);
            }
        }
        //itemInfos中的数据赋值到拖动列表中，并且建立唯一ID用于点击事件
        LoadHeroItems();
    }

    #endregion

    #region 监听事件

    void StartGame()
    {
        AudioSystemManager.Instance.PlaySound("Start_Game_Button");

        var info = GameObject.Find("LanNetWorkManager");

        info.GetComponent<MainSceneControlManager>().LoadMainFightScene();
    }

    void LeftBtn()
    {
        HeroSelect.instance.ToLeft();
    }

    void RightBtn()
    {
        HeroSelect.instance.ToRight();
    }

    void ChooseLanNet()
    {
        // UIManager.Instance.OpenUI(UIType.NetWork, null);
    }

    void StartMatching()
    {
        var CommunicationManager = GameObject.Find("LanNetWorkManager").gameObject.GetComponent<CommunicationInteractionManager>();

        CommunicationManager.C2G_StartMatching(0);

        StartMatchingButton.gameObject.SetActive(false);

    }
    #endregion
}
