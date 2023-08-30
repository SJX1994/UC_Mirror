using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 战斗页面上浮UI
/// </summary>
public class LegionUi : BaseUI
{
    public override UIType GetUIType()
    {
        return UIType.LegionUi;
    }

    #region 交互组件

    public TextMeshProUGUI SoulNum;
    public TextMeshProUGUI ExpNum;

    public Button UpBtn;
    public Button LowBtn;
    public Button ReseatBtn;
    public Button AcceBtn;

    #endregion

    #region 长按参数

    [Tooltip("长按多少秒触发时间")]
    public float intervalTime = 0.5f;

    [Tooltip("长按每次多久触发一次事件")]
    public float trrigerIntervalTime = 0.5f;

    [Tooltip("长按每次最短多久触发一次事件")]
    public float trrigerIntervalMinTime = 0.1f;

    [Tooltip("长按触发事件间隔每次减少的时间")]
    public float trrigerReduceTime = 0.1f;

    [Tooltip("是否可以触发")]
    public bool interactable = true;

    #region 事件
    [Tooltip("长按中触发什么事件")]
    public UnityEvent onPressing;
    [Tooltip("长按到触发间隔后只触发一次事件")]
    public UnityEvent onPressBegin;
    [Tooltip("长按开始触发的事件")]
    public UnityEvent onStartPress;
    [Tooltip("退出长按时触发的事件")]
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

    #region 通讯组件
    private CommunicationInteractionManager CommunicationManager;
    #endregion

    #region 业务逻辑
    public override void OnStart()
    {
        // 通信获取
        // TODO 暂时获取方式
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

    #region 数据加载

    public override void OnLoadData(params object[] param)
    {
        //先将数据加载进来
        base.OnLoadData(param);
    }
    #endregion

    #region 事件监听
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
