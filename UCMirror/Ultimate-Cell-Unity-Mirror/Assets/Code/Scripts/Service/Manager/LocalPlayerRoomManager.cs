using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalPlayerRoomManager : MonoBehaviour
{
    // 通讯组件
    public G2C_CommunicationManager comm;

    public G2C_BroadcastClass broad;

    // 砖块管理类
    private G2C_TetrisBlocksManager tetris;

    // Unit 信息管理类
    private G2C_UnitInfoManager unitManager;

    // 通信类
    private BroadcastClass broadcastClass;

    // 通信管理器
    private CommunicationInteractionManager CommunicationManager;

    // 客户端通信组件
    // public G2C_NetWorkMessageManager netWorkManager; 

    private void Start()
    {
        // 通信获取
        // 暂时获取方式
        CommunicationManager = GameObject.Find("LanNetWorkManager").gameObject.GetComponent<CommunicationInteractionManager>();

        broadcastClass = CommunicationManager.GetComponent<BroadcastClass>();

        broadcastClass.LocalTetrisCreate += CreateBlocksAndUnit;

        broadcastClass.BuildingUpdateFunc += ChangeBuildingState;

        tetris = this.gameObject.GetComponent<G2C_TetrisBlocksManager>();

        unitManager = this.gameObject.GetComponent<G2C_UnitInfoManager>();

        // netWorkManager = GameObject.Find("NetWorkMessage").GetComponent<G2C_NetWorkMessageManager>();

        Debug.Log(this.name + ": 本地模拟房间新建成功");
    }

    /// <summary>
    /// 新建砖块与Unit
    /// </summary>
    public void CreateBlocksAndUnit(Dictionary<int, TetrisClass> blocksDic)
    {
        // 本地砖块放置
        tetris.BlockSetLocal(blocksDic);

        // 本地新建Unit
        // unitManager.CreateNewUnit(blocksDic);
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
    /// 砖块信息更新方法
    /// </summary>
    /// <param name="info"></param>
    /// <param name="state"></param>
    public void SendTetrisState(List<UnitInfoClass> info, bool state)
    {
        G2C_TetrisUpdateStruct msg = new G2C_TetrisUpdateStruct()
        {
            UpdateClass = info,
            state = state
        };

        // 向玩家一发送砖块更新信息
        {
            // netWorkManager.G2C_SendTetrisState(msg, playerOne);
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

            // netWorkManager.G2C_SendTetrisState(msg, playerTwo);
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

        G2C_TetrisGridUpdateStruct playerOnemsg = new G2C_TetrisGridUpdateStruct();

        playerOnemsg.playerGrid = playerOneList;

        // netWorkManager.G2C_SendGridUpdateInfo(playerOnemsg, playerOne);

        G2C_TetrisGridUpdateStruct playerTwomsg = new G2C_TetrisGridUpdateStruct();

        playerTwomsg.playerGrid = playerTwoList;

        // netWorkManager.G2C_SendGridUpdateInfo(playerTwomsg, playerTwo);
    }

    /// <summary>
    /// 更改建筑状态
    /// </summary>
    /// <param name="info"></param>
    public void ChangeBuildingState(BuildingStateClass info)
    {
        tetris.GetBuildingChangeLocal(info);
    }
}