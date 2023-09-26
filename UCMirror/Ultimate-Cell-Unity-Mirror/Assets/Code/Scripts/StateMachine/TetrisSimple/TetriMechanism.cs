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
    void Start()
    {
        TetriBlockSimple tbs = GetComponent<TetriBlockSimple>();
        player = tbs.player;
        tbs.TetriPosIdChanged += TetriMechanismGetPosId;
        tbs.TetriPlayerChanged += TetriMechanismGetPlayer;

    }
    void TetriMechanismGetPosId(Vector2 posId)
    {
        // Time.timeScale = 3;
        this.posId = posId;
        if(posId.x == 0 && player == Player.Player2)
        {
            // Debug.Log("Player2 到达底线 触达ID" + posId);
            BlocksCounter.DoLineEffect(posId);
        }else if(posId.x == 19 && player == Player.Player1)
        {
            // Debug.Log("Player1 到达底线 触达ID" + posId);
            BlocksCounter.DoLineEffect(posId);
        }
        
    }
    void TetriMechanismGetPlayer(Player player)
    {
        this.player = player;
    }
}