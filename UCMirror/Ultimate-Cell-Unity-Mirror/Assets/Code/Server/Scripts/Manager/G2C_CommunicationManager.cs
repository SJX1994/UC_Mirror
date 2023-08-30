using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class G2C_CommunicationManager : MonoBehaviour 
{
    /// <summary>
    /// 数据管理类
    /// </summary>
    public G2C_BroadcastClass broadcastClass;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// 玩家加入大厅
    /// </summary>
    /// <param name="info"></param>
    public void PlayerAddRoom(int info) 
    {
        var PlayerAddRoom = broadcastClass.PlayerAddRoom;

        if (PlayerAddRoom != null) 
        {
            PlayerAddRoom(info);
        }
    }

    /// <summary>
    /// 玩家开始匹配
    /// </summary>
    /// <param name="info"></param>
    public void PlayerStartMatching(NetworkConnection info) 
    {
        var PlayerStartMatching = broadcastClass.PlayerStartMatching;

        if (PlayerStartMatching != null) 
        {
            PlayerStartMatching(info);
        }
    }

    /// <summary>
    /// 通知玩家开始切换场景
    /// </summary>
    /// <param name="info"></param>
    public void PlayerChangeScene(List<NetworkConnection> info) 
    {
        var PlayerChangeScene = broadcastClass.PlayerChangeScene;

        if (PlayerChangeScene != null) 
        {
            PlayerChangeScene(info);
        }
    }

    /// <summary>
    /// 获取玩家发送的砖块信息
    /// </summary>
    /// <param name="info"></param>
    public void GetBlocksMessageFromClient(G2C_BlocksMessageClass info) 
    {
        var GetBlocksMessageFromClient = broadcastClass.GetBlocksInfo;

        if (GetBlocksMessageFromClient != null) 
        {
            GetBlocksMessageFromClient(info);
        }
    }

    /// <summary>
    /// 发送砖块信息
    /// </summary>
    /// <param name="info"></param>
    public void SendBlockCreateMessage(G2C_OpponentBricksClass info) 
    {
        var SendBlockCreateMessage = broadcastClass.SendBlocksCreateMessage;

        if (SendBlockCreateMessage != null) 
        {
            SendBlockCreateMessage(info);
        }
    }

    /// <summary>
    /// 发送木偶新建信息
    /// </summary>
    public void SendPuppetCteateInfo(G2C_PuppetStateClass info) 
    {
        var SendPuppetCteateInfo = broadcastClass.SendPuppetCteateInfo;

        if (SendPuppetCteateInfo != null) 
        {
            SendPuppetCteateInfo(info);
        }
    }

    /// <summary>
    /// 获取建筑状态更新信息
    /// </summary>
    /// <param name="info"></param>
    public void GetBuildingStateFromClient(BuildingStateClass info) 
    {
        var GetBuildingStateFromClient = broadcastClass.GetBuildingStateFromClient;

        if (GetBuildingStateFromClient != null) 
        {
            GetBuildingStateFromClient(info);
        }
    }
}