using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class G2C_BroadcastClass : MonoBehaviour 
{
    /// <summary>
    /// 玩家加入游戏大厅
    /// </summary>
    public UnityAction<int> PlayerAddRoom;

    /// <summary>
    /// 玩家开始匹配
    /// </summary>
    public UnityAction<NetworkConnection> PlayerStartMatching;

    /// <summary>
    /// 通知玩家切换场景
    /// </summary>
    public UnityAction<List<NetworkConnection>> PlayerChangeScene;

    /// <summary>
    /// 获取到玩家发送的砖块信息
    /// </summary>
    public UnityAction<G2C_BlocksMessageClass> GetBlocksInfo;

    /// <summary>
    /// 发送砖块信息
    /// </summary>
    public UnityAction<G2C_OpponentBricksClass> SendBlocksCreateMessage;

    /// <summary>
    /// 发送木偶新建信息
    /// </summary>
    public UnityAction<G2C_PuppetStateClass> SendPuppetCteateInfo;

    /// <summary>
    /// 建筑信息
    /// </summary>
    public UnityAction<BuildingStateClass> GetBuildingStateFromClient;

}