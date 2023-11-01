using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UC_PlayerData;
using UnityEngine.Rendering;

[AddComponentMenu("UC_PVP")]
public class NetworkManagerUC_PVP : NetworkManager
{
#region 联网数据对象
    [Header("UC_PVP:_Static:")]
    public Transform buoyPlayer1Spawn;
    public Transform buoyPlayer2Spawn;
    [Header("UC_PVP_Server:")]
    public BlocksCreator_Main blocksCreator;
    public AvatarUI_Main avatarUI;
    public BlocksUI blocksUI;
    public Transform canvasManager_StayMachine;
    [Header("UC_PVP_Client:")]
    public BuoyInfo buoyPlayer;
    public IdelHolder idelHolderP1;
    public IdelHolder idelHolderP2;
#endregion 联网数据对象
#region 联网数据关系
    public override void Awake()
    {
        if(!RunModeCheck())return;
        base.Awake();
        DebugManager.instance.enableRuntimeUI = false;
    }
    public override void OnStartServer()
    {
        if(!RunModeCheck())return;
        // Debug.Log("OnStartServer");
        base.OnStartServer();
        AvatarUIStartLoad();
        BlocksUIStartLoad();
    }
    // public override void OnStopServer()
    // {
    //     if(!RunModeCheck())return;
    //     Debug.Log("OnStopServer");
        
    // }
    // public override void OnStartClient()
    // {
    //     if(!RunModeCheck())return;
    //     Debug.Log("OnStartClient");
        
    // }
    // public override void OnStopClient()
    // {
    //     if(!RunModeCheck())return;
    //     Debug.Log("OnStopClient!");
    // }
    // public override void OnClientConnect()
    // {
    //     if(!RunModeCheck())return;
    //     // Debug.Log("OnClientConnect");
    //     base.OnClientConnect();
    // }
    // public override void OnClientDisconnect()
    // {
    //     if(!RunModeCheck())return;
    //     Debug.Log("OnClientDisconnect");
    //     NetworkServer.Shutdown();
    //     StopServer();
    // }
  
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if(!RunModeCheck())return;
        
        // 根据连接的顺序选择玩家预制体
        Transform start = numPlayers == 0 ? buoyPlayer1Spawn : buoyPlayer2Spawn;
        GameObject playerObject = Instantiate(playerPrefab,start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, playerObject);
        playerObject.TryGetComponent(out buoyPlayer);
        
        if (numPlayers == 1)
        {
            // 玩家1入场测试 应当玩家2入场时再生成
            // MapResourceStartLoad(); 
            bool loadTetrominos = false;
            PetriDishStartLoad_P1(loadTetrominos);
            PetriDishStartLoad_P2(loadTetrominos);
            Set_Buoy_Player(numPlayers);
            ResetPlayerStaticData(Player.Player1);
        }
        else if (numPlayers == 2)
        {
            MapResourceStartLoad();
            bool loadTetrominos = true;
            PetriDishStartLoad_P1(loadTetrominos);
            PetriDishStartLoad_P2(loadTetrominos);
            Set_Buoy_Player(numPlayers);
            ResetPlayerStaticData(Player.Player2);
            Invoke(nameof(Set_Visible_Of_IdelHolder),1.0f);
            BlocksUIActive();
            
        }
        Debug.Log("OnServerAddPlayer" + numPlayers);
    } 
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.Log("玩家掉线");
        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
        
        
        // 重新启动服务器
        StopServer();
        // StartServer();
    }
#endregion 联网数据关系
#region 数据操作
    void ResetPlayerStaticData(Player player)
    {
        // Debug.Log("ResetPlayerStaticData!!");
        if(player == Player.Player1)
        {
            UserAction.Player1UserState = UserAction.State.Loading;
        }
        else if(player == Player.Player2)
        {
            UserAction.Player2UserState = UserAction.State.Loading;
        }
    }
    public void Set_Buoy_Player(int playerAdded)
    {
        if(playerAdded == 1)
        {
            // 玩家1赋值的 浮标表现
            buoyPlayer.player = Player.Player1;
            buoyPlayer.Active();
            idelHolderP1.playerPVP_Temp = Player.Player1;
            if(!idelHolderP2) return;
            idelHolderP2.playerPVP_Temp = Player.Player1;
        }
        else if(playerAdded == 2)
        {
            // 玩家2赋值的 浮标表现
            buoyPlayer.player = Player.Player2;
            buoyPlayer.Active();
            idelHolderP2.playerPVP_Temp = Player.Player2;
            if(!idelHolderP1) return;
            idelHolderP1.playerPVP_Temp = Player.Player2;
            
        }
    }
    bool RunModeCheck()
    {
        if(RunModeData.CurrentRunMode!=RunMode.Host)
        {
            transform.GetComponent<NetworkManagerHUD>().enabled = false;
            transform.GetComponent<Transport>().enabled = false;
            this.enabled = false;
            return false;
        }
        return true;
    }
#endregion 数据操作
#region 联网数据操作
    void Set_Visible_Of_IdelHolder()
    {
       
        idelHolderP1.Client_HideOther();
        idelHolderP2.Client_HideOther();
    }
    void PetriDishStartLoad_P1(bool loadTetrominos)
    {
        if(!loadTetrominos)
        {
            // 生成培养皿
            idelHolderP1 = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "IdelChest_P1")).GetComponent<IdelHolder>();
            // 服务器生成培养皿
            NetworkServer.Spawn(idelHolderP1.gameObject);
        }else
        {
            if(!idelHolderP1)
            {
                // 生成培养皿
                idelHolderP1 = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "IdelChest_P1")).GetComponent<IdelHolder>();
                // 服务器生成培养皿
                NetworkServer.Spawn(idelHolderP1.gameObject);
            }
            IdelBox idelBox = idelHolderP1.idelUI.BoxInfo[0].GetComponent<IdelBox>();
            idelBox.Init(loadTetrominos);
        }
        
        
    }
    void PetriDishStartLoad_P2(bool loadTetrominos)
    {
        if(!loadTetrominos)
        {
            // 生成培养皿
            idelHolderP2 = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "IdelChest_P2")).GetComponent<IdelHolder>();
            // 服务器生成培养皿
            NetworkServer.Spawn(idelHolderP2.gameObject);
        }else
        {
            if(!idelHolderP2)
            {
                // 生成培养皿
                idelHolderP2 = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "IdelChest_P2")).GetComponent<IdelHolder>();
                // 服务器生成培养皿
                NetworkServer.Spawn(idelHolderP2.gameObject);
            }
            IdelBox idelBox = idelHolderP2.idelUI.BoxInfo[0].GetComponent<IdelBox>();
            idelBox.Init(loadTetrominos);
        }
        
    }
    void AvatarUIStartLoad()
    {
        avatarUI = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "AvatarsUI")).GetComponent<AvatarUI_Main>();
        NetworkServer.Spawn(avatarUI.gameObject);
    }
    void BlocksUIStartLoad()
    {
        blocksUI = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "BlocksUI")).GetComponent<BlocksUI>();
        NetworkServer.Spawn(blocksUI.gameObject);
    }
    void MapResourceStartLoad()
    {
        // 生成战场方格
        blocksCreator = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "BlocksManager")).GetComponent<BlocksCreator_Main>();
        NetworkServer.Spawn(blocksCreator.gameObject);
    }
    void BlocksUIActive()
    {
        if(!blocksCreator)MapResourceStartLoad();
        blocksCreator.BlocksUIActive();
    }
#endregion 联网数据操作
}
