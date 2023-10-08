/// **************************************
/// 按钮事件的接口
/// **************************************
using UnityEngine;

public class BtnEvent : MonoBehaviour
{
    /// <summary>
    /// 注册按钮点击事件
    /// </summary>
    /// <param name="buttonName">按钮名称</param>
    /// <param name="myEventHandler">点击的事件</param>
    public static void RigisterButtonClickEvent(GameObject[] buttonObjs, EventTriggerListener.MyEventHandler myEventHandler)
    {
        for (int i = 0; i < buttonObjs.Length; i++)
        {
            if (buttonObjs[i] != null)
            {
                RigisterButtonClickEvent(buttonObjs[i], myEventHandler);
            }
        }
    }
    public static void RigisterButtonClickEvent(GameObject buttonObj, EventTriggerListener.MyEventHandler myEventHandler)
    {
        if (buttonObj != null)
        {
            EventTriggerListener.GetListener(buttonObj).onClick = myEventHandler;
        }
    }
    /// <summary>
    /// 注册按钮悬停事件
    /// </summary>
    /// <param name="buttonName">按钮名称</param>
    /// <param name="myEventHandler"></param>
    public static void RigisterButtonEnterEvent(GameObject[] buttonObjs, EventTriggerListener.MyEventHandler myEventHandler)
    {
        for (int i = 0; i < buttonObjs.Length; i++)
        {
            if (buttonObjs[i] != null)
            {
                RigisterButtonEnterEvent(buttonObjs[i], myEventHandler);
            }
        }
    }

    public static void RigisterButtonEnterEvent(GameObject buttonObj, EventTriggerListener.MyEventHandler myEventHandler)
    {
        if (buttonObj != null)
        {
            EventTriggerListener.GetListener(buttonObj).onMouseEnter = myEventHandler;
        }
    }

    /// <summary>
    /// 注册按钮按下
    /// </summary>
    /// <param name="buttonObj"></param>
    /// <param name="myEventHandler"></param>
    public static void RigisterButtonDownEvent(GameObject buttonObj, EventTriggerListener.MyEventHandler myEventHandler)
    {
        if (buttonObj != null)
        {
            EventTriggerListener.GetListener(buttonObj).onPointerDown = myEventHandler;
        }
    }
    
    /// <summary>
    /// 注册按钮抬起
    /// </summary>
    /// <param name="buttonObj">按钮</param>
    /// <param name="myEventHandler">状态接口</param>
    public static void RigisterButtonUpEvent(GameObject buttonObj, EventTriggerListener.MyEventHandler myEventHandler)
    {
        if (buttonObj != null)
        {
            EventTriggerListener.GetListener(buttonObj).onPointerUp = myEventHandler;
        }
    }

    /// <summary>
    /// 注册按钮离开事件
    /// </summary>
    /// <param name="buttonName">按钮名称</param>
    /// <param name="myEventHandler"></param>
    public static void RigisterButtonExitEvent(GameObject[] buttonObjs, EventTriggerListener.MyEventHandler myEventHandler)
    {
        for (int i = 0; i < buttonObjs.Length; i++)
        {
            if (buttonObjs[i] != null)
            {
                RigisterButtonExitEvent(buttonObjs[i], myEventHandler);
            }
        }
    }

    public static void RigisterButtonExitEvent(GameObject buttonObj, EventTriggerListener.MyEventHandler myEventHandler)
    {
        if (buttonObj != null)
        {
            EventTriggerListener.GetListener(buttonObj).onMouseExit = myEventHandler;
        }
    }

    /// <summary>
    /// 注册按钮拖动开始事件
    /// </summary>
    /// <param name="buttonObj"></param>
    /// <param name="myEventHandler"></param>
    public static void RigisterButtonDragStartEvent(GameObject buttonObj, EventTriggerListener.MyEventHandler myEventHandler)
    {
        if(buttonObj != null)
        {
            EventTriggerListener.GetListener(buttonObj). onBeginDrag = myEventHandler;
        }
    }

    /// <summary>
    /// 注册按钮拖动事件
    /// </summary>
    /// <param name="buttonObj"></param>
    /// <param name="myEventHandler"></param>
    public static void RigisterButtonDragEvent(GameObject buttonObj, EventTriggerListener.MyEventHandler myEventHandler)
    {
        if (buttonObj != null)
        {
            EventTriggerListener.GetListener(buttonObj).onDrag = myEventHandler;
        }
    }

    /// <summary>
    /// 注册按钮拖动结束事件
    /// </summary>
    /// <param name="buttonObj"></param>
    /// <param name="myEventHandler"></param>
    public static void RigisterButtonDragEndEvent(GameObject buttonObj, EventTriggerListener.MyEventHandler myEventHandler)
    {
        if (buttonObj != null)
        {
            EventTriggerListener.GetListener(buttonObj).onEndDrag = myEventHandler;
        }
    }
}
