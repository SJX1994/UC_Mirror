using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UC_PlayerData;
using UnityEngine.Rendering;

[AddComponentMenu("UC_PVP")]
public class NetworkManagerUC_PVP : NetworkManager
{
#region 数据对象
    [Header("UC_PVP:_Static:")]
    public Transform buoyPlayer1Spawn;
    public Transform buoyPlayer2Spawn;
    [Header("UC_PVP_Server:")]
    public BlocksCreator_Main blocksCreator;
    public Transform canvasManager_StayMachine;
    [Header("UC_PVP_Client:")]
    public BuoyInfo buoyPlayer;
    public IdelHolder idelHolderP1;
    public IdelHolder idelHolderP2;
#endregion 数据对象
#region 数据关系
    public override void Awake()
    {
        if(!RunModeCheck())return;
        base.Awake();
        DebugManager.instance.enableRuntimeUI = false;
    }
    public override void OnStartServer()
    {
        if(!RunModeCheck())return;
        Debug.Log("OnStartServer");
        base.OnStartServer();
        // 生成培养皿
        idelHolderP1 = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "IdelHolder_P1")).GetComponent<IdelHolder>();
        idelHolderP2 = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "IdelHolder_P2")).GetComponent<IdelHolder>();
        // 服务器生成培养皿
        NetworkServer.Spawn(idelHolderP1.gameObject);
        NetworkServer.Spawn(idelHolderP2.gameObject);
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
    //     Debug.Log("OnStopClient");
        
    // }
    public override void OnClientConnect()
    {
        if(!RunModeCheck())return;
        Debug.Log("OnClientConnect");
        base.OnClientConnect();
    }
    // public override void OnClientDisconnect()
    // {
    //     if(!RunModeCheck())return;
    //     Debug.Log("OnClientDisconnect");
    // }
  
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if(!RunModeCheck())return;
        Debug.Log("OnServerAddPlayer");
        // 根据连接的顺序选择玩家预制体
        Transform start = numPlayers == 0 ? buoyPlayer1Spawn : buoyPlayer2Spawn;
        GameObject playerObject = Instantiate(playerPrefab,start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, playerObject);
        playerObject.TryGetComponent(out buoyPlayer);
        
        if (numPlayers == 1)
        {
            // 玩家1赋值的 浮标表现
            buoyPlayer.player = Player.Player1;
            buoyPlayer.Active();
            // 不看对方的培养皿
            SetPVPplayer(numPlayers);
            // 生成战场方格测试用
            blocksCreator = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "BlocksManager")).GetComponent<BlocksCreator_Main>();
            NetworkServer.Spawn(blocksCreator.gameObject);
        }
        else if (numPlayers == 2)
        {
            
            // 玩家2赋值的 浮标表现
            buoyPlayer.player = Player.Player2;
            buoyPlayer.Active();
            // 生成战场方格
            blocksCreator = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "BlocksManager")).GetComponent<BlocksCreator_Main>();
            NetworkServer.Spawn(blocksCreator.gameObject);
            // 生成Canvas管理系统
            canvasManager_StayMachine = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "CanvasManager_StayMachine")).transform;
            NetworkServer.Spawn(canvasManager_StayMachine.gameObject);
            // 不看对方的培养皿
            SetPVPplayer(numPlayers);
            
        }
        Debug.Log(numPlayers);
    } 
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if(blocksCreator) NetworkServer.Destroy(blocksCreator.gameObject);
        if(canvasManager_StayMachine) NetworkServer.Destroy(canvasManager_StayMachine.gameObject);
        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }
#endregion 数据关系
#region 数据操作
    public void SetPVPplayer(int player)
    {
        if(player == 1)
        {
            idelHolderP1.playerPVP = Player.Player1;
            idelHolderP2.playerPVP = Player.Player1;
        }
        else if(player == 2)
        {
            idelHolderP1.playerPVP = Player.Player2;
            idelHolderP2.playerPVP = Player.Player2;
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
}
