using UnityEngine;
using System.Collections.Generic;
using UC_PlayerData;
using Mirror;
using System.Linq;
public class BlocksProps : NetworkBehaviour
{
#region 数据对象
    int obstacleCount = 15; // 满格20格
    TetriObstacle tetriObstacle;
    public TetriObstacle TetriObstacle
    {
        get{
            if(!tetriObstacle)tetriObstacle = Resources.Load<TetriObstacle>("Props/ObstacleTetromino");
            return tetriObstacle;
        }
    }
    TetriBall tetriBall;
    public TetriBall TetriBall
    {
        get{
            if(!tetriBall)tetriBall = Resources.Load<TetriBall>("Props/BallTetromino");
            return tetriBall;
        }
    }
    TetriMoveDirection tetriMoveDirection;
    public TetriMoveDirection TetriMoveDirection
    {
        get{
            if(!tetriMoveDirection)tetriMoveDirection = Resources.Load<TetriMoveDirection>("Props/MoveDirectionTetromino");
            return tetriMoveDirection;
        }
    }
    // Player ChainBallSwitcher = Player.NotReady;
    Player MoveDirectionChangerSwitcher;
#endregion 数据对象
#region  联网数据对象
    NetworkManagerUC_PVP networkManagerUC_PVP;
    NetworkManagerUC_PVP NetworkManagerUC_PVP
    {
        get{
            if(!isServer)return null;
            if(!networkManagerUC_PVP)networkManagerUC_PVP = FindObjectOfType<NetworkManagerUC_PVP>();
            return networkManagerUC_PVP;
        }
    }
#endregion 联网数据对象
#region 数据关系
    void Start()
    {
        MoveDirectionChangerSwitcher = Player.Player1;
    }
#endregion 数据关系
#region 数据操作
    public void Generate(PropsData.PropsState propsState)
    {
        switch(propsState)
        {
            case PropsData.PropsState.ChainBall:
                Invoke(nameof(GenerateChainBall_First),9.1f);
            break;
            case PropsData.PropsState.MoveDirectionChanger:
                Invoke(nameof(GenerateMoveDirectionChanger),1.1f);
                Invoke(nameof(GenerateMoveDirectionChanger),2.1f);
            break;
            case PropsData.PropsState.Obstacle:
                Invoke(nameof(GenerateObstacle),0.5f);
            break;
        }
    }
    void GenerateObstacle()
    {
        if(Local())
        {
            if(obstacleCount == 0)return;
            tetriObstacle = Instantiate(TetriObstacle);
            LocalMode(tetriObstacle.transform);
            tetriObstacle.name = "ObstacleTetromino";
            tetriObstacle.BlocksCreator = transform.GetComponent<BlocksCreator_Main>();
            tetriObstacle.Generate(Player.NotReady);
            obstacleCount--;
            Invoke(nameof(GenerateObstacle),0.5f);
        }else
        {
            if(!isServer)return;
            if(obstacleCount == 0)return;
            InstantiateTetriObstacleOnServer();
            if(!tetriObstacle)return;
            SetTetriObstacleOnServer();
        }
    }
    void GenerateMoveDirectionChanger()
    {
        if(Local())
        {
            tetriMoveDirection = Instantiate(TetriMoveDirection);
            LocalMode(tetriMoveDirection.transform);
            MoveDirectionChangerSwitcher = Switcher(MoveDirectionChangerSwitcher);
            TetriMoveDirection.BlocksCreator = transform.GetComponent<BlocksCreator_Main>();
            TetriMoveDirection.Generate(MoveDirectionChangerSwitcher);
            TetriMoveDirection.OnTetriMoveDirectionCollected += (TetriMoveDirection tetri) => 
            {
                Invoke(nameof(GenerateMoveDirectionChanger),0.1f);
            };
        }else
        {
            if(!isServer)return;
            InstantiateTetriMoveDirectionOnServer();
            if(!tetriMoveDirection)return;
            SetTetriMoveDirectionOnServer();
        }
        
    }
    void GenerateChainBall_First()
    {
        // ChainBallSwitcher = Switcher(ChainBallSwitcher);
        if(Local())
        {
            tetriBall = Instantiate(TetriBall);
            LocalMode(tetriBall.transform);
            // tetriBall.Generate(ChainBallSwitcher);
            tetriBall.Generate(Player.NotReady);
            bool continuousGeneration = false;
            if(!continuousGeneration)return;
            tetriBall.OnTetriBallCollected += (TetriBall ball) => 
            {
                Invoke(nameof(GenerateChainBall_First),0.1f);
            };
        }else
        {
            if(!isServer)return;
            InstantiateTetriBallOnServer();
            if(!tetriBall)return;
            SetTetriBallOnServer();
            
        }
        
        
    }
    public void Event_GenerateChainBall_MoraleAccumulationMaxed(Player whoMax)
    {
        tetriBall = Instantiate(TetriBall);
        tetriBall.BlocksCreator = transform.GetComponent<BlocksCreator_Main>();
        tetriBall.Generate(whoMax);
    }
    Player Switcher(Player player)
    {
        Player playerTemp = player;
        if(player == Player.Player1)
        {
            playerTemp = Player.Player2;
        }else if(player == Player.Player2)
        {
            playerTemp = Player.Player1;
        }else if(player == Player.NotReady)
        {
            playerTemp = Player.Player1;
        }
        return playerTemp;
    }
#endregion 数据操作
#region 联网数据操作
    void LocalMode(Transform needSwitchObject)
    {
        if(needSwitchObject.TryGetComponent(out NetworkTransformBase networkTransform))
        networkTransform.enabled = false;
        if(needSwitchObject.TryGetComponent(out NetworkIdentity networkIdentity))
        networkIdentity.enabled = false;
        DestroyImmediate(networkIdentity,true);
    }
    void ServerMode(Transform needSwitchObject)
    {
        if(!isServer)return;
        needSwitchObject.TryGetComponent(out NetworkTransformBase networkTransform);
        if(!networkTransform.enabled)
        networkTransform.enabled = true;
        if(!needSwitchObject.TryGetComponent(out NetworkIdentity networkIdentity))
        {
            NetworkIdentity networkIdentityTemp = needSwitchObject.gameObject.AddComponent<NetworkIdentity>();
            networkIdentityTemp.serverOnly = false;
            networkIdentityTemp.visible = Visibility.Default;
        }
    }
    public bool Local()
    {
        if(RunModeData.CurrentRunMode == RunMode.Local)return true;
        return false;
    }
    void InstantiateTetriBallOnServer()
    {
        tetriBall = Instantiate(NetworkManagerUC_PVP.spawnPrefabs.Find(prefab => prefab.name == "BallTetromino")).GetComponent<TetriBall>();
        ServerMode(tetriBall.transform);
        NetworkServer.Spawn(tetriBall.gameObject);
    }
    void InstantiateTetriObstacleOnServer()
    {
        tetriObstacle = Instantiate(NetworkManagerUC_PVP.spawnPrefabs.Find(prefab => prefab.name == "ObstacleTetromino")).GetComponent<TetriObstacle>();
        ServerMode(tetriObstacle.transform);
        NetworkServer.Spawn(tetriObstacle.gameObject);
    }
    void InstantiateTetriMoveDirectionOnServer()
    {
        tetriMoveDirection = Instantiate(NetworkManagerUC_PVP.spawnPrefabs.Find(prefab => prefab.name == "MoveDirectionTetromino")).GetComponent<TetriMoveDirection>();
        ServerMode(tetriMoveDirection.transform);
        NetworkServer.Spawn(tetriMoveDirection.gameObject);
    }
    [Server]
    void SetTetriBallOnServer()
    {
        tetriBall.BlocksCreator = transform.GetComponent<BlocksCreator_Main>();
        // tetriBall.Generate(ChainBallSwitcher);
        tetriBall.Generate(Player.NotReady);
        SetTetriBallOnClient(tetriBall.netId);
        bool continuousGeneration = false;
        if(!continuousGeneration)return;
        tetriBall.OnTetriBallCollected += (TetriBall ball) => 
        {
            Invoke(nameof(GenerateChainBall_First),0.1f);
        };
    }
    [ClientRpc]
    void SetTetriBallOnClient(uint netIdFromServer)
    {
        tetriBall = FindObjectsOfType<TetriBall>().ToList().Find(ball => ball.netId == netIdFromServer);
        tetriBall.transform.SetParent(transform);
        tetriBall.StartTimer_Growing();
    }
    [Server]
    void SetTetriObstacleOnServer()
    {
        tetriObstacle.name = "ObstacleTetromino";
        tetriObstacle.BlocksCreator = transform.GetComponent<BlocksCreator_Main>();
        tetriObstacle.Generate(Player.NotReady);
        SetTetriObstacleOnClient(tetriObstacle.netId);
        obstacleCount--;
        Invoke(nameof(GenerateObstacle),0.5f);
    }
    [ClientRpc]
    void SetTetriObstacleOnClient(uint netIdFromServer)
    {
        tetriObstacle = FindObjectsOfType<TetriObstacle>().ToList().Find(obstacle => obstacle.netId == netIdFromServer);
        tetriObstacle.transform.SetParent(transform);
        tetriObstacle.StartTimer_Growing();
    }
    [Server]
    void SetTetriMoveDirectionOnServer()
    {
        MoveDirectionChangerSwitcher = Switcher(MoveDirectionChangerSwitcher);
        TetriMoveDirection.BlocksCreator = transform.GetComponent<BlocksCreator_Main>();
        TetriMoveDirection.Generate(MoveDirectionChangerSwitcher);
        SetTetriMoveDirectionOnClient(tetriMoveDirection.netId);
        TetriMoveDirection.OnTetriMoveDirectionCollected += (TetriMoveDirection tetri) => 
        {
            Invoke(nameof(GenerateMoveDirectionChanger),0.1f);
        };
    }
    [ClientRpc]
    void SetTetriMoveDirectionOnClient(uint netIdFromServer)
    {
        tetriMoveDirection = FindObjectsOfType<TetriMoveDirection>().ToList().Find(moveDirection => moveDirection.netId == netIdFromServer);
        tetriMoveDirection.transform.SetParent(transform);
        tetriMoveDirection.StartTimer_Growing();
    }
#endregion 联网数据操作
}