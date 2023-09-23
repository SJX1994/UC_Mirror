using Common;
using I18N.Common;
using Mirror;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using static StartCheckManager;

public class LanNetWorkManager : NetworkBehaviour
{
    private BroadcastClass broadcastClass;

    private CommunicationInteractionManager CommunicationManager;

    [Header("网络连接组件")]
    public NetworkManager manager;

    public string NetWorkAddress = "192.168.1.18";

    private void Start()
    {
        DontDestroyOnLoad(this);
        // 通信获取
        // 暂时获取方式
        CommunicationManager = this.GetComponent<CommunicationInteractionManager>();

        broadcastClass = this.GetComponent<BroadcastClass>();

        broadcastClass.MessageSendTest += SendCustomMessageToServer;

        broadcastClass.StrartCheck += C2G_SendLoginMessageToServer;

        broadcastClass.StartMatching += C2G_StratMatching;

        broadcastClass.C2G_SendBlocksInfo += C2G_SendBlocksInfo;

        broadcastClass.BuildingUpdateFunc += C2G_BuildingUpdateFunc;

        broadcastClass.StartGameButtonAction += StartGame;

        // 接收链接成功的信息
        NetworkClient.RegisterHandler<ConnectionStruct>(G2C_ConnectionSuccess);

        // 接收测试数据结构体信息
        NetworkClient.RegisterHandler<StructClassInfo>(OnClientMessageReceived);

        // 接收登陆版本验证结构体信息
        NetworkClient.RegisterHandler<StartCheckClass>(G2C_StartCheckBack);

        // 开始游戏切换场景
        NetworkClient.RegisterHandler<GameStartStruct>(G2C_StartGame);

        // 接收砖块新建信息
        NetworkClient.RegisterHandler<OpponentBricksStruct>(G2C_GetBlockInfoCreate);

        // 接收砖块更新信息
        NetworkClient.RegisterHandler<G2C_TetrisUpdateStruct>(G2C_TetrisUpdateStruct);

        // 接收碰撞盒更新信息
        NetworkClient.RegisterHandler<G2C_TetrisGridUpdateStruct>(G2C_TetrisGridUpdateStruct);

        // 接收新建木偶信息
        NetworkClient.RegisterHandler<PuppetUnitStateStruct>(G2C_PuppetUnitStateStruct);

        // 接收魔偶固定信息更新
        NetworkClient.RegisterHandler<G2C_PuppetFixedinformationStruct>(G2C_PuppetUnitFixedInfoUpdate);

        // 接收木偶位置更新信息
        NetworkClient.RegisterHandler<G2C_UnitInfoUpdateStruct>(G2C_PuppetUnitUpdatePos);

        // 接收初始化建筑位置
        NetworkClient.RegisterHandler<CreateBuildingStruct>(G2C_CreateBuildingStruct);
    }

    /// <summary>
    /// 开始游戏按钮
    /// </summary>
    /// <param name="info"></param>
    void StartGame(int info)
    {
        // 连接服务器
        // TODO 携程进行连接，并加载其他动画
        OnJoinServer();
    }

    /// <summary>
    /// 开始连接服务器
    /// </summary>
    /// <param name="info"></param>
    void OnJoinServer()
    {
        try
        {
            manager.networkAddress = NetWorkAddress;
            Debug.Log("Start jion the Server: " + manager.networkAddress);
            manager.StartClient();

        }
        catch (SocketException e)
        {
            CommunicationManager.ServerStoped(0);

            Debug.Log(e);
        }
    }

    /// <summary>
    /// 客户端发送消息通知给服务器
    /// </summary>
    /// <param name="info"></param>
    public void SendCustomMessageToServer(int info)
    {
        StructClassInfo msg = new()
        {
            myInt = 42,
            myString = "Hello Server",
            // 设置其他需要传输的字段
        };

        Debug.Log("Message Send !!!");

        NetworkClient.Send(msg); // 消息测试
    }

