using Mirror;
using Mirror.Discovery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G2C_NetWorkManager : NetworkManager
{

    [Header("�����������")]
    public NetworkManager manager;

    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

    [Header("Ԥ����")]
    public GameObject playerInfo;

    // ���ط�����������
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
    /// ��������������
    /// </summary>
    public override void OnStartServer()
    {
        Debug.Log("Server Started !!!");
    }

    /// <summary>
    /// ֹͣ����������
    /// </summary>
    public override void OnStopServer()
    {
        Debug.Log("Server Stopped !!!");
    }

    /// <summary>
    /// �ͻ����������
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
    /// �ͻ��˶Ͽ��������
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.Log($"Client DisConnect ConnId={conn.connectionId}");
    }
}
