using GameFrameWork;
using UnityEngine;
using UnityEngine.UI;
using static ResManager;

public class HeroStory : BaseUI
{
    public override UIType GetUIType()
    {
        return UIType.HeroStory;
    }

    #region �������

    public GameObject heroParent;
    public Button backBtn;
    public Text heroName;
    public Text heroStory;

    private UnitBasicClass heroInfo;
    private Animation anim;
    private GameObject hero;
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
        canvas.sortingOrder = 32;
        canvas.planeDistance = 3;
        #endregion

        //����¼���
        BtnEvent.RigisterButtonClickEvent(backBtn.transform.gameObject, p => { CloseHeroStory(); });

        anim = this.GetComponent<Animation>();
    }

    private void CloseHeroStory()
    {
        UIManager.Instance.CloseUI(UIType.HeroStory);
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
            SetInfo();
        }
    }

    private void SetInfo()
    {
        //Ӣ�۷���ҳ��
        hero = Instantiate(heroInfo.CharacterStyle, heroParent.transform);
        heroName.text = heroInfo.excelTemp.Name;
        heroStory.text = heroInfo.BackGroundStory;
    }

    #endregion
}
