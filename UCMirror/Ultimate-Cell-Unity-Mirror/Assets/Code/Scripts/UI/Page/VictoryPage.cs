using GameFrameWork;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class VictoryPage : BaseUI
{
    public override UIType GetUIType()
    {
        return UIType.VictoryPage;
    }

    #region �������

    [Header("����")]
    public GameObject Star1;
    public GameObject Star2;
    public GameObject Star3;

    [Header("��ť")]
    public Button NextMapBtn;
    public Button BackBtn;

    [Header("��ֵ")]
    public Text FractionText;
    public Text GoldText;
    public Text ExpText;

    [Header("��Ч")]
    public ParticleSystem VictoryEffect;

    private int fractio = 0;
    private int gold = 0;
    private int exp = 0;
    #endregion

    #region ҵ��

    public override void OnStart()
    {
        #region ����߼�
        //������Ҫ��UI�趨ΪScreenSpace
        Camera uiCam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        Canvas canvas = this.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = uiCam;
        canvas.sortingOrder = 200;
        canvas.planeDistance = 5;
        canvas.sortingLayerName = "Flow";
        #endregion

        BtnEvent.RigisterButtonClickEvent(NextMapBtn.transform.gameObject, p => { });
        BtnEvent.RigisterButtonClickEvent(BackBtn.transform.gameObject, p => { });

        int targetFractio = 302;
        DOTween.To(() => fractio, x => fractio = x, targetFractio, 2);

        int targetGold = 199;
        DOTween.To(() => gold, x => gold = x, targetGold, 2);

        int targetExp = 32;
        DOTween.To(() => exp, x => exp = x, targetExp, 2);

        Animation anim = GetComponent<Animation>();
        anim.Play("A_Victory");
        Invoke("PlayEffect", 1f);

        int starNum = 3;
        if(starNum == 3)
        {
            Invoke("Star1Load", 0.5f);
            Invoke("Star2Load", 0.7f);
            Invoke("Star3Load", 0.9f);
        }
    }

    public override void OnUpdate(float dealTime)
    {
        FractionText.text = fractio.ToString();
        GoldText.text = gold.ToString();
        ExpText.text = exp.ToString();

    }

    void PlayEffect()
    {
        VictoryEffect.Play();
    }

    void Star1Load()
    {
        UIManager.Instance.OpenSubUI(UIType.StarItem, null, Star1);
    }

    void Star2Load()
    {
        UIManager.Instance.OpenSubUI(UIType.StarItem, null, Star2);
    }

    void Star3Load()
    {
        UIManager.Instance.OpenSubUI(UIType.StarItem, null, Star3);
    }

    #endregion

}
