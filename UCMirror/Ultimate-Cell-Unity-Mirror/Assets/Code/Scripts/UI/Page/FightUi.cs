using GameFrameWork;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 战斗页面基础UI页
/// </summary>



public class FightUi : BaseUI
{
    public static UnityAction onMoveCam;
    public override UIType GetUIType()
    {
        return UIType.FightUi;
    }

    private Vector3 m_transform;

    #region 交互组件
    public GameObject HeroConfigItem_0;
    public GameObject HeroConfigItem_1;
    public GameObject HeroConfigItem_2;
    public GameObject HeroConfigItem_3;

    public GameObject UltimateItem;
    public Button UltimateBtn;

    //相机参数
    public Vector3 EnemyRegionPoint;
    public Vector3 BaseRegionPoint;
    public float Smoothing = 0.5f;
    #endregion

    #region 业务逻辑
    public override void OnStart()
    {
        #region 相机逻辑
        //这里需要把UI设定为3DUI
        Camera uiCam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        Canvas canvas = this.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = uiCam;
        canvas.sortingOrder = 21;
        #endregion

        m_transform = UltimateItem.transform.position;

        onMoveCam = CamHoming;

        BtnEvent.RigisterButtonDragStartEvent(UltimateItem.transform.gameObject, p => { OnUltimateDragStart(); });
        BtnEvent.RigisterButtonDragEvent(UltimateBtn.transform.gameObject, p => { OnUltimateDrag(); });
        BtnEvent.RigisterButtonDragEndEvent(UltimateBtn.transform.gameObject, p => { OnUltimateDragEnd(); });

        base.OnAwake();
    }

    public override void OnUpdate(float dealTime)
    {
        //摄像机事件
        onMoveCam();

        if (Input.GetMouseButtonUp(0))
        {
            //点击屏幕让按钮移动
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool isCollider = Physics.Raycast(ray, out hit);
            if (isCollider)
            {
                //hit.point = new Vector3( -14f , hit.point.y + 1f , Mathf.Clamp(hit.point.z,-8,8));
                //添加一个状态，闪现动画
                //添加一个特效，闪现特效
                //UltimateItem.transform.position = hit.point;

                var CommunicationManager = GameObject.Find("LanNetWorkManager").gameObject.GetComponent<CommunicationInteractionManager>();

                CommunicationManager.OnUIPosUpdate(UltimateItem.transform.position);
            }
        }

        base.OnUpdate(dealTime);
    }

    #endregion

    #region 事件监听
    void OnUltimateDragStart()
    {
        //Debug.Log("11");
        ////m_transform = UltimateItem.transform.position;
        //Debug.Log(m_transform);
    }

    void OnUltimateDrag()
    {
        //播放被拖拽的动画
        //PlayDragAnimation

        //设定Ultimate点击控制和拖动控制
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool isCollider = Physics.Raycast(ray, out hit);
        if (isCollider)
        {
            if (hit.transform.gameObject.tag == "EnemyRegion") onMoveCam = MoveToEnemy;
            else if (hit.transform.gameObject.tag == "BaseRegion") onMoveCam = MoveToBase;

            hit.point = new Vector3(hit.point.x, hit.point.y + 1f, Mathf.Clamp(hit.point.z, -8, 8));
            UltimateItem.transform.position = hit.point;
            UltimateItem.transform.SetParent(this.transform, true);
        }

        //设定Ultimate和其他按钮交互
        List<RaycastResult> list = new List<RaycastResult>();
        //获取场景中的EventSystem,将鼠标位置传给eventData
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(eventData, list);
        for (int i = 0; i < list.Count; i++)
        {
            GameObject obj = list[i].gameObject;
            Debug.Log(obj.tag);
        }
    }

    void OnUltimateDragEnd()
    {
        //关闭所有UI

        //让奥特梅尔回到原位置
        UltimateItem.transform.position = m_transform;
        //检测碰撞
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool isCollider = Physics.Raycast(ray, out hit);
        if (isCollider)
        {
            if(hit.transform.gameObject.tag == "PlayerRegion")
            {
                //在玩家区域活动生成技能
                //播放状态
                //传一个最后位置
            }
        }

        //设定相机回到初始点
        onMoveCam = CamHoming;
    }

    #endregion

    #region 摄像机位移事件
    void MoveToEnemy()
    {
        //执行摄像机偏移向敌人区域的事件
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, EnemyRegionPoint, Smoothing * Time.deltaTime);
    }

    void MoveToBase()
    {
        //执行摄像机偏移向敌人区域的事件
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, BaseRegionPoint, Smoothing * Time.deltaTime);
    }

    void CamHoming()
    {
        if (Camera.main.transform.position == new Vector3(0, 17, -3.5f)) return;
        //执行回到原点的事件
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(0, 17, -3.5f), Smoothing * Time.deltaTime);
    }
    #endregion
}
