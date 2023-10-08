using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
public class TetriMechanism : MonoBehaviour
{
    Player player;
    Vector2 posId;
    private BlocksCounter blocksCounter;
    public BlocksCounter BlocksCounter
    {
        get
        {
            if(!blocksCounter)blocksCounter = FindObjectOfType<BlocksCounter>();
            return blocksCounter;
        }
    }
    private TetrisBlockSimple tetrisBlockSimple;
    public TetrisBlockSimple TetrisBlockSimple
    {
        get
        {
            if(!tetrisBlockSimple) tetrisBlockSimple = GetComponent<TetriBlockSimple>().tetrisBlockSimple;
            return tetrisBlockSimple;
        }
    }
    void Start()
    {
        TetriBlockSimple tbs = GetComponent<TetriBlockSimple>();
        player = TetrisBlockSimple.player;
        tbs.TetriPosIdChanged += OnPosIdChanged;
        tbs.TetriPlayerChanged += OnPlayerChanged;

    }
    public BlockDisplay FindBlockWithId(Vector2 posId)
    {
        BlockDisplay block = BlocksCounter.BlocksCreator.blocks.Find((block) => block.posId == posId);
        // if(!block)Debug.Log("找不到砖块");
        return block;
    }
    void OnPosIdChanged(Vector2 posId)
    {
        // Time.timeScale = 3;
        this.posId = posId;
        if((posId.x == 0 && player == Player.Player2)||(posId.x == 19 && player == Player.Player1))
        {
            // 砖块表现
            BlocksCounter.DoReachBottomLineGain(posId);
            // Unit 表现
            TetriUnitSimple tus = GetComponent<TetriUnitSimple>();
            if(!tus.haveUnit)return;
            tus.haveUnit.BlocksMechanismDoing(BlocksData.BlocksMechanismType.ReachBottomLine);
        }
    }
    void OnPlayerChanged(Player player)
    {
        this.player = player;
    }
}