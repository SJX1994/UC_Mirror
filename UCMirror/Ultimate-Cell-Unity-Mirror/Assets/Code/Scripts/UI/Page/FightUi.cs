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
/// ս��ҳ�����UIҳ
/// </summary>



public class FightUi : BaseUI
{
    public static UnityAction onMoveCam;
    public override UIType GetUIType()
    {
        return UIType.FightUi;
    }

    private Vector3 m_transform;

    #region �������
    public GameObject HeroConfigItem_0;
    public GameObject HeroConfigItem_1;
    public GameObject HeroConfigItem_2;
    public GameObject HeroConfigItem_3;

    public GameObject UltimateItem;
    public Button UltimateBtn;

    //�������
    public Vector3 EnemyRegionPoint;
    public Vector3 BaseRegionPoint;
    public float Smoothing = 0.5f;
    #endregion

    #region ҵ���߼�
    public override void OnStart()
    {
        #region ����߼�
        //������Ҫ��UI�趨Ϊ3DUI
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
        //������¼�
        onMoveCam();

        if (Input.GetMouseButtonUp(0))
        {
            //�����Ļ�ð�ť�ƶ�
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool isCollider = Physics.Raycast(ray, out hit);
            if (isCollider)
            {
                //hit.point = new Vector3( -14f , hit.point.y + 1f , Mathf.Clamp(hit.point.z,-8,8));
                //���һ��״̬�����ֶ���
                //���һ����Ч��������Ч
                //UltimateItem.transform.position = hit.point;

                var CommunicationManager = GameObject.Find("LanNetWorkManager").gameObject.GetComponent<CommunicationInteractionManager>();

                CommunicationManager.OnUIPosUpdate(UltimateItem.transform.position);
            }
        }

        base.OnUpdate(dealTime);
    }

    #endregion

    #region �¼�����
    void OnUltimateDragStart()
    {
        //Debug.Log("11");
        ////m_transform = UltimateItem.transform.position;
        //Debug.Log(m_transform);
    }

    void OnUltimateDrag()
    {
        //���ű���ק�Ķ���
        //PlayDragAnimation

        //�趨Ultimate������ƺ��϶�����
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

        //�趨Ultimate��������ť����
        List<RaycastResult> list = new List<RaycastResult>();
        //��ȡ�����е�EventSystem,�����λ�ô���eventData
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
        //�ر�����UI

        //�ð���÷���ص�ԭλ��
        UltimateItem.transform.position = m_transform;
        //�����ײ
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool isCollider = Physics.Raycast(ray, out hit);
        if (isCollider)
        {
            if(hit.transform.gameObject.tag == "PlayerRegion")
            {
                //������������ɼ���
                //����״̬
                //��һ�����λ��
            }
        }

        //�趨����ص���ʼ��
        onMoveCam = CamHoming;
    }

    #endregion

    #region �����λ���¼�
    void MoveToEnemy()
    {
        //ִ�������ƫ�������������¼�
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, EnemyRegionPoint, Smoothing * Time.deltaTime);
    }

    void MoveToBase()
    {
        //ִ�������ƫ�������������¼�
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, BaseRegionPoint, Smoothing * Time.deltaTime);
    }

    void CamHoming()
    {
        if (Camera.main.transform.position == new Vector3(0, 17, -3.5f)) return;
        //ִ�лص�ԭ����¼�
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(0, 17, -3.5f), Smoothing * Time.deltaTime);
    }
    #endregion
}
