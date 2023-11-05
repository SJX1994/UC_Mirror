using UnityEngine;
using System.Collections.Generic;
using UC_PlayerData;

public class BlockObstacle : MonoBehaviour, IBlockProp
{
    
    private Vector2 posId = Vector2.zero;
    public Vector2 PosId
    {
        get
        {
            if(posId.Equals(Vector2.zero)) posId = transform.GetComponent<BlockDisplay>().posId;
            return posId;
        }
        set
        {
            posId = value;
        }
    }
    private bool moveCollect;
    public bool MoveCollect{
        get
        {
            return moveCollect;
        }
        set
        {
            moveCollect = value;
            BlockPropsState.moveCollect = moveCollect;
        }
    }
    private bool stopMoveProp = false;
    public bool StopMoveProp
    {
        get
        {
            return stopMoveProp;
        }
        set
        {
            stopMoveProp = value;
            BlockPropsState.stopMoveProp = stopMoveProp;
        }
    }
    private BlockDisplay blockDisplay;
    public BlockDisplay BlockDisplay
    {
        get
        {
            if(!blockDisplay)blockDisplay = GetComponent<BlockDisplay>();
            return blockDisplay;
        }
        
    }
    private BlockPropsState blockPropsState;
    public BlockPropsState BlockPropsState
    {
        get
        {
            if(!blockPropsState)blockPropsState = GetComponent<BlockPropsState>();
            return blockPropsState;
        }
        
    }
    private KeyValuePair<BlockObstacle,TetriObstacle> blockPairTetri = new();
    public KeyValuePair<BlockObstacle,TetriObstacle> BlockPairTetri
    {
        get
        {
            return blockPairTetri;
        }
        set
        {
            if(!blockPropsState)blockPropsState = GetComponent<BlockPropsState>();
            blockPropsState.propsState = PropsData.PropsState.Obstacle;
            blockPairTetri = value;
        }   
    }

    public void Collect()
    {
        blockPropsState.propsState = PropsData.PropsState.None;
        if(!blockPairTetri.Value)return;
        blockPairTetri.Value.Collect();
        blockPairTetri = new();
    }
}