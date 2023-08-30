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

    #region �������

    [Header("��ť")]
    public Button NextBtn;
    public Button SkillBtn;
    public Button HeroBtn;

    [Header("Ӣ����Ϣ")]
    public GameObject HeroParent;
    public Text HeroName;
    public GameObject TagParent;

    [Header("������Ϣ")]
    public Text AggressivityValue;
    public Text HealthValue;
    public Text AttackSpeedValue;
    public Text AttackRangeValue;

    [Header("������Ϣ")]
    public Image SkillImg;
    public Text SkillName;
    public Text SkillLevel;
    public Text SkillInfo;
    public Text NextLevelInfo;

    [Header("��Ч")]
    public GameObject effectParent;

    private UnitBasicClass heroInfo;
    private GameObject hero;
    private Animation anim;

    #endregion

    #region ҵ���߼�
    public override void OnStart()
    {
        #region ����߼�
        //������Ҫ��UI�趨Ϊ3DUI
        Camera uiCam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        Canvas canvas = this.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = uiCam;
        canvas.sortingOrder = 30;
        canvas.planeDistance = 4;
        #endregion

        //����¼���
        BtnEvent.RigisterButtonClickEvent(NextBtn.transform.gameObject, p => { CloseDetailUi(); });
        BtnEvent.RigisterButtonClickEvent(SkillBtn.transform.gameObject, p => { });
        BtnEvent.RigisterButtonClickEvent(HeroBtn.transform.gameObject, p => { OpenHeroStory(); });
        
        anim = this.GetComponent<Animation>();
        anim.Play("A_HeroDetailOpen");
    }

    #endregion

    #region ���ݼ���
    public override void OnLoadData(params object[] param)
    {
        heroInfo = (UnitBasicClass)param[0];
        if (heroInfo == null)
        {
            Debug.Log("��ʧ��Ӣ����Ϣ��");
            return;
        }
        else
        {
            SetHeroBasic();
            SetSkilBasic();
        }
    }

    /// <summary>
    /// ����Ӣ�ۻ�����Ϣ
    /// </summary>
    void SetHeroBasic()
    {
        //Ӣ�۷���ҳ��
        hero = Instantiate(heroInfo.CharacterStyle, HeroParent.transform);
        //����Ӣ����
        HeroName.text = heroInfo.excelTemp.Name;
        //��ӱ�ǩ
        for (int i = 0; i < heroInfo.Label.Length; i++)
        {
            string tempTag = heroInfo.Label[i];
            object[] data = new object[] { tempTag };
            UIManager.Instance.OpenSubUI(UIType.SubHeroTag, data, TagParent);
        }
        //���û�������ֵ
        AggressivityValue.text = heroInfo.excelTemp.Aggressivity.ToString();
        HealthValue.text = heroInfo.excelTemp.Health.ToString();
        AttackSpeedValue.text = heroInfo.excelTemp.Attack_Speed.ToString();
        AttackRangeValue.text = heroInfo.excelTemp.Attack_Range.ToString();
    }

    /// <summary>
    /// ����Ӣ�ۼ�����Ϣ
    /// </summary>
    void SetSkilBasic()
    {

    }

    #endregion

    #region �¼�����
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
