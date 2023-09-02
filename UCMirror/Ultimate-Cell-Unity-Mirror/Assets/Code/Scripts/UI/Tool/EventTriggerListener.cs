/// **************************************
/// ��װ��ť�¼�
/// **************************************
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerListener : EventTrigger
{
    public delegate void MyEventHandler(GameObject go);

    public MyEventHandler onClick;
    public MyEventHandler onMouseEnter;
    public MyEventHandler onMouseExit;
    public MyEventHandler onBeginDrag;
    public MyEventHandler onDrag;
    public MyEventHandler onEndDrag;
    public MyEventHandler onPointerDown;
    public MyEventHandler onPointerUp;
    public static EventTriggerListener GetListener(GameObject go)
    {
        //��ȡEventtrigger���
        EventTriggerListener eventTriggerListener = go.GetComponent<EventTriggerListener>();
        //��������������������
        if (eventTriggerListener == null)
        {
            eventTriggerListener = go.AddComponent<EventTriggerListener>();
        }
        return eventTriggerListener;
    }
    
    /// <summary>
    /// ��д����¼�
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(gameObject);
    }

    /// <summary>
    /// ��д�����¼�
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke(gameObject);
    }

    /// <summary>
    /// ��д̧���¼�
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke(gameObject);
    }

    /// <summary>
    /// ��д��ͣ�¼�
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerEnter(PointerEventData eventData)
    {
        onMouseEnter?.Invoke(gameObject);
    }

    /// <summary>
    /// ��д�뿪�¼�
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerExit(PointerEventData eventData)
    {
        onMouseExit?.Invoke(gameObject);
    }

    /// <summary>
    /// ��ʼ��ק�¼�
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag?.Invoke(gameObject);
    }


    /// <summary>
    /// ��ק���¼�
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(gameObject);
    }

    /// <summary>
    /// ��ק�����¼�
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag?.Invoke(gameObject);
    }
}
