/// **************************************
/// ��ť�¼��Ľӿ�
/// **************************************
using UnityEngine;

public class BtnEvent : MonoBehaviour
{
    /// <summary>
    /// ע�ᰴť����¼�
    /// </summary>
    /// <param name="buttonName">��ť����</param>
    /// <param name="myEventHandler">������¼�</param>
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
    /// ע�ᰴť��ͣ�¼�
    /// </summary>
    /// <param name="buttonName">��ť����</param>
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
    /// ע�ᰴť����
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
    /// ע�ᰴţ̌��
    /// </summary>
    /// <param name="buttonObj">��ť</param>
    /// <param name="myEventHandler">״̬�ӿ�</param>
    public static void RigisterButtonUpEvent(GameObject buttonObj, EventTriggerListener.MyEventHandler myEventHandler)
    {
        if (buttonObj != null)
        {
            EventTriggerListener.GetListener(buttonObj).onPointerUp = myEventHandler;
        }
    }

    /// <summary>
    /// ע�ᰴť�뿪�¼�
    /// </summary>
    /// <param name="buttonName">��ť����</param>
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
    /// ע�ᰴť�϶���ʼ�¼�
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
    /// ע�ᰴť�϶��¼�
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
    /// ע�ᰴť�϶������¼�
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
