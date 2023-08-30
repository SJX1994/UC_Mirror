using UnityEngine;
using UnityEngine.UI;
using GameFrameWork;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// 首页
/// 用于继承BaseUI实现的抽象类
/// </summary>

public class HomePage : BaseUI
{
    public override UIType GetUIType()
    {
        return UIType.None;
    }

    #region 页面交互组件
    public Button Btn_Publican;
    public Button Btn_Bookcase;
    public Button Btn_Hero0;
    public Button Btn_Hero1;
    public Button Btn_Hero2;
    public Button Btn_Hero3;
    public Button Btn_Legion;
    public Button Btn_Record;

    public TextMeshProUGUI UserInfo_0;
    public TextMeshProUGUI UserInfo_1;
    public TextMeshProUGUI UserInfo_2;
    #endregion

    #region PrivateValue
    UserInfoClass UserInfo;
    #endregion

    #region 业务逻辑
    public override void OnAwake()
    {
        //在Start里面进行
        #region 设置Canvas为屏幕
        //相机优先处理
        Camera uiCam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        Canvas canvas = this.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = uiCam;
        canvas.planeDistance = 10;
        canvas.sortingOrder = 19;
        #endregion

        //因为点击英雄配置界面是一起的，所以存一个按钮数组监听
        GameObject[] _heroBtn = new GameObject[] { Btn_Hero0.gameObject, Btn_Hero1 .gameObject, Btn_Hero2 .gameObject, Btn_Hero3.gameObject};

        #region 点击监听事件
        //进入地图关卡选择界面
        BtnEvent.RigisterButtonClickEvent(Btn_Publican.transform.gameObject, p =>{onPublicanClick();});

        //进入英雄配置界面
        BtnEvent.RigisterButtonClickEvent(_heroBtn, p =>{OnHeroClick();});
        #endregion

        #region 浮动监听事件
        ///*****************************
        ///英雄浮动监听
        ///*****************************
        BtnEvent.RigisterButtonEnterEvent(Btn_Hero0.transform.gameObject, p =>
        {
            OnHero0Enter();
        });
        BtnEvent.RigisterButtonExitEvent(Btn_Hero0.transform.gameObject, p =>
        {
            OnHero0Exit();
        });
        BtnEvent.RigisterButtonEnterEvent(Btn_Hero1.transform.gameObject, p =>
        {
            OnHero1Enter();
        });
        BtnEvent.RigisterButtonExitEvent(Btn_Hero1.transform.gameObject, p =>
        {
            OnHero1Exit();
        });
        BtnEvent.RigisterButtonEnterEvent(Btn_Hero2.transform.gameObject, p =>
        {
            OnHero2Enter();
        });
        BtnEvent.RigisterButtonExitEvent(Btn_Hero2.transform.gameObject, p =>
        {
            OnHero2Exit();
        });
        BtnEvent.RigisterButtonEnterEvent(Btn_Hero3.transform.gameObject, p =>
        {
            OnHero3Enter();
        });
        BtnEvent.RigisterButtonExitEvent(Btn_Hero3.transform.gameObject, p =>
        {
            OnHero3Exit();
        });

        ///*****************************
        ///老板浮动监听
        ///*****************************
        BtnEvent.RigisterButtonEnterEvent(Btn_Publican.transform.gameObject, p =>
        {
            OnPublicanEnter();
        });
        BtnEvent.RigisterButtonExitEvent(Btn_Publican.transform.gameObject, p =>
        {
            OnPublicanExit();
        });
        #endregion
    }

    #endregion

    #region 数据加载
    public override void OnLoadData(params object[] param)
    {
        UserInfo = (UserInfoClass)param[4];
        if (UserInfo != null)
        {
            UserInfo_0.text = UserInfo.FastTime;
            UserInfo_1.text = UserInfo.Dead.ToString();
            UserInfo_2.text = UserInfo.Hero.ToString();
            //判断组件是否已经Set了数据
        }
        else
        {
            throw new System.Exception("首页初始化时候没有获取用户数据！");
        }
        base.OnLoadData(param);
    }
    #endregion

