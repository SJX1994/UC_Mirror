using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class G2C_MatchingManager : NetworkBehaviour 
{

    [Header("通讯组件")]
    public G2C_CommunicationManager comm;

    public G2C_BroadcastClass broad;

    public void Start()
    {
        NetworkServer.RegisterHandler<StartMatchingStruct>(StartMatchingManager); // 注册对自定义消息的处理方法


        broad.PlayerChangeScene += G2C_GameStartChangeScene;
    }

    /// <summary>
    /// 开始匹配
    /// </summary>
    /// <param name="info"></param>
    private void StartMatchingManager(NetworkConnection conn, StartMatchingStruct info) 
    {
        comm.PlayerStartMatching(conn);
    }

    /// <summary>
    /// 开始游戏切换场景
    /// </summary>
    private void G2C_GameStartChangeScene(List<NetworkConnection> info) 
    {
        // 向玩家一发送对战信息
        GameStartStruct msg1 = new GameStartStruct() 
        {
            RivalId= info[1].connectionId,
        };

        info[0].Send(msg1, info[0].connectionId);

        // 向玩家二发送对战信息
        GameStartStruct msg2 = new GameStartStruct()
        {
            RivalId = info[0].connectionId,
        };

        info[1].Send(msg2, info[1].connectionId);
    }
}