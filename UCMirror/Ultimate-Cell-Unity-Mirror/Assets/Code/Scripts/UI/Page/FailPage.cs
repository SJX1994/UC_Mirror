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

    #region �������

    [Header("��ť")]
    public Button againBtn;
    public Button backBtn;

    [Header("�ı�")]
    public Text infoText;

    [Header("��Ч")]
    public ParticleSystem failEffect;

    #endregion

    #region ҵ���߼�

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

        Animation anim = GetComponent<Animation>();
        anim.Play("A_Fail");

        //������
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
