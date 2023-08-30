
using GameFrameWork;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ResManager;

public class IdelUI : MonoBehaviour
{
   

    #region 数据对象
    public GameObject SelectionBox;

    public GameObject[] BoxInfo;
    MechanismInPut mechanismInPut;
    #endregion

    #region 数据关系
    void Start()
    {
        Invoke("Init", 0.1f);
    }


    #endregion

    #region 数据方法
    void Init()
    {
        //camera
        if(GameObject.FindGameObjectWithTag("UICamera"))
        {
            Camera uiCam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
            Canvas canvas = this.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = uiCam;
            canvas.sortingOrder = 21;
        }
        
        
        Invoke("LateStart", 0.2f);
    }
    void LateStart()
    {
        mechanismInPut = FindObjectOfType<MechanismInPut>();
    }
    /// <summary>
    /// 想法刷新按钮触发事件
    /// </summary>
    public void RefreshButtonClick() 
    {
        MechanismInPut.Instance.warningSystem.changeWarningTypes = WarningSystem.WarningType.Reflash;

        foreach (GameObject Info in BoxInfo) 
        {
            Info.GetComponent<IdelBox>().RefreshGameObj();
        }

        AudioSystemManager.Instance.PlaySound("Button_Click");
    }
    #endregion
}
