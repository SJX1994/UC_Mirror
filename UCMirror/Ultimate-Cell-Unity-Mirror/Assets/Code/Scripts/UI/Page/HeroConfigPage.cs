using GameFrameWork;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeroConfigPage : BaseUI
{
    /// <summary>
    /// Ӣ������ҳ��
    /// ���ڼ̳�BaseUIʵ�ֵĳ�����
    /// </summary>
    /// <returns></returns>
    public override UIType GetUIType()
    {
        return UIType.None;
    }

    #region ҳ�潻�����
    public Button Btn_Back;
    public Button Btn_Determine;
    public GameObject HeroItemBox;
    public GameObject ConfigItemBox;
    #endregion

    #region ҵ���߼�
    public override void OnAwake()
    {
        #region ����CanvasΪ��Ļ
        //������ȴ���
        Camera uiCam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        Canvas canvas = this.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = uiCam;
        canvas.planeDistance = 10;
        canvas.sortingOrder = 20;
        #endregion

        //��ʼ��������ť�߼�
        BtnEvent.RigisterButtonClickEvent(Btn_Back.transform.gameObject, p =>{OnBackClick();});
        BtnEvent.RigisterButtonClickEvent(Btn_Determine.transform.gameObject, p =>{OnDetermineClick();});

        //��ʼ��������Ӣ��
        //��ȡ�Ѿ������õ�Ӣ��
        object[] isConfigHero = ResManager.GetIsConfigHero();
        //�������õ�Ӣ������Item�����б���
        for(int i = 0; i < isConfigHero.Length; i++)
        {
            UIManager.Instance.OpenSubUI(UIType.SubHeroItem, (object[])isConfigHero[i], HeroItemBox);
        }

        //��ʼ������Ӣ��
        //��ȡ����Ӣ�۵���Ϣ
        object[] heros = ResManager.GetAllHero();
        //������Ӣ��һ��һ������Item�����б���
        for( int i = 0; i < heros.Length; i++)
        {
            object[] _heros = new object[1];
            _heros[0] = heros[i];
            UIManager.Instance.OpenSubUI(UIType.SubConfigItem , _heros , ConfigItemBox);
        }

    }
    #endregion

    #region �¼�����

    #region ����¼�
    void OnBackClick()
    {
        Debug.Log("�������ã��ص���ҳ��");
        //�������ûص���ҳ��
        //GameObject HeroConfigPage = Instantiate(Resources.Load<GameObject>("Prefabs/UIPage/HomePage"));
        //Destroy();
        UIManager.Instance.OpenUICloseOtherUI(UIType.HomePage2D, null);
    }

    void OnDetermineClick()
    {
        Debug.Log("�������ã��ص���ҳ��");
        //�������ûص���ҳ��
        //GameObject HeroConfigPage = Instantiate(Resources.Load<GameObject>("Prefabs/UIPage/HomePage"));
        //Destroy();
        //BeConfig��Ӣ��Set���˾�
        //SetAllHeroInFo
        UIManager.Instance.OpenUICloseOtherUI(UIType.HomePage2D, null);
    }
    #endregion

    #region �����¼�


    #endregion

    #endregion

}
