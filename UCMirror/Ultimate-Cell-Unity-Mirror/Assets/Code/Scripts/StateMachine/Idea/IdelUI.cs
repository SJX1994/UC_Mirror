
using GameFrameWork;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UC_PlayerData;

public class IdelUI : MonoBehaviour
{
    #region 数据对象
    public IdelHolder idelHolder;
    public Player player;
    public GameObject SelectionBox;

    public GameObject[] BoxInfo;
    MechanismInPut mechanismInPut;
    #endregion

    #region 数据关系
    void Start()
    {
        // Active();
    }
    public void Active()
    {
        if(!idelHolder)
        {
            idelHolder = transform.parent.GetComponent<IdelHolder>();
            idelHolder.idelUI = this;
            player = idelHolder.player;
        }
      
        Invoke(nameof(Init), 0.1f);
        
    }
    public void Hide()
    {
        foreach (var Info in BoxInfo)
        {
            Info.SetActive(false);
        }
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
        Invoke(nameof(LateStart), 0.2f);
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
