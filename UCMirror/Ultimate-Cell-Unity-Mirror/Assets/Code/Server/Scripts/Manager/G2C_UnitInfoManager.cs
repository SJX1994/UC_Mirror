using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using static ResManager;

public class G2C_UnitInfoManager : MonoBehaviour
{
    /*[Header("战士单位")]
    public Unit cellUnit;*/

    // Unit ID 管理
    // private int UnitId = 1000;

    // Unit信息整体管理
    private Dictionary<int, UnitInfoClass> unitInfoState = new();

    // 提线整体管理
    private Dictionary<int, PuppetLine> puppetDic = new();

    // Unit新建暂时存储区
    private List<int> NewUnList = new();

    // 游戏房间
    private G2C_PlayerRoom playerRoom;

    // 敌对信息存储 玩家一
    private Dictionary<int, Unit> playerOneUnit = new();

    // 敌对信息存储 玩家二
    private Dictionary<int, Unit> playerTwoUnit = new();

    // Unit整体管理
    // private Dictionary<int, Unit> cellUnits = new();

    // 通讯管理器
    private GameObject sceneLoader;

    public CommunicationInteractionManager CommunicationManager;

    private BroadcastClass broadcastClass;

    // Unit 状态机
    private StateMachineManager stateManager;

    private void Start()
    {
        playerRoom = this.GetComponent<G2C_PlayerRoom>();

        // 暂时获取方式
        if (GameObject.Find("LanNetWorkManager") == null)
        {
            return;
        }
        sceneLoader = GameObject.Find("LanNetWorkManager").gameObject;

        // 全局通信方法管理
        CommunicationManager = sceneLoader.GetComponent<CommunicationInteractionManager>();

        // 全局通信事件注册类
        broadcastClass = sceneLoader.GetComponent<BroadcastClass>();

        // 获取Unit状态方法器
        stateManager = FindObjectOfType<StateMachineManager>();
    }

    /// <summary>
    /// 新建Unit信息
    /// </summary>
    /// <param name="info"></param>
    /// <param name="blocks"></param>
    // public void CreateNewUnit(Dictionary<int, TetrisClass> blocks)
    // {
    //     // 将传输数据转化为 UnitInfoClass
    //     var UnitInfoList = ChangeTetrisIntoUnitInfo(blocks, true);

    //     foreach (UnitInfoClass info in UnitInfoList)
    //     {
    //         if (info.UnitIndexId > 0)
    //         {
    //             CreateCellUnit(info);
    //         }
    //         else
    //         {
    //             CreateVirusUnit(info);
    //         }
    //     }
    // }

    /// <summary>
    /// 新建玩家一 Unit数据
    /// </summary>
    // private void CreateCellUnit(UnitInfoClass info)
    // {
    //     if (!unitInfoState.ContainsKey(info.UnitIndexId))
    //     {
    //         // 根据 位置创建 单位
    //         var unit = stateManager.CreateCellUnit(info);

    //         playerOneUnit.Add(info.UnitIndexId, unit);

    //         unitInfoState.Add(info.UnitIndexId, info);

    //         if (CommunicationManager.ServerState == 1)
    //         {
    //             playerRoom.G2C_SendPuppetCreateInfo(info.UnitIndexId);

    //             var puppetListener = unit.GetComponent<PuppetLineServerListener>();

    //             puppetListener.unitInfoManager = this;
    //         }
    //     }
    // }

    /// <summary>
    /// 新建玩家二 Unit数据
    /// </summary>
    // private void CreateVirusUnit(UnitInfoClass info)
    // {
    //     if (!unitInfoState.ContainsKey(info.UnitIndexId))
    //     {
    //         // 根据 位置创建 单位
    //         var unit = stateManager.CreateVirusUnit(info);

    //         playerTwoUnit.Add(info.UnitIndexId, unit);

    //         unitInfoState.Add(info.UnitIndexId, info);

    //         if (CommunicationManager.ServerState == 1)
    //         {
    //             playerRoom.G2C_SendPuppetCreateInfo(info.UnitIndexId);

    //             var puppetListener = unit.GetComponent<PuppetLineServerListener>();

    //             puppetListener.unitInfoManager = this;
    //         }

    //     }
    // }

