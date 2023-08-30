/// **************************************
/// 封装按钮事件
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
        //获取Eventtrigger组件
        EventTriggerListener eventTriggerListener = go.GetComponent<EventTriggerListener>();
        //如果不存在组件，则添加
        if (eventTriggerListener == null)
        {
            eventTriggerListener = go.AddComponent<EventTriggerListener>();
        }
        return eventTriggerListener;
    }
    
    /// <summary>
    /// 重写点击事件
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(gameObject);
    }

    /// <summary>
    /// 重写按下事件
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke(gameObject);
    }

    /// <summary>
    /// 重写抬起事件
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke(gameObject);
    }

    /// <summary>
    /// 重写悬停事件
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerEnter(PointerEventData eventData)
    {
        onMouseEnter?.Invoke(gameObject);
    }

    /// <summary>
    /// 重写离开事件
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerExit(PointerEventData eventData)
    {
        onMouseExit?.Invoke(gameObject);
    }

    /// <summary>
    /// 开始拖拽事件
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag?.Invoke(gameObject);
    }


    /// <summary>
    /// 拖拽中事件
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(gameObject);
    }

    /// <summary>
    /// 拖拽结束事件
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag?.Invoke(gameObject);
    }
}
