using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
using Mirror;
public class TetriMechanism : NetworkBehaviour
{
#region 数据对象
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
#endregion 数据对象
#region 数据关系
    void Start()
    {
        TetriBlockSimple tbs = GetComponent<TetriBlockSimple>();
        player = TetrisBlockSimple.player;
        tbs.TetriPosIdChanged += OnPosIdChanged;
        tbs.TetriPlayerChanged += OnPlayerChanged;

    }
#endregion 数据关系
#region 数据操作
    public BlockDisplay FindBlockWithId(Vector2 posId)
    {
        BlockDisplay block = BlocksCounter.BlocksCreator.blocks.Find((block) => block.posId == posId);
        // if(!block)Debug.Log("找不到砖块");
        return block;
    }
    void OnPosIdChanged(Vector2 posId)
    {
        if(Local())
        {
            // Time.timeScale = 3;
            this.posId = posId;
            if((posId.x == 0 && player == Player.Player2)||(posId.x == 19 && player == Player.Player1))
            {
                // 砖块表现
                BlocksCounter.DoReachBottomLineGain(posId);
                // Unit 表现
                TetriUnitSimple tus = GetComponent<TetriUnitSimple>();
                if(!tus.HaveUnit)return;
                tus.HaveUnit.Event_BlocksMechanismDoing(BlocksData.BlocksMechanismType.ReachBottomLine);
            }
        }else
        {
            if(!isServer)return;
            // Time.timeScale = 3;
            this.posId = posId;
            if((posId.x == 0 && player == Player.Player2)||(posId.x == 19 && player == Player.Player1))
            {
                // 砖块表现
                BlocksCounter.DoReachBottomLineGain(posId);
                Client_DoReachBottomLineGain(posId);
                // Unit 表现
                TetriUnitSimple tus = GetComponent<TetriUnitSimple>();
                if(!tus.HaveUnit)return;
                tus.HaveUnit.Event_BlocksMechanismDoing(BlocksData.BlocksMechanismType.ReachBottomLine);
            }
        }
        
    }
    void OnPlayerChanged(Player player)
    {
        this.player = player;
    }
#endregion 数据操作
#region 联网数据操作
    bool Local()
    {
        if(RunModeData.CurrentRunMode == RunMode.Local)return true;
        return false;
    }
    [ClientRpc]
    void Client_DoReachBottomLineGain(Vector2 posId)
    {
        BlocksCounter.DoReachBottomLineGain(posId);
    }
#endregion 联网数据操作
}