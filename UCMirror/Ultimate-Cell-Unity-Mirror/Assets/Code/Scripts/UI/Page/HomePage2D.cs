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
    /// ��ҳ�ϵ�2D����UI
    /// </summary>
    /// <returns></returns>
    public override UIType GetUIType()
    {
        return UIType.HomePage2D;
    }

    #region �������
    [Header("��ť���")]
    public Button StartGameBtn;//��ʼ��Ϸ
    public Button HeroToLeftBtn;//���󻬶���ť
    public Button HeroToRightBtn;//���һ�����ť
    public Button UserBtn;//�û����水ť
    public Button CoinsBtn;//��һ�ȡ��ť
    public Button ExperienceBtn;//�����ȡ��ť
    public Button LanNetButton; //��������ս��ť
    public Button StartMatchingButton;//��ʼƥ�䰴ť

    [Space(20)]
    [Header("Ӣ�ۻ������")]
    public HeroSelect heroSelect;//Ӣ�ۻ������

    public HeroSelect.ItemInfo[] itemInfos;

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
        canvas.sortingOrder = 21;
        canvas.planeDistance = 5;
        #endregion

        //�󶨵�������¼�
        BtnEvent.RigisterButtonClickEvent(StartGameBtn.transform.gameObject, p => { StartGame(); });
        BtnEvent.RigisterButtonClickEvent(HeroToLeftBtn.transform.gameObject, p => { LeftBtn(); });
        BtnEvent.RigisterButtonClickEvent(HeroToRightBtn.transform.gameObject, p => { RightBtn(); });
        BtnEvent.RigisterButtonClickEvent(UserBtn.transform.gameObject, p => { });
        BtnEvent.RigisterButtonClickEvent(CoinsBtn.transform.gameObject, p => { });
        BtnEvent.RigisterButtonClickEvent(ExperienceBtn.transform.gameObject, p => { });
        BtnEvent.RigisterButtonClickEvent(LanNetButton.transform.gameObject, p => { ChooseLanNet(); });
        BtnEvent.RigisterButtonClickEvent(StartMatchingButton.transform.gameObject, p => { StartMatching(); });

    }

    //����Ӣ���б�
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
            //Ҫͨ��ID��ȡ��Ӣ����Ϣ������һ��Static class
            UnitBasicClass heroInfo = UnitInfoManager.GetUnitBasicInfoByUnitId(index);
            //��Ӣ��ϸ�ڽ��棬��class�����´򿪵Ľ���
            object[] data = new object[] { heroInfo };
            UIManager.Instance.OpenUI(UIType.HeroDetail, data);
        };
    }
    #endregion

    #region ���ݼ���

    public override void OnLoadData(params object[] param)
    {

        //��ȡ����Ӣ�ۣ����ҽ�Ӣ�����ݷ���itemInfos����ȥ
        List<UnitViewClass> _beConfigHerosInfo = (List<UnitViewClass>)param[0];
        if (_beConfigHerosInfo.Count == 0)
        {
            Debug.Log("��ҳӢ������δ�ռ����κ����ݣ�");
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
        //itemInfos�е����ݸ�ֵ���϶��б��У����ҽ���ΨһID���ڵ���¼�
        LoadHeroItems();
    }

    #endregion

    #region �����¼�

    void StartGame()
    {
        AudioSystemManager.Instance.PlaySound("Start_Game_Button");

        var info = GameObject.Find("LanNetWorkManager");

        // info.GetComponent<MainSceneControlManager>().LoadMainFightScene();
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
