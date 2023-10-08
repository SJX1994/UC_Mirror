using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UC_PlayerData;

public class BuildingManager : Singleton<BuildingManager>
{
    public UnityAction<Building> OnUnmarshalCall;
    public UnityAction<Building> OnMarshallingCall;

    public UnityAction<Building> OnKeepCall;

    MechanismInPut mechanismInPut;
    SpriteRenderer marshalling;
    SpriteRenderer keep;
    SpriteRenderer unmarshal;
    SpriteRenderer marshallingText;
    SpriteRenderer unmarshalText;

    SpriteRenderer keepText;
    public List<Building> buildings = new List<Building>();
    const int NotFlowOrder = 101;
    Color FlowColor = new Color(1, 1, 1, 1);
    Color NotFlowColor = new Color(1, 1, 1, 0.5f);
    // 通信管理器
    // private CommunicationInteractionManager CommunicationManager;

    // 己方建筑
    private GameObject SelfBuilding;

    // 敌方建筑
    private GameObject RivalBuilding;
    bool isWarningSystem = false;
    // Start is called before the first frame update
    void Start()
    {
        
        marshalling = transform.Find("marshalling").GetComponent<SpriteRenderer>();
        unmarshal = transform.Find("unmarshal").GetComponent<SpriteRenderer>();
        keep = transform.Find("keep").GetComponent<SpriteRenderer>();
        marshallingText = marshalling.transform.Find("text").GetComponent<SpriteRenderer>();
        unmarshalText = unmarshal.transform.Find("text").GetComponent<SpriteRenderer>();
        keepText = keep.transform.Find("text").GetComponent<SpriteRenderer>();
        Invoke(nameof(LateStart), 0.1f);

        // 通信获取
        // 暂时获取方式
        // CommunicationManager = GameObject.Find(nameof(LanNetWorkManager)).gameObject.GetComponent<CommunicationInteractionManager>();

    }

