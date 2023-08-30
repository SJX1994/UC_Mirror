using System.Collections.Generic;
using UnityEngine;

public class CommunicationInteraction: MonoBehaviour
{
    private Dictionary<int, UnitInfoClass> unitInfoState = new();

    private Dictionary<int, UnitInfoClass> VirusInfoState = new();

    private BroadcastClass broadcastClass;

    private CommunicationInteractionManager CommunicationManager;

    private GameObject sceneLoader;

    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError("CommunicationInteraction Start"+ transform.name);
        // TODO 暂时获取方式
        sceneLoader = GameObject.Find("LanNetWorkManager").gameObject;

        // 全局通信方法管理
        CommunicationManager = sceneLoader.GetComponent<CommunicationInteractionManager>();

        // 全局通信事件注册类
        broadcastClass = sceneLoader.GetComponent<BroadcastClass>();

        // 生成砖块信息监听
        broadcastClass.TetrisInfoCreate += OnListenTetrisCreate;

        // 更新砖块信息监听
        broadcastClass.TetrisInfoUpdate += OnListenTetrisUpdate;

        // 外尔新建监听
        broadcastClass.VirusInfoCreate += OnlistenVirusCreate;

        // 外尔信息更新监听
        broadcastClass.VirusInfoUpdate += OnlistenVirusUpdate;

    }

    void OnlistenVirusCreate(List<UnitInfoClass> infoList)
    {
        foreach (UnitInfoClass info in infoList)
        {
            if (!VirusInfoState.ContainsKey(info.UnitIndexId) && info.CreateUnit)
            {
                VirusInfoState.Add(info.UnitIndexId, info);

                Debug.Log("外尔信息接收成功 " + "UnitIndexId:" + info.UnitIndexId + "," + "NewTetris:" + info.CreateUnit + "," + "Pos:" + info.UnitPos);
            }
            else
            {
                Debug.LogError("Unit has been create, please check Tetris logic");
            }
        }
    }

    void OnlistenVirusUpdate(List<UnitInfoClass> infoList)
    {
        foreach (UnitInfoClass info in infoList)
        {
            if (VirusInfoState.TryGetValue(info.UnitIndexId, out UnitInfoClass value))
            {
                VirusInfoState[info.UnitIndexId] = info;

                Debug.Log("外尔信息更新成功 " + "UnitIndexId:" + info.UnitIndexId + "," + "NewTetris:" + info.CreateUnit + "," + "Pos:" + info.UnitPos);
            }
            else
            {
                Debug.LogError("Unit has not been created yet, please check Tetris logic");
            }
        }

    }

    /// <summary>
    /// 砖块生成监听
    /// </summary>
    /// <param name="info"></param>
    void OnListenTetrisCreate(List<UnitInfoClass> infoList) 
    {
        foreach (UnitInfoClass info in infoList) 
        {
            if (!unitInfoState.ContainsKey(info.UnitIndexId) && info.CreateUnit)
            {
                unitInfoState.Add(info.UnitIndexId, info);

                Debug.Log("塞尔信息接收成功 " + "UnitIndexId:" + info.UnitIndexId + "," + "NewTetris:" + info.CreateUnit + "," + "Pos:" + info.UnitPos);
            }
            else 
            {
                Debug.LogError("Unit has been create, please check Tetris logic");
            }
        }
    }

    /// <summary>
    /// 砖块信息更新监听
    /// </summary>
    /// <param name="info"></param>
    void OnListenTetrisUpdate(List<UnitInfoClass> infoList)
    {
        foreach (UnitInfoClass info in infoList)
        {
            if (unitInfoState.TryGetValue(info.UnitIndexId, out UnitInfoClass value))
            {
                unitInfoState[info.UnitIndexId] = info;

                Debug.Log("塞尔信息更新成功 " + "UnitIndexId:" + info.UnitIndexId + "," + "NewTetris:" + info.CreateUnit + "," + "Pos:" + info.UnitPos);
            }
            else
            {
                Debug.LogError("Unit has not been created yet, please check Tetris logic");
            }
        }
    }

    /// <summary>
    /// Unit死亡时的数据通信
    /// </summary>
    /// <param name="UnitIndexId"></param>
    void OnUnitDie(int UnitIndexId) 
    {
        CommunicationManager.UnitDieInfoProcess(UnitIndexId);
    }
}
