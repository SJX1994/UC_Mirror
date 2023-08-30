using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLevel : BaseUI
{
    private Canvas canvas;

    public override UIType GetUIType()
    {
        return UIType.None;
    }

    //把Level按钮封装
    public Button Btn_Level_0;
    public Button Btn_Level_1;
    public Button Btn_Level_2;
    public Button Btn_Level_3;
    public Button Btn_Level_4;
    public Button Btn_Level_5;

    public Button Back;

    #region Unity Functions
    public override void OnAwake()
    {
        //Camera cam = Camera.main;
        //Debug.Log(cam.name);
        //canvas = this.transform.GetComponent<Canvas>();
        //if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        //{
        //    canvas.worldCamera = cam;
        //}
        //else
        //{
        //    canvas.renderMode = RenderMode.ScreenSpaceCamera;
        //    canvas.worldCamera = cam;
        //}
        //canvas.planeDistance = 10;
        //canvas.sortingOrder = 10;
        #region Click Event
        BtnEvent.RigisterButtonClickEvent(Btn_Level_0.transform.gameObject, p =>
        {
            OnLevelClick(null);
        });
        BtnEvent.RigisterButtonClickEvent(Btn_Level_1.transform.gameObject, p =>
        {
            OnLevelClick(null);
        });
        BtnEvent.RigisterButtonClickEvent(Btn_Level_2.transform.gameObject, p =>
        {
            OnLevelClick(null);
        });
        BtnEvent.RigisterButtonClickEvent(Btn_Level_3.transform.gameObject, p =>
        {
            OnLevelClick(null);
        });
        BtnEvent.RigisterButtonClickEvent(Btn_Level_4.transform.gameObject, p =>
        {
            OnLevelClick(null);
        });
        BtnEvent.RigisterButtonClickEvent(Btn_Level_5.transform.gameObject, p =>
        {
            OnLevelClick(null);
        });
        BtnEvent.RigisterButtonClickEvent(Back.transform.gameObject, p =>
        {
            OnBackClick();
        });
        #endregion

    }
    #endregion

    void OnLevelClick(object[] _uiparam)
    {
        if (UIManager.Instance.TryGetUI(UIType.None))
        {
            UIManager.Instance.CloseUI(UIType.None);
            if (!UIManager.Instance.TryGetUI(UIType.None))
            {
                UIManager.Instance.OpenUI(UIType.None, _uiparam);
            }
            else
            {
                UIManager.Instance.CloseUI(UIType.None);
            }
        }
        else
        {
            UIManager.Instance.OpenUI(UIType.None, _uiparam);
        }
    }

    void OnBackClick()
    {
        Debug.Log("放弃选择关卡，回到主页面");
        UIManager.Instance.OpenUICloseOtherUI(UIType.HomePage2D, null);
    }

}
