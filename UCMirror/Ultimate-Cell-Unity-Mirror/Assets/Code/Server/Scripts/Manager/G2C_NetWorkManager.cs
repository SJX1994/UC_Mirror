using Mirror;
using Mirror.Discovery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G2C_NetWorkManager : NetworkManager
{

    [Header("网络连接组件")]
    public NetworkManager manager;

    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

    [Header("预制体")]
    public GameObject playerInfo;

    // 本地服务启动程序
    private CommunicationInteractionManager comm;

    public override void Start()
    {
        comm = GameObject.Find("LanNetWorkManager").GetComponent<CommunicationInteractionManager>();

        if (comm.ServerState == 2) 
        {
            this.gameObject.GetComponent<NetworkManagerHUD>().enabled = false;
        }
    }

    /// <summary>
    /// 启动服务器函数
    /// </summary>
    public override void OnStartServer()
    {
        Debug.Log("Server Started !!!");
    }

    /// <summary>
    /// 停止服务器函数
    /// </summary>
    public override void OnStopServer()
    {
        Debug.Log("Server Stopped !!!");
    }

    /// <summary>
    /// 客户端连接组件
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log($"Client Connect ConnId={conn.connectionId}");

        ConnectionStruct msg = new ConnectionStruct()
        {
            success = true
        };

        conn.Send(msg, conn.connectionId);
    }

    /// <summary>
    /// 客户端断开链接组件
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.Log($"Client DisConnect ConnId={conn.connectionId}");
    }
}