    void LateStart()
    {
        isWarningSystem = FindObjectOfType<WarningSystem>();
        buildings = FindObjectsOfType<Building>().ToList();
        foreach (var building in buildings)
        {
            building.OnBuildingMoveing += OnBuildingMoveing;
            building.OnBuildingFuctionHit += OnBuildingFuctionHit;
            building.OnBuildingFuctionExit += OnBuildingFuctionExit;
            building.OnTheLastStayBuilding += OnTheLastStayBuilding;
        }

        //设置建筑初始位置 && 用建筑名对建筑进行区分
        // buildings[0].gameObject.name = "SelfBuilding";

        // buildings[0].gameObject.transform.position = new Vector3(-1.08f, 0, -2.52221f);

        // SelfBuilding = buildings[0].gameObject;

        // buildings[1].gameObject.name = "RivalBuilding";

        // buildings[1].gameObject.transform.position = new Vector3(3.78f, 0, -2.52221f);

        // RivalBuilding = buildings[1].gameObject;

       // Debug.Log("建筑初始化完成");
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 建筑位置变更事件
    /// </summary>
    /// <param name="building"></param>
    /// <param name="slot"></param>
    void OnTheLastStayBuilding(Building building, BuildingSlot slot)
    {
        var pos = slot.blockDisplay.posId;

        if (pos != new Vector2(11f, 4f))
        {
            // 发送位置变更信息
            this.SendBuildingState(pos.x, pos.y);
        }
    }

    /// <summary>
    /// 解除编组触发事件
    /// </summary>
    /// <param name="spriteRenderer"></param>
    /// <param name="building"></param>
    void OnBuildingFuctionExit(SpriteRenderer spriteRenderer, Building building)
    {

        if (spriteRenderer.name == "marshalling")
        {
            // // 发送编组信息
            // this.SendBuildingState(-1f, -1f, 1);
            if(isWarningSystem)
                // MechanismInPut.Instance.warningSystem.changeWarningTypes = WarningSystem.WarningType.BuildingMarshalling;
            if (OnMarshallingCall != null)
            {
                OnMarshallingCall(building);
            }
        }
        else if (spriteRenderer.name == "unmarshal")
        {
            // // 发送编组解除信息
            // this.SendBuildingState(-1f, -1f, 2);
            if(isWarningSystem)
                // MechanismInPut.Instance.warningSystem.changeWarningTypes = WarningSystem.WarningType.BuildingUnmarshal;
            if (OnUnmarshalCall != null)
            {
                OnUnmarshalCall(building);
            }
        }
        else if (spriteRenderer.name == "keep")
        {
            // // 发送编组保持信息
            // this.SendBuildingState(-1f, -1f, 2);
            if(isWarningSystem)
                // MechanismInPut.Instance.warningSystem.changeWarningTypes = WarningSystem.WarningType.BuildingKeep;
            if (OnKeepCall != null)
            {
                OnKeepCall(building);
            }
        }
    }

    /// <summary>
    /// 建筑状态变更表现改变事件
    /// </summary>
    /// <param name="spriteRenderer"></param>
    /// <param name="isHit"></param>
    void OnBuildingFuctionHit(SpriteRenderer spriteRenderer, bool isHit)
    {
        if (!isHit)
        {
            marshalling.color = NotFlowColor;
            unmarshal.color = NotFlowColor;
            keep.color = NotFlowColor;
            marshalling.sortingOrder = NotFlowOrder;
            unmarshal.sortingOrder = NotFlowOrder;
            keep.sortingOrder = NotFlowOrder;
            marshallingText.color = NotFlowColor;
            unmarshalText.color = NotFlowColor;
            keepText.color = NotFlowColor;
            marshallingText.sortingOrder = NotFlowOrder - 1;
            unmarshalText.sortingOrder = NotFlowOrder - 1;
            keepText.sortingOrder = NotFlowOrder - 1;
        }
        else
        {
            if (spriteRenderer.name == "marshalling")
            {
                marshalling.color = FlowColor;
                marshalling.sortingOrder = Dispaly.FlowOrder;

                marshallingText.color = FlowColor;
                marshallingText.sortingOrder = Dispaly.FlowOrder + 1;

            }
            else if (spriteRenderer.name == "unmarshal")
            {
                unmarshal.color = FlowColor;
                unmarshal.sortingOrder = Dispaly.FlowOrder;

                unmarshalText.color = FlowColor;
                unmarshalText.sortingOrder = Dispaly.FlowOrder + 1;


            }
            else if (spriteRenderer.name == "keep")
            {
                keep.color = FlowColor;
                keep.sortingOrder = Dispaly.FlowOrder;

                keepText.color = FlowColor;
                keepText.sortingOrder = Dispaly.FlowOrder + 1;


            }
        }

    }

    /// <summary>
    /// 建筑移动事件
    /// </summary>
    /// <param name="building"></param>
    void OnBuildingMoveing(Building building)
    {
        if (building.Draging == false)
        {
            Invoke("Reset", 0.3f);
        }
        else
        {
            Invoke("Active", 0.3f);
        }
    }

    void Active()
    {
        marshalling.color = NotFlowColor;
        unmarshal.color = NotFlowColor;
        keep.color = NotFlowColor;
        marshalling.sortingOrder = NotFlowOrder;
        unmarshal.sortingOrder = NotFlowOrder;
        keep.sortingOrder = NotFlowOrder;
        marshallingText.color = NotFlowColor;
        unmarshalText.color = NotFlowColor;
        keepText.color = NotFlowColor;
        marshallingText.sortingOrder = NotFlowOrder - 1;
        unmarshalText.sortingOrder = NotFlowOrder - 1;
        keepText.sortingOrder = NotFlowOrder - 1;
        marshalling.gameObject.SetActive(true);
        unmarshal.gameObject.SetActive(true);
        keep.gameObject.SetActive(true);
    }

    void Reset()
    {
        marshalling.color = NotFlowColor;
        unmarshal.color = NotFlowColor;
        keep.color = NotFlowColor;
        marshalling.sortingOrder = NotFlowOrder;
        unmarshal.sortingOrder = NotFlowOrder;
        keep.sortingOrder = NotFlowOrder;
        marshallingText.color = NotFlowColor;
        unmarshalText.color = NotFlowColor;
        keepText.color = NotFlowColor;
        marshallingText.sortingOrder = NotFlowOrder - 1;
        unmarshalText.sortingOrder = NotFlowOrder - 1;
        keepText.sortingOrder = NotFlowOrder - 1;
        marshalling.gameObject.SetActive(false);
        unmarshal.gameObject.SetActive(false);
        keep.gameObject.SetActive(false);

    }

    /// <summary>
    /// 发送
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="state"></param>
    void SendBuildingState(float posx = -1, float posy = -1, int state = 0)
    {
        // 新建客户端通讯类
        BuildingStateClass info = new()
        {
            BuildingPosX = posx,
            BuildingPosY = posy,
            FunctionHitState = state
        };

        // 发送至客户端服务器沟通器进行统一更新
        // CommunicationManager.BuildingUpdateFunc(info);

        Debug.Log("发送建筑状态更新：" + posx + ", " + posy + ", " + state);

    }
}
