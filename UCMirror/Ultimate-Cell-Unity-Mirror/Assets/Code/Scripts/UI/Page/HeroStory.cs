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

    #region 交互组件

    public GameObject heroParent;
    public Button backBtn;
    public Text heroName;
    public Text heroStory;

    private UnitBasicClass heroInfo;
    private Animation anim;
    private GameObject hero;
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
        canvas.sortingOrder = 32;
        canvas.planeDistance = 3;
        #endregion

        //点击事件绑定
        BtnEvent.RigisterButtonClickEvent(backBtn.transform.gameObject, p => { CloseHeroStory(); });

        anim = this.GetComponent<Animation>();
    }

    private void CloseHeroStory()
    {
        UIManager.Instance.CloseUI(UIType.HeroStory);
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
            SetInfo();
        }
    }

    private void SetInfo()
    {
        //英雄放入页面
        hero = Instantiate(heroInfo.CharacterStyle, heroParent.transform);
        heroName.text = heroInfo.excelTemp.Name;
        heroStory.text = heroInfo.BackGroundStory;
    }

    #endregion
}
