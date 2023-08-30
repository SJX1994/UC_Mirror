using GameFrameWork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroDetail : BaseUI
{
    public override UIType GetUIType()
    {
        return UIType.HeroDetail;
    }

    #region 交互组件

    [Header("按钮")]
    public Button NextBtn;
    public Button SkillBtn;
    public Button HeroBtn;

    [Header("英雄信息")]
    public GameObject HeroParent;
    public Text HeroName;
    public GameObject TagParent;

    [Header("基本信息")]
    public Text AggressivityValue;
    public Text HealthValue;
    public Text AttackSpeedValue;
    public Text AttackRangeValue;

    [Header("技能信息")]
    public Image SkillImg;
    public Text SkillName;
    public Text SkillLevel;
    public Text SkillInfo;
    public Text NextLevelInfo;

    [Header("特效")]
    public GameObject effectParent;

    private UnitBasicClass heroInfo;
    private GameObject hero;
    private Animation anim;

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
        canvas.sortingOrder = 30;
        canvas.planeDistance = 4;
        #endregion

        //点击事件绑定
        BtnEvent.RigisterButtonClickEvent(NextBtn.transform.gameObject, p => { CloseDetailUi(); });
        BtnEvent.RigisterButtonClickEvent(SkillBtn.transform.gameObject, p => { });
        BtnEvent.RigisterButtonClickEvent(HeroBtn.transform.gameObject, p => { OpenHeroStory(); });
        
        anim = this.GetComponent<Animation>();
        anim.Play("A_HeroDetailOpen");
    }

    #endregion

    #region 数据加载
    public override void OnLoadData(params object[] param)
    {
        heroInfo = (UnitBasicClass)param[0];
        if (heroInfo == null)
        {
            Debug.Log("丢失了英雄信息！");
            return;
        }
        else
        {
            SetHeroBasic();
            SetSkilBasic();
        }
    }

    /// <summary>
    /// 设置英雄基本信息
    /// </summary>
    void SetHeroBasic()
    {
        //英雄放入页面
        hero = Instantiate(heroInfo.CharacterStyle, HeroParent.transform);
        //设置英雄名
        HeroName.text = heroInfo.excelTemp.Name;
        //添加标签
        for (int i = 0; i < heroInfo.Label.Length; i++)
        {
            string tempTag = heroInfo.Label[i];
            object[] data = new object[] { tempTag };
            UIManager.Instance.OpenSubUI(UIType.SubHeroTag, data, TagParent);
        }
        //设置基本属性值
        AggressivityValue.text = heroInfo.excelTemp.Aggressivity.ToString();
        HealthValue.text = heroInfo.excelTemp.Health.ToString();
        AttackSpeedValue.text = heroInfo.excelTemp.Attack_Speed.ToString();
        AttackRangeValue.text = heroInfo.excelTemp.Attack_Range.ToString();
    }

    /// <summary>
    /// 设置英雄技能信息
    /// </summary>
    void SetSkilBasic()
    {

    }

    #endregion

    #region 事件监听
    void CloseDetailUi()
    {
        UIManager.Instance.CloseUI(UIType.HeroDetail);
    }

    void OpenHeroStory()
    {
        if (heroInfo != null)
        {
            object[] data = new object[] { heroInfo };
            UIManager.Instance.OpenUI(UIType.HeroStory, data);
        }
        else
        {
            return;
        }
    }
    #endregion
}
