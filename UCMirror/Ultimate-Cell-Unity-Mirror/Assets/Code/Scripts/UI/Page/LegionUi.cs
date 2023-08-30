using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// ս��ҳ���ϸ�UI
/// </summary>
public class LegionUi : BaseUI
{
    public override UIType GetUIType()
    {
        return UIType.LegionUi;
    }

    #region �������

    public TextMeshProUGUI SoulNum;
    public TextMeshProUGUI ExpNum;

    public Button UpBtn;
    public Button LowBtn;
    public Button ReseatBtn;
    public Button AcceBtn;

    #endregion

    #region ��������

    [Tooltip("���������봥��ʱ��")]
    public float intervalTime = 0.5f;

    [Tooltip("����ÿ�ζ�ô���һ���¼�")]
    public float trrigerIntervalTime = 0.5f;

    [Tooltip("����ÿ����̶�ô���һ���¼�")]
    public float trrigerIntervalMinTime = 0.1f;

    [Tooltip("���������¼����ÿ�μ��ٵ�ʱ��")]
    public float trrigerReduceTime = 0.1f;

    [Tooltip("�Ƿ���Դ���")]
    public bool interactable = true;

    #region �¼�
    [Tooltip("�����д���ʲô�¼�")]
    public UnityEvent onPressing;
    [Tooltip("���������������ֻ����һ���¼�")]
    public UnityEvent onPressBegin;
    [Tooltip("������ʼ�������¼�")]
    public UnityEvent onStartPress;
    [Tooltip("�˳�����ʱ�������¼�")]
    public UnityEvent onEndPress;
    #endregion

    [HideInInspector]
    public bool isPressing { get; private set; }

    private bool m_IsPressingBegin = false;
    private bool m_IsStartPress = false;
    private float m_CountTime;
    private float m_TriggerCountTime;
    private float m_TriggerReduceCountTime;

    #endregion

    #region ͨѶ���
    private CommunicationInteractionManager CommunicationManager;
    #endregion

    #region ҵ���߼�
    public override void OnStart()
    {
        // ͨ�Ż�ȡ
        // TODO ��ʱ��ȡ��ʽ
        CommunicationManager = GameObject.Find("LanNetWorkManager").gameObject.GetComponent<CommunicationInteractionManager>();

        BtnEvent.RigisterButtonClickEvent(UpBtn.transform.gameObject, p => { OnClickUpBtn(); });
        BtnEvent.RigisterButtonClickEvent(LowBtn.transform.gameObject, p => { OnClickLowBtn(); });
        BtnEvent.RigisterButtonClickEvent(ReseatBtn.transform.gameObject, p => { OnClickReseatBtn(); });
        BtnEvent.RigisterButtonDownEvent(AcceBtn.transform.gameObject, p => { OnClickAcceBtn(); });
        BtnEvent.RigisterButtonUpEvent(AcceBtn.transform.gameObject, p => { OnClickModerate(); });
    }

    public override void OnUpdate(float dealTime)
    {
        if (interactable)
        {
            if (m_IsStartPress)
            {
                m_CountTime = intervalTime;
                m_TriggerReduceCountTime = trrigerIntervalTime;
                m_IsStartPress = false;
                if (onStartPress != null)
                    onStartPress.Invoke();
            }
            if (isPressing)
            {
                m_CountTime -= Time.deltaTime;
                if (m_CountTime <= 0)
                {
                    if (m_TriggerCountTime > 0)
                    {
                        m_TriggerCountTime -= Time.deltaTime;
                    }
                    else
                    {
                        m_TriggerCountTime = m_TriggerReduceCountTime;
                        if (m_TriggerReduceCountTime > trrigerIntervalMinTime)
                            m_TriggerReduceCountTime -= trrigerReduceTime;
                        if (!m_IsPressingBegin)
                        {
                            if (onPressBegin != null)
                                onPressBegin.Invoke();
                            m_IsPressingBegin = true;
                        }
                        if (onPressing != null)
                            onPressing.Invoke();
                    }
                }
            }
        }
    }
    #endregion

    #region ���ݼ���

    public override void OnLoadData(params object[] param)
    {
        //�Ƚ����ݼ��ؽ���
        base.OnLoadData(param);
    }
    #endregion

    #region �¼�����
    void OnClickUpBtn()
    {
        CommunicationManager.OnUIControl(EventType.UIControl.Up);
    }

    void OnClickLowBtn()
    {
        CommunicationManager.OnUIControl(EventType.UIControl.Down);

    }

    void OnClickReseatBtn()
    {
        CommunicationManager.OnUIControl(EventType.UIControl.Trans);

    }

    void OnClickAcceBtn()
    {
        CommunicationManager.OnUIControl(EventType.UIControl.Acce);
    }

    void OnClickModerate()
    {
        CommunicationManager.OnUIControl(EventType.UIControl.Moderate);
    }
    #endregion
}
