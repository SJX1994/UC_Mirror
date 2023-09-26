using UnityEngine;
using System.Collections.Generic;
using UC_PlayerData;
public class BlocksProps : MonoBehaviour
{
    TetriBall tetriBall;
    TetriMoveDirection tetriMoveDirection;
    Player ChainBallSwitcher;
    Player MoveDirectionChangerSwitcher;
    void Start()
    {
        ChainBallSwitcher = Player.Player1;
        MoveDirectionChangerSwitcher = Player.Player1;
    }
    public void Generate(PropsData.PropsState propsState)
    {
        switch(propsState)
        {
            case PropsData.PropsState.ChainBall:
                Invoke(nameof(GenerateChainBall),0.1f);
            break;
            case PropsData.PropsState.MoveDirectionChanger:
                Invoke(nameof(GenerateMoveDirectionChanger),1.1f);
                Invoke(nameof(GenerateMoveDirectionChanger),2.1f);
            break;
        }
    }
    void GenerateMoveDirectionChanger()
    {
        MoveDirectionChangerSwitcher = Switcher(MoveDirectionChangerSwitcher);
        tetriMoveDirection = Resources.Load<TetriMoveDirection>("Props/MoveDirectionTetromino");
        tetriMoveDirection = Instantiate(tetriMoveDirection);
        tetriMoveDirection.blocksCreator = transform.GetComponent<BlocksCreator>();
        tetriMoveDirection.Generate(MoveDirectionChangerSwitcher);
        tetriMoveDirection.OnTetriMoveDirectionCollected += (TetriMoveDirection tetri) => 
        {
            Invoke(nameof(GenerateMoveDirectionChanger),2.1f);
        };
    }
    void GenerateChainBall()
    {
        Player turn = Switcher(ChainBallSwitcher);
        tetriBall = Resources.Load<TetriBall>("Props/BallTetromino");
        tetriBall = Instantiate(tetriBall);
        tetriBall.blocksCreator = transform.GetComponent<BlocksCreator>();
        tetriBall.Generate(turn);
        tetriBall.OnTetriBallCollected += (TetriBall ball) => 
        {
            Invoke(nameof(GenerateChainBall),0.1f);
        };
    }
    Player Switcher(Player player)
    {
        if(player == Player.Player1)
        {
            ChainBallSwitcher = Player.Player2;
        }else
        {
            ChainBallSwitcher = Player.Player1;
        }
        return ChainBallSwitcher;
    }
}