    /// <summary>
    /// 更新Unit信息
    /// </summary>
    /// <param name="blocks"></param>
    public void UpdateUnit(List<UnitInfoClass> info)
    {
        List<UnitInfoClass> info1 = new List<UnitInfoClass>();

        List<UnitInfoClass> info2 = new List<UnitInfoClass>();

        foreach (var unit in info)
        {
            if (unit.UnitIndexId > 0)
            {
                info1.Add(unit);
            }
            else
            {
                info2.Add(unit);
            }
        }

        CommunicationManager.UnitInfoAllUpdate(info);

        CommunicationManager.VirusInfoUpdate(info);
    }

    /// <summary>
    /// 将传输数据转化为 UnitInfoClass
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    private List<UnitInfoClass> ChangeTetrisIntoUnitInfo(Dictionary<int, TetrisClass> info, bool CreateInfo)
    {
        List<UnitInfoClass> BlocksReadyUnit = new List<UnitInfoClass>();

        foreach (int key in info.Keys)
        {
            UnitInfoClass unitInfoClass = new();

            unitInfoClass.UnitIndexId = info[key].UnitIndexId;

            unitInfoClass.UnitPos = new Vector2(info[key].posx, info[key].posy);

            if (CreateInfo)
            {
                if (info[key].UnitIndexId > 0)
                {
                    unitInfoClass.UnitPosUse = PositionTools.CountPosShifting(info[key].posx, info[key].posy);
                }
                else
                {
                    unitInfoClass.UnitPosUse = PositionTools.CountPosShifting(info[key].posx, info[key].posy);
                }
            }
            else
            {
                unitInfoClass.UnitPosUse = PositionTools.CountPosShifting(info[key].posx, info[key].posy);

            }

            unitInfoClass.color = info[key].Color;

            unitInfoClass.CreateUnit = true;

            BlocksReadyUnit.Add(unitInfoClass);
        }

        return BlocksReadyUnit;
    }


    /// <summary>
    /// 发送puppet状态更新信息
    /// </summary>
    /// <param name="UnitId"></param>
    /// <param name="pos"></param>
    public void G2C_SendPuppetInfo(int UnitId, G2C_UnitInfoUpdateStruct playerStruct)
    {
        if (CommunicationManager.ServerState != 1) return;

        // 玩家一特殊操作
        {
            playerRoom.netWorkManager.G2C_SendPuppetUpdateInfo(playerStruct, playerRoom.playerOne);
        }

        // 玩家二特殊操作
        {
            playerStruct.UnitId = -UnitId;

            if (playerStruct.PosUpdate)
            {
                var pos = playerStruct.PuppetPosition;

                playerStruct.PuppetPosition = new Vector3(-pos.x + 3, pos.y, pos.z);
            }

            if (playerStruct.FilpUpdate) 
            {
                if (playerStruct.UnitId > 0)
                {
                    playerStruct.PuppetFilp = !playerStruct.PuppetFilp;
                }
            }

            playerRoom.netWorkManager.G2C_SendPuppetUpdateInfo(playerStruct, playerRoom.playerTwo);
        }
    }

    /// <summary>
    /// 发送Puppet固定信息
    /// </summary>
    /// <param name="UnitId"></param>
    /// <param name="playerStruct"></param>
    public void G2C_SendPuppetFixedInfo(int UnitId, G2C_PuppetFixedinformationStruct playerStruct)
    {
        if (CommunicationManager.ServerState != 1) return;

        // 玩家一特殊操作
        {
            playerRoom.netWorkManager.G2C_SendPuppetFixedInfo(playerStruct, playerRoom.playerOne);
        }

        // 玩家二特殊操作
        {
            playerStruct.UnitId = -UnitId;

            if (playerStruct.OnPuppetShooting)
            {
                var pos = playerStruct.PuppetShootingPos;

                playerStruct.PuppetShootingPos = new Vector3(-pos.x + 3, pos.y, pos.z);
            }

            if (playerStruct.OnPuppetStateChanged)
            {
                var pos = playerStruct.PuppetState2;

                playerStruct.PuppetState2 = new Vector3(-pos.x + 3, pos.y, pos.z);

            }

            playerRoom.netWorkManager.G2C_SendPuppetFixedInfo(playerStruct, playerRoom.playerTwo);
        }

    }

    /// <summary>
    /// 获取所有敌人的信息
    /// </summary>
    /// <param name="UnitId"></param>
    public Unit[] GetEnemyUnit(int UnitId)
    {
        if (UnitId > 0)
        {
            return playerTwoUnit.Values.ToArray();
        }
        else
        {
            return playerOneUnit.Values.ToArray();
        }
    }
}