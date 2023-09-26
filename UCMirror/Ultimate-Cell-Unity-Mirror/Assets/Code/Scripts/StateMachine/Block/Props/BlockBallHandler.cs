using UnityEngine;
using System.Collections.Generic;
using UC_PlayerData;
public class BlockBallHandler : MonoBehaviour,IBlockProp
{
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
    private KeyValuePair<BlockBallHandler,TetriBall> blockPairTetri = new();
    public KeyValuePair<BlockBallHandler,TetriBall> BlockPairTetri
    {
        get
        {
            return blockPairTetri;
        }
        set
        {
            if(!blockPropsState)blockPropsState = GetComponent<BlockPropsState>();
            blockPropsState.propsState = PropsData.PropsState.ChainBall;
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