using UnityEngine;
using System.Collections.Generic;
using UC_PlayerData;
public class BlocksProps : MonoBehaviour
{
    TetriBall tetriBall;
    Player ChainBallSwitcher;
    void Start()
    {
        ChainBallSwitcher = Player.Player1;
    }
    public void Generate(PropsData.PropsState propsState)
    {
        switch(propsState)
        {
            case PropsData.PropsState.ChainBall:
                Invoke(nameof(GenerateChainBall),0.1f);
            break;
        }
    }
    void GenerateChainBall()
    {
        Player turn = Switcher(ChainBallSwitcher);
        tetriBall = Resources.Load<TetriBall>("BallTetromino");
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