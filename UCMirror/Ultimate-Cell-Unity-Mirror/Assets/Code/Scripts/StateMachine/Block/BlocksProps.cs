using UnityEngine;
using System.Collections.Generic;
using UC_PlayerData;
public class BlocksProps : MonoBehaviour
{
    int obstacleCount = 15;
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
    Player ChainBallSwitcher = Player.NotReady;
    Player MoveDirectionChangerSwitcher;
    void Start()
    {
        MoveDirectionChangerSwitcher = Player.Player1;
    }
    public void Generate(PropsData.PropsState propsState)
    {
        switch(propsState)
        {
            case PropsData.PropsState.ChainBall:
                Invoke(nameof(GenerateChainBall),3.1f);
            break;
            case PropsData.PropsState.MoveDirectionChanger:
                Invoke(nameof(GenerateMoveDirectionChanger),1.1f);
                Invoke(nameof(GenerateMoveDirectionChanger),2.1f);
            break;
            case PropsData.PropsState.Obstacle:
                // 满格20格
                Invoke(nameof(GenerateObstacle),0.5f);
            break;
        }
    }
    void GenerateObstacle()
    {
        if(obstacleCount == 0)return;
        tetriObstacle = Instantiate(TetriObstacle);
        tetriObstacle.name = "ObstacleTetromino";
        tetriObstacle.BlocksCreator = transform.GetComponent<BlocksCreator_Main>();
        tetriObstacle.Generate(Player.NotReady);
        obstacleCount--;
        Invoke(nameof(GenerateObstacle),0.5f);
    }
    void GenerateMoveDirectionChanger()
    {
        MoveDirectionChangerSwitcher = Switcher(MoveDirectionChangerSwitcher);
        tetriMoveDirection = Instantiate(TetriMoveDirection);
        TetriMoveDirection.BlocksCreator = transform.GetComponent<BlocksCreator_Main>();
        TetriMoveDirection.Generate(MoveDirectionChangerSwitcher);
        TetriMoveDirection.OnTetriMoveDirectionCollected += (TetriMoveDirection tetri) => 
        {
            Invoke(nameof(GenerateMoveDirectionChanger),0.1f);
        };
    }
    void GenerateChainBall()
    {
        ChainBallSwitcher = Switcher(ChainBallSwitcher);
        tetriBall = Instantiate(TetriBall);
        tetriBall.BlocksCreator = transform.GetComponent<BlocksCreator_Main>();
        tetriBall.Generate(ChainBallSwitcher);
        tetriBall.OnTetriBallCollected += (TetriBall ball) => 
        {
            Invoke(nameof(GenerateChainBall),0.1f);
        };
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
}