    /// <summary>
    /// 客户端接收服务器的消息通知
    /// </summary>
    /// <param name="msg"></param>
    void OnClientMessageReceived(StructClassInfo msg)
    {
        Debug.Log("客户端收到消息：" + msg.myString);
    }

    /// <summary>
    ///  服务器连接成功返回值
    /// </summary>
    /// <param name="msg"></param>
    void G2C_ConnectionSuccess(ConnectionStruct msg)
    {
        Debug.Log("Join Server Success !!!");

        CommunicationManager.StartCheck(0);
    }

    /// <summary>
    /// 向服务器发送版本验证信息
    /// </summary>
    public void C2G_SendLoginMessageToServer(StartCheckClass startcheck)
    {
        Debug.Log("Strat Message Send !!!");

        NetworkClient.Send(startcheck);
    }


    /// <summary>
    /// 接收服务器返回的登陆版本验证信息
    /// </summary>
    /// <param name="msg"></param>
    void G2C_StartCheckBack(StartCheckClass msg)
    {
        if (msg.StartCheckBool)
        {
            // 开始登陆验证
            CommunicationManager.G2C_StratCheckReturn(1);
        }
        else
        {
            // 提示下载最新版本
            CommunicationManager.G2C_StratCheckReturn(0);
        }
    }

    /// <summary>
    /// 开始匹配
    /// </summary>
    /// <param name="info"></param>
    void C2G_StratMatching(int info)
    {
        Debug.Log("Start Matching !!!");

        StartMatchingStruct msg = new()
        {
            matching = 1,
        };

        NetworkClient.Send(msg);
    }

    /// <summary>
    /// 开始游戏切换场景
    /// </summary>
    /// <param name="msg"></param>
    void G2C_StartGame(GameStartStruct msg)
    {
        CommunicationManager.StartGamePVP(0);

        Debug.Log("Message Get Create fight Scene");
    }

    /// <summary>
    /// 发送砖块信息
    /// </summary>
    void C2G_SendBlocksInfo(Dictionary<int, TetrisClass> info)
    {
        List<TetrisClass> blocksList = new List<TetrisClass>();

        foreach (TetrisClass classinfo in info.Values)
        {
            blocksList.Add(classinfo);
        }

        BlocksInfoStruct msg = new()
        {
            blocksInfo = blocksList
        };

        NetworkClient.Send(msg);

        Debug.Log("Blocks Message Send !!!");
    }

    /// <summary>
    /// 接收砖块信息
    /// </summary>
    /// <param name="msg"></param>
    void G2C_GetBlockInfoCreate(OpponentBricksStruct msg)
    {
        Debug.Log("Get Blocks Message from : " + msg.fromplayer);

        CommunicationManager.GetOpponBlocks(msg.tetrisClaass);
    }

    /// <summary>
    /// 接收木偶状态信息
    /// </summary>
    void G2C_PuppetUnitStateStruct(PuppetUnitStateStruct msg)
    {
        if (msg.UnitState == 0)
        {
            Debug.Log("Get Message from Server: Create Puppet" + msg.UnitId);

            CommunicationManager.CreatePuppet(msg.UnitId);
        }
    }

