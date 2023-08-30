using GameFrameWork;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeroConfigPage : BaseUI
{
    /// <summary>
    /// 英雄配置页面
    /// 用于继承BaseUI实现的抽象类
    /// </summary>
    /// <returns></returns>
    public override UIType GetUIType()
    {
        return UIType.None;
    }

    #region 页面交互组件
    public Button Btn_Back;
    public Button Btn_Determine;
    public GameObject HeroItemBox;
    public GameObject ConfigItemBox;
    #endregion

    #region 业务逻辑
    public override void OnAwake()
    {
        #region 设置Canvas为屏幕
        //相机优先处理
        Camera uiCam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        Canvas canvas = this.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = uiCam;
        canvas.planeDistance = 10;
        canvas.sortingOrder = 20;
        #endregion

        //初始化基本按钮逻辑
        BtnEvent.RigisterButtonClickEvent(Btn_Back.transform.gameObject, p =>{OnBackClick();});
        BtnEvent.RigisterButtonClickEvent(Btn_Determine.transform.gameObject, p =>{OnDetermineClick();});

        //初始化已配置英雄
        //获取已经被配置的英雄
        object[] isConfigHero = ResManager.GetIsConfigHero();
        //将已配置的英雄生成Item放入列表中
        for(int i = 0; i < isConfigHero.Length; i++)
        {
            UIManager.Instance.OpenSubUI(UIType.SubHeroItem, (object[])isConfigHero[i], HeroItemBox);
        }

        //初始化所有英雄
        //获取所有英雄的信息
        object[] heros = ResManager.GetAllHero();
        //将所有英雄一个一个生成Item放入列表中
        for( int i = 0; i < heros.Length; i++)
        {
            object[] _heros = new object[1];
            _heros[0] = heros[i];
            UIManager.Instance.OpenSubUI(UIType.SubConfigItem , _heros , ConfigItemBox);
        }

    }
    #endregion

    #region 事件监听

    #region 点击事件
    void OnBackClick()
    {
        Debug.Log("放弃配置，回到主页面");
        //放弃配置回到主页面
        //GameObject HeroConfigPage = Instantiate(Resources.Load<GameObject>("Prefabs/UIPage/HomePage"));
        //Destroy();
        UIManager.Instance.OpenUICloseOtherUI(UIType.HomePage2D, null);
    }

    void OnDetermineClick()
    {
        Debug.Log("保存配置，回到主页面");
        //保存配置回到主页面
        //GameObject HeroConfigPage = Instantiate(Resources.Load<GameObject>("Prefabs/UIPage/HomePage"));
        //Destroy();
        //BeConfig的英雄Set给乃久
        //SetAllHeroInFo
        UIManager.Instance.OpenUICloseOtherUI(UIType.HomePage2D, null);
    }
    #endregion

    #region 浮动事件


    #endregion

    #endregion

}
