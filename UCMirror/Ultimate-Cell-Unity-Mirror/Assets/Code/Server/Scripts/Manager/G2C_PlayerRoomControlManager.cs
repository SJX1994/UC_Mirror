using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class G2C_PlayerRoomControlManager : MonoBehaviour 
{
    [Header("服务器通讯组件")]
    public G2C_CommunicationManager comm;

    public G2C_BroadcastClass broad;

    [Header("玩家房间")]
    public GameObject PlayerRoom;

    // 玩家大厅
    private Dictionary<int, int> PlayerInfo = new Dictionary<int, int>();

    // 匹配池
    private Stack<NetworkConnection> MatchStack = new Stack<NetworkConnection>();

    // 玩家所在房间
    private Dictionary<int, int> PlayerRoomInfo= new Dictionary<int, int>();

    // 房间脚本管理
    private Dictionary<int, G2C_PlayerRoom> playerRoomScript = new Dictionary<int, G2C_PlayerRoom>();

    // 房间ID管理
    private int PlayerRoomId = 100;

    private void Start()
    {
        broad.PlayerAddRoom += PlayerAddRoom;

        broad.PlayerStartMatching += PlayerMatching;

        broad.GetBlocksInfo += GetClientBlocksMessage;

        broad.GetBuildingStateFromClient += GetBuildingStateFromClient;
    }

    private void Update()
    {
        // 玩家匹配成功
        if (MatchStack.Count >= 2) 
        {
            List<NetworkConnection> list = new List<NetworkConnection>();

            for (int i = 0; i < 2; i++)
            {
                NetworkConnection conn = MatchStack.Pop();

                list.Add(conn);

            }

            PlayerRoomCreate(list);
        }
    }

    /// <summary>
    /// 玩家加入游戏大厅
    /// </summary>
    /// <param name="id"></param>
    private void PlayerAddRoom(int id) 
    {
        if (!PlayerInfo.ContainsKey(id)) 
        {
            PlayerInfo.Add(id, 0);
        }
    }

    /// <summary>
    /// 玩家离开游戏大厅
    /// </summary>
    /// <param name="id"></param>
    private void PlayerLeaveRoom(int id) 
    {
        if (PlayerInfo.ContainsKey(id)) 
        {
            PlayerInfo.Remove(id);
        }
    }

    /// <summary>
    /// 玩家开始匹配
    /// </summary>
    /// <param name="id"></param>
    private void PlayerMatching(NetworkConnection conn) 
    {
        // 玩家跳出游戏大厅
        PlayerLeaveRoom(conn.connectionId);

        // 玩家开始匹配
        MatchStack.Push(conn);
    }

    /// <summary>
    /// 新建游戏房间
    /// </summary>
    /// <param name="PlayerId"></param>
    private void PlayerRoomCreate(List<NetworkConnection> PlayerId)
    {
        // 新建脚本游戏物体
        var playerRoom = GameObject.Instantiate(PlayerRoom, this.gameObject.transform) as GameObject;

        // 设置游戏物体名称
        playerRoom.name = "playerRoom" + PlayerRoomId;

        // 设置玩家一
        playerRoom.GetComponent<G2C_PlayerRoom>().playerOne = PlayerId[0];

        // 设置玩家二
        playerRoom.GetComponent<G2C_PlayerRoom>().playerTwo = PlayerId[1];

        // 将脚本加入房间管理
        playerRoomScript.Add(PlayerRoomId, playerRoom.GetComponent<G2C_PlayerRoom>());

        // 将玩家加入房间管理
        PlayerRoomInfo.Add(PlayerId[0].connectionId, PlayerRoomId);

        // 将玩家加入房间管理
        PlayerRoomInfo.Add(PlayerId[1].connectionId, PlayerRoomId);

        // 房间Id自增
        PlayerRoomId++;

        // 启动脚本
        playerRoom.SetActive(true);

        Debug.Log("Player Room Create !!!" + playerRoom.name);
    }

    /// <summary>
    /// 获取客户端发来的砖块消息进行统一管理
    /// </summary>
    /// <param name="info"></param>
    void GetClientBlocksMessage(G2C_BlocksMessageClass info) 
    {
        // 根据玩家Id获取房间Id
        var playerId = info.ConnId;

        var roomId = PlayerRoomInfo.ContainsKey(playerId)? PlayerRoomInfo[playerId]: -1;

        if (roomId == -1) 
        {
            return;
        }

        // 根据房间Id找到房间脚本
        var RoomScript = playerRoomScript.ContainsKey(roomId)? playerRoomScript[roomId]: null;

        if (RoomScript == null) { return; }

        // 运行砖块的生成
        RoomScript.CreateBlocksAndUnit(info);
    }

    /// <summary>
    /// 获取从客户端获取的建筑信息统一管理
    /// </summary>
    /// <param name="info"></param>
    void GetBuildingStateFromClient(BuildingStateClass info)
    {
        // 根据玩家Id获取房间Id
        var playerId = info.conn.connectionId;

        var roomId = PlayerRoomInfo.ContainsKey(playerId) ? PlayerRoomInfo[playerId] : -1;

        if (roomId == -1)
        {
            return;
        }

        // 根据房间Id找到房间脚本
        var RoomScript = playerRoomScript.ContainsKey(roomId) ? playerRoomScript[roomId] : null;

        if (RoomScript == null) { return; }

        // 运行砖块的生成
        RoomScript.ChangeBuildingState(info);

    }
}