    /// <summary>
    /// 接收木偶固定信息更新
    /// </summary>
    /// <param name="msg"></param>
    void G2C_PuppetUnitFixedInfoUpdate(G2C_PuppetFixedinformationStruct msg)
    {
        Debug.Log(msg.UnitId + ": 木偶固定信息接收成功");

        PuppetFixedInfoClass info = new PuppetFixedInfoClass() 
        {
            UnitId = msg.UnitId,
            OnPuppetAttacking = msg.OnPuppetAttacking,
            OnPuppetAttackFinish = msg.OnPuppetAttackFinish,
            OnPuppetShooting = msg.OnPuppetShooting,
            OnPuppetSide = msg.OnPuppetSide,
            OnPuppetStateChanged = msg.OnPuppetStateChanged,
            OnPuppetTypeChanged = msg.OnPuppetTypeChanged,
            OnPuppetSkinChanged = msg.OnPuppetSkinChanged,
            OnPuppetDestory = msg.OnPuppetDestory,
            OnPuppetChangeWeapon = msg.OnPuppetChangeWeapon,
            PuppetAttacking = msg.PuppetAttacking,
            PuppetAttackFinish = msg.PuppetAttackFinish,
            isPuppetShooting = msg.isPuppetShooting,
            PuppetShootingPos = msg.PuppetShootingPos,
            PuppetSide = msg.PuppetSide,
            PuppetState1 = msg.PuppetState1,
            PuppetState2 = msg.PuppetState2,
            PuppetTypeChanged = msg.PuppetTypeChanged,
            PuppetSkinChanged = msg.PuppetSkinChanged,
            PuppetDestory = msg.PuppetDestory,
            PuppetChangeWeapon = msg.PuppetChangeWeapon
        };

        CommunicationManager.UpdatePuppetFixedInfo(info);
    }

    /// <summary>
    /// 接收木偶位置信息
    /// </summary>
    /// <param name="msg"></param>
    void G2C_PuppetUnitUpdatePos(G2C_UnitInfoUpdateStruct msg)
    {
        UnitInfoUpdateStruct info = new UnitInfoUpdateStruct()
        {
            UnitId = msg.UnitId,
            PosUpdate = msg.PosUpdate,
            ScaleUpdate = msg.ScaleUpdate,
            HealthUpdate = msg.HealthUpdate,
            FilpUpdate = msg.FilpUpdate,
            SpeedUpdate = msg.SpeedUpdate,
            AttackPosUpdate = msg.AttackPosUpdate,
            PuppetPosition = msg.PuppetPosition,
            PuppetScale = msg.PuppetScale,
            PuppetHealth = msg.PuppetHealth,
            PuppetFilp = msg.PuppetFilp,
            PuppetSpeed = msg.PuppetSpeed,
            PuppetAttackPosition = msg.PuppetAttackPosition,
        };

        CommunicationManager.UpdatePuppetInfo(info);
    }

    /// <summary>
    /// 向服务器发送Building状态变更信息
    /// </summary>
    /// <param name="info"></param>
    void C2G_BuildingUpdateFunc(BuildingStateClass info)
    {
        Debug.Log("Building Change : " + info.BuildingPosX + " " + info.BuildingPosY + " " + info.FunctionHitState);

        BuildingStatestruct msg = new BuildingStatestruct();

        msg.BuildingPos = new Vector2(info.BuildingPosX, info.BuildingPosY);

        if (info.FunctionHitState == 1)
        {
            msg.FunctionHit = true;
        }
        else if (info.FunctionHitState == 2)
        {
            msg.FunctionExit = true;
        }

        NetworkClient.Send(msg);
    }

    /// <summary>
    /// 初始化接收建筑位置
    /// </summary>
    /// <param name="info"></param>
    void G2C_CreateBuildingStruct(CreateBuildingStruct info)
    {
        // TODO 切换场景界面更新
    }

    /// <summary>
    /// 接收砖块更新信息
    /// </summary>
    /// <param name="info"></param>
    void G2C_TetrisUpdateStruct(G2C_TetrisUpdateStruct info)
    {
        G2C_TetrisUpdateClass infoClass = new G2C_TetrisUpdateClass()
        {
            UpdateClass = info.UpdateClass,
            state = info.state,
        };

        CommunicationManager.G2C_TetrisUpdateFunc(infoClass);
    }

    /// <summary>
    /// 接收碰撞盒更新信息
    /// </summary>
    /// <param name="info"></param>
    void G2C_TetrisGridUpdateStruct(G2C_TetrisGridUpdateStruct info)
    {
        string[,] newGrid = new string[20, 10];

        foreach (TetrisGridUpdateClass infoClass in info.playerGrid)
        {
            newGrid[infoClass.row, infoClass.col] = infoClass.tetrisId;
        }

        CommunicationManager.G2C_GridInfoUpdate(newGrid);
    }
}
