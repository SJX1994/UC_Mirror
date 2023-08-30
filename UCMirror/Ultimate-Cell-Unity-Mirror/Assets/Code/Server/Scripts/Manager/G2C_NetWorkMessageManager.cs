using Mirror;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unit;
using static UnityEngine.UI.CanvasScaler;

public class G2C_NetWorkMessageManager : NetworkBehaviour
{
    [Header("通讯组件")]
    public G2C_CommunicationManager comm;

    public G2C_BroadcastClass broad;

    public void Start()
    {
        broad.SendBlocksCreateMessage += G2C_SendBlockCreateMessage;

        NetworkServer.RegisterHandler<NetWorkTestClass>(OnServerCustomMessageReceived); // 注册对自定义消息的处理方法

        NetworkServer.RegisterHandler<BlocksInfoStruct>(C2G_GetPlayerBlocksMessage); // 接收砖块消息

        NetworkServer.RegisterHandler<BuildingStatestruct>(C2G_GetBuildingStateMessage); // 接收砖块消息

    }

    public void OnServerCustomMessageReceived(NetworkConnection conn, NetWorkTestClass message)
    {
        Debug.Log("服务器收到自定义消息：" + message.myString);

        SendCustomMessageToClient(conn);
    }

    void SendCustomMessageToClient(NetworkConnection connection)
    {
        NetWorkTestClass msg = new NetWorkTestClass
        {
            myInt = 42,
            myString = "Hello Client"
        };

        Debug.Log($"Send Message To Client : {connection.connectionId}");

        connection.Send(msg, connection.connectionId); // 发送自定义消息到指定的客户端
    }

    /// <summary>
    /// 砖块消息接收
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="message"></param>
    void C2G_GetPlayerBlocksMessage(NetworkConnection connection, BlocksInfoStruct message) 
    {
        G2C_BlocksMessageClass msg = new G2C_BlocksMessageClass() 
        {
            ConnId= connection.connectionId,
            Conn= connection,
            BlocksInfo = message
        };

        comm.GetBlocksMessageFromClient(msg);
    }

    /// <summary>
    /// 发送砖块新建信息
    /// </summary>
    /// <param name="msg"></param>
    void G2C_SendBlockCreateMessage(G2C_OpponentBricksClass info) 
    {
        var ConnSend = info.sendPlayer;

        OpponentBricksStruct msg = new OpponentBricksStruct()
        {
            tetrisClaass = info.tetrisClaass,
            fromplayer = info.fromplayer.connectionId,
            sendPlayer = info.sendPlayer.connectionId,
            
        };

        ConnSend.Send(msg, ConnSend.connectionId);
    }

    /// <summary>
    /// 发送木偶新建信息
    /// </summary>
    /// <param name="info"></param>
    public void G2C_SendPuppetCteateInfo(PuppetUnitStateStruct info, NetworkConnection player) 
    {
        player.Send(info, player.connectionId);
    }

    /// <summary>
    /// 向玩家发送unit更新信息
    /// </summary>
    /// <param name="info"></param>
    /// <param name="player"></param>
    public void G2C_SendPuppetUpdateInfo(G2C_UnitInfoUpdateStruct info, NetworkConnection player) 
    {
        player.Send(info, player.connectionId);
    }

    /// <summary>
    /// 向玩家发送砖块更新信息
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="player"></param>
    public void G2C_SendTetrisState(G2C_TetrisUpdateStruct msg, NetworkConnection player) 
    {
        player.Send(msg, player.connectionId);
    }

    /// <summary>
    /// 发送碰撞盒更新事件
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="player"></param>
    public void G2C_SendGridUpdateInfo(G2C_TetrisGridUpdateStruct msg, NetworkConnection player) 
    {
        player.Send(msg, player.connectionId);
    }

    /// <summary>
    /// 接收建筑状态变更信息
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="msg"></param>
    public void C2G_GetBuildingStateMessage(NetworkConnection connection, BuildingStatestruct msg) 
    {
        BuildingStateClass info = new BuildingStateClass();

        info.conn = connection;

        info.BuildingPosX = msg.BuildingPos.x;

        info.BuildingPosY = msg.BuildingPos.y;

        info.FunctionHitState = 0;

        if (msg.FunctionHit) 
        {
            info.FunctionHitState = 1;
        }
        else if (msg.FunctionExit) 
        {
            info.FunctionHitState = 2;
        }

        comm.GetBuildingStateFromClient(info);
    }

    /// <summary>
    /// 发送木偶固定信息
    /// </summary>
    /// <param name="info"></param>
    /// <param name="connection"></param>
    public void G2C_SendPuppetFixedInfo(G2C_PuppetFixedinformationStruct info, NetworkConnection connection) 
    {
        connection.Send(info, connection.connectionId);
    }
}
