using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FailPage : BaseUI
{
    public override UIType GetUIType()
    {
        return UIType.None;
    }

    #region 交互组件

    [Header("按钮")]
    public Button againBtn;
    public Button backBtn;

    [Header("文本")]
    public Text infoText;

    [Header("特效")]
    public ParticleSystem failEffect;

    #endregion

    #region 业务逻辑

    public override void OnStart()
    {
        #region 相机逻辑
        //这里需要把UI设定为ScreenSpace
        Camera uiCam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        Canvas canvas = this.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = uiCam;
        canvas.sortingOrder = 200;
        canvas.planeDistance = 5;
        canvas.sortingLayerName = "Flow";
        #endregion

        Animation anim = GetComponent<Animation>();
        anim.Play("A_Fail");

        //按键绑定
        BtnEvent.RigisterButtonClickEvent(againBtn.transform.gameObject, p => { ClickAgainBtn(); });
        BtnEvent.RigisterButtonClickEvent(backBtn.transform.gameObject, p => { ClickBackBtn(); });

        Invoke("PlayEffect", 0.5f);
    }

    void PlayEffect()
    {
        failEffect.Play();
    }

    void ClickAgainBtn()
    {

    }

    void ClickBackBtn()
    {

    }

    #endregion
}
