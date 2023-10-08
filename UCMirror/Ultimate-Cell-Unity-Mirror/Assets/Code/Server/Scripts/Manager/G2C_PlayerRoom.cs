using Mirror;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class G2C_PlayerRoom : MonoBehaviour 
{
    // 玩家一id
    public NetworkConnection playerOne;

    // 玩家二id
    public NetworkConnection playerTwo;

    // 通讯组件
    public G2C_CommunicationManager comm;

    public G2C_BroadcastClass broad;

    // 砖块管理类
    private G2C_TetrisBlocksManager tetris;

    // Unit 信息管理类
    private G2C_UnitInfoManager unitManager;

    // 客户端通信组件
    public G2C_NetWorkMessageManager netWorkManager;

    // 通信类
    private BroadcastClass broadcastClass;

    // 通信管理器
    private CommunicationInteractionManager CommunicationManager;

    private void Start()
    {
        // 通信获取
        // 暂时获取方式
        CommunicationManager = GameObject.Find("LanNetWorkManager").gameObject.GetComponent<CommunicationInteractionManager>();

        broadcastClass = CommunicationManager.GetComponent<BroadcastClass>();

        broadcastClass.SendTetrisUpdateToClient += SendTetrisState;

        broadcastClass.G2C_GridInfoUpdate += SendGridUpdateState;

        tetris = this.gameObject.GetComponent<G2C_TetrisBlocksManager>();

        unitManager = this.gameObject.GetComponent<G2C_UnitInfoManager>();

        netWorkManager = GameObject.Find("NetWorkMessage").GetComponent<G2C_NetWorkMessageManager>();

        Debug.Log(this.name + "Battlefield control !!!");

        Debug.Log("PlayerOne: " + playerOne.connectionId + " && " + "PlayerTwo: " + playerTwo.connectionId);

        RoomCreateChangeScene();
    }

    /// <summary>
    /// 房间加载完成切换场景
    /// </summary>
    private void RoomCreateChangeScene() 
    {
        comm.PlayerChangeScene(new List<NetworkConnection>() { playerOne , playerTwo });
    }

    /// <summary>
    /// 新建砖块与Unit
    /// </summary>
    public void CreateBlocksAndUnit(G2C_BlocksMessageClass info) 
    {
        Debug.Log("Create Blocks : " + info.ConnId);

        // 判断是哪个玩家发来了信息
        var player = JudgePlayer(info);

        // 玩家一逻辑
        if (player.connectionId == playerOne.connectionId)
        {
            // 将通讯数据转化为字典数据
            var blocksDic = this.ChangeListIntoDic(info.BlocksInfo.blocksInfo, true);

            // 检查砖块摆放是否符合规范
            if (tetris.BlocksSet(blocksDic))
            {
                // 返回消息不合规
                Debug.Log("Blocks dont Match:" + info.ConnId);

                // 返回ConnId 砖块不合规
            }

            // 根据砖块信息生成Unit
            // unitManager.CreateNewUnit(blocksDic);

            // 将砖块数据镜像翻转
            var blocksList = tetris.BlocksMirrorSet(blocksDic);

            // 向另一个玩家发送镜像砖块新建数据，发送端玩家砖块自己新建
            this.G2C_SendCreateBlocksMessage(player, blocksList);

            // 再次翻转值准备存储
            tetris.BlocksMirrorSet(blocksDic);
        }
        // 玩家二逻辑
        else
        {
            // 将通讯数据转化为字典数据
            var blocksDic = this.ChangeListIntoDic(info.BlocksInfo.blocksInfo, false);

            // 检查砖块摆放是否符合规范 已完成砖块镜像翻转
            if (tetris.BlocksSet(blocksDic))
            {
                // 返回消息不合规
                Debug.Log("Blocks dont natch:" + info.ConnId);

                // 返回ConnId 砖块不合规
            }

            // 根据砖块信息生成Unit
            // unitManager.CreateNewUnit(blocksDic);

            // 向另一个玩家发送镜像砖块新建数据，发送端玩家砖块自己新建
            this.G2C_SendCreateBlocksMessage(player, blocksDic.Values.ToList());
        }
    }

    /// <summary>
    /// 将数据传输过程中的List转化为Dic
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    private Dictionary<int, TetrisClass> ChangeListIntoDic(List<TetrisClass> info, bool playerInfo)
    {

        Dictionary<int, TetrisClass> blockinfo = new Dictionary<int, TetrisClass>();

        // player One 与 Player Two的ID进行区分
        if (playerInfo)
        {
            foreach (var block in info)
            {
                blockinfo.Add(block.UnitIndexId, block);
            }

        }
        else
        {
            foreach (var block in info)
            {
                block.UnitIndexId = -block.UnitIndexId;

                blockinfo.Add(block.UnitIndexId, block);
            }

        }

        return blockinfo;

    }

    /// <summary>
    /// 发送玩家新建砖块数据
    /// </summary>
    private void G2C_SendCreateBlocksMessage(NetworkConnection player, List<TetrisClass> blocks) 
    {
        // 生成返回值通讯类
        G2C_OpponentBricksClass msg = new G2C_OpponentBricksClass();

        msg.fromplayer = player;

        msg.sendPlayer = player == playerOne? playerTwo : playerOne;

        msg.tetrisClaass = blocks;

        comm.SendBlockCreateMessage(msg);
    }

    /// <summary>
    /// 判断玩家
    /// </summary>
    /// <param name="info"></param>
    private NetworkConnection JudgePlayer(G2C_BlocksMessageClass info)
    {
        // 判断数据是否为玩家一发送数据
        if (info.ConnId == playerOne.connectionId)
        {
            return playerOne;

        }
        else
        {
            return playerTwo;
        }

    }

    /// <summary>
    /// 发送木偶新建信息
    /// </summary>
    /// <param name="UnitId"></param>
    public void G2C_SendPuppetCreateInfo(int UnitId) 
    {
        PuppetUnitStateStruct player1msg = new PuppetUnitStateStruct() 
        {
            UnitId= UnitId,
            UnitState = 0
        };

        netWorkManager.G2C_SendPuppetCteateInfo(player1msg, playerOne);

        PuppetUnitStateStruct player2msg = new PuppetUnitStateStruct()
        {
            UnitId = -UnitId,
            UnitState = 0
        };

        netWorkManager.G2C_SendPuppetCteateInfo(player2msg, playerTwo);
    }

    /// <summary>
    /// 砖块信息更新方法
    /// </summary>
    /// <param name="info"></param>
    /// <param name="state"></param>
    public void SendTetrisState(List<UnitInfoClass> info, bool state) 
    {
        G2C_TetrisUpdateStruct msg = new G2C_TetrisUpdateStruct() 
        {
            UpdateClass = info,
            state= state
        };

        // 向玩家一发送砖块更新信息
        {
            netWorkManager.G2C_SendTetrisState(msg, playerOne);
        }

        // 向玩家二发送砖块更新信息
        {
            List<UnitInfoClass> playerTwoList = new List<UnitInfoClass>();

            foreach (UnitInfoClass unitInfo in info)
            {
                UnitInfoClass add = new UnitInfoClass()
                {
                    UnitIndexId = -unitInfo.UnitIndexId,
                    UnitPos = unitInfo.UnitPos,
                    UnitPosUse = unitInfo.UnitPosUse,
                    CreateUnit = unitInfo.CreateUnit,
                    color = unitInfo.color,
                    UnitLevel = unitInfo.UnitLevel
                };

                var infox = add.UnitPos.x;

                add.UnitPos.x = (19 - infox * 2) + infox;

                playerTwoList.Add(add);
            }

            msg.UpdateClass = playerTwoList;

            netWorkManager.G2C_SendTetrisState(msg, playerTwo);
        }
    }

    /// <summary>
    /// 碰撞管理器更新事件
    /// </summary>
    /// <param name="grid"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void SendGridUpdateState(string[,] grid)
    {
        List<TetrisGridUpdateClass> playerOneList = new();

        List<TetrisGridUpdateClass> playerTwoList = new();

        var mirrorGrid = new string[20, 10];

        // 丑陋！！！ 有时间改了！！！
        for (int row = 0; row < 20; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                if (grid[row, col] != null)
                {
                    mirrorGrid[19 - row, col] = grid[row, col];

                    TetrisGridUpdateClass info = new TetrisGridUpdateClass();

                    info.tetrisId = grid[row, col];

                    info.row = row;

                    info.col = col;

                    playerOneList.Add(info);
                }
            }
        }

        for (int row = 0; row < 20; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                if (mirrorGrid[row, col] != null)
                {
                    TetrisGridUpdateClass info = new TetrisGridUpdateClass();

                    info.tetrisId = mirrorGrid[row, col];

                    info.row = row;

                    info.col = col;

                    playerTwoList.Add(info);

                }
            }
        }

        G2C_TetrisGridUpdateStruct playerOnemsg = new G2C_TetrisGridUpdateStruct() ;

        playerOnemsg.playerGrid = playerOneList;

        netWorkManager.G2C_SendGridUpdateInfo(playerOnemsg, playerOne);

        G2C_TetrisGridUpdateStruct playerTwomsg = new G2C_TetrisGridUpdateStruct();

        playerTwomsg.playerGrid = playerTwoList;

        netWorkManager.G2C_SendGridUpdateInfo(playerTwomsg, playerTwo);
    }

    /// <summary>
    /// 更改建筑状态
    /// </summary>
    /// <param name="info"></param>
    public void ChangeBuildingState(BuildingStateClass info) 
    {
        tetris.GetBuildingChangeState(info);
    }
}