    #region 事件监听

    #region 点击事件
    void onPublicanClick()
    {
        Debug.Log("地图关卡界面！");
        //加载开始游戏界面
        UIManager.Instance.OpenUICloseOtherUI(UIType.None, null);
    }

    void OnHeroClick()
    {
        Debug.Log("配置英雄牌组界面！");
        UIManager.Instance.OpenUICloseOtherUI(UIType.None, null);
        this.gameObject.SetActive(false);
    }


    #endregion

    #region 浮动事件
    /// <summary>
    /// 英雄的浮动监听事件
    /// </summary>
    void OnHero0Enter()
    {
        GameObject _hero0 = GameObject.Find("CharacterPoint/HeroPoint_0");
        List<SpriteRenderer> spriteRenderers = UIPerformance.GetSpriteRenderer(_hero0);
        UIPerformance.ButtonEnterEffect(spriteRenderers);
    }

    void OnHero0Exit()
    {
        GameObject _hero0 = GameObject.Find("CharacterPoint/HeroPoint_0");
        List<SpriteRenderer> spriteRenderers = UIPerformance.GetSpriteRenderer(_hero0);
        UIPerformance.ButtonExitEffect(spriteRenderers);
    }

    void OnHero1Enter()
    {
        GameObject _hero1 = GameObject.Find("CharacterPoint/HeroPoint_1");
        List<SpriteRenderer> spriteRenderers = UIPerformance.GetSpriteRenderer(_hero1);
        UIPerformance.ButtonEnterEffect(spriteRenderers);
    }

    void OnHero1Exit()
    {
        GameObject _hero1 = GameObject.Find("CharacterPoint/HeroPoint_1");
        List<SpriteRenderer> spriteRenderers = UIPerformance.GetSpriteRenderer(_hero1);
        UIPerformance.ButtonExitEffect(spriteRenderers);
    }

    void OnHero2Enter()
    {
        GameObject _hero2 = GameObject.Find("CharacterPoint/HeroPoint_2");
        List<SpriteRenderer> spriteRenderers = UIPerformance.GetSpriteRenderer(_hero2);
        UIPerformance.ButtonEnterEffect(spriteRenderers);
    }

    void OnHero2Exit()
    {
        GameObject _hero2 = GameObject.Find("CharacterPoint/HeroPoint_2");
        List<SpriteRenderer> spriteRenderers = UIPerformance.GetSpriteRenderer(_hero2);
        UIPerformance.ButtonExitEffect(spriteRenderers);
    }

    void OnHero3Enter()
    {
        GameObject _hero3 = GameObject.Find("CharacterPoint/HeroPoint_3");
        List<SpriteRenderer> spriteRenderers = UIPerformance.GetSpriteRenderer(_hero3);
        UIPerformance.ButtonEnterEffect(spriteRenderers);
    }

    void OnHero3Exit()
    {
        GameObject _hero3 = GameObject.Find("CharacterPoint/HeroPoint_3");
        List<SpriteRenderer> spriteRenderers = UIPerformance.GetSpriteRenderer(_hero3);
        UIPerformance.ButtonExitEffect(spriteRenderers);
    }

    void OnPublicanEnter()
    {
        GameObject _publican = GameObject.Find("CharacterPoint/Publican");
        List<SpriteRenderer> spriteRenderers = UIPerformance.GetSpriteRenderer(_publican);
        UIPerformance.ButtonEnterEffect(spriteRenderers);
    }

    void OnPublicanExit()
    {
        GameObject _publican = GameObject.Find("CharacterPoint/Publican");
        List<SpriteRenderer> spriteRenderers = UIPerformance.GetSpriteRenderer(_publican);
        UIPerformance.ButtonExitEffect(spriteRenderers);
    }
    #endregion

    #endregion
}
