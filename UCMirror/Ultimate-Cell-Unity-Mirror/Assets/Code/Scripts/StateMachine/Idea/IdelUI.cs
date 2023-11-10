
using GameFrameWork;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UC_PlayerData;
using Mirror;
public class IdelUI : NetworkBehaviour
{
    #region 数据对象
    public IdelHolder idelHolder;
    public Player player;
    public GameObject SelectionBox;

    public GameObject[] BoxInfo;
    MechanismInPut mechanismInPut;
    public bool hiden = false;
    #endregion

    #region 数据关系
    void Start()
    {
        foreach (var Info in BoxInfo)
        {
            if(!Info)continue;
            Info.GetComponent<IdelBox>().idelUI = this;
            Info.GetComponent<IdelBox>().player = player;
        }
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
            // ChangeHideColorRecursive(Info.transform);
        }
        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        Vector2 newPosition = new Vector2(-4.44f, rectTransform.anchoredPosition.y);
        rectTransform.anchoredPosition = newPosition;
        hiden = true;
        Button refalshButton = transform.Find("RefreshButton").GetComponent<Button>();
        refalshButton.interactable  = false;
    }
    #endregion

    #region 数据方法
    private void ChangeHideColorRecursive(Transform parent)
    {
        // 获取当前物体的Image组件
        if (parent.TryGetComponent<Image>(out var image))
        {
            // 修改颜色
            image.color = new Color(image.color.r, image.color.g, image.color.b, Dispaly.HidenAlpha);
        }
        
        // 遍历所有子物体并递归调用ChangeColorRecursive方法
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            ChangeHideColorRecursive(child);
        }
    }
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
        // MechanismInPut.Instance.warningSystem.changeWarningTypes = WarningSystem.WarningType.Reflash;
        // Debug.Log("刷新想法");
        foreach (GameObject Info in BoxInfo) 
        {
            if(!Info)continue;
            Info.GetComponent<IdelBox>().RefreshGameObj();
        }
        
        
    }
    
    #endregion
}
