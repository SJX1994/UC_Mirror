using UnityEngine;
using System.Collections.Generic;
using UC_PlayerData;
public class BlockPropsState : MonoBehaviour
{
    public bool moveCollect;
    private BlockMoveDirection blockMoveDirection;
    public BlockMoveDirection BlockMoveDirection
    {
        get
        {
            if(!blockMoveDirection)blockMoveDirection = GetComponent<BlockMoveDirection>();
            return blockMoveDirection;
        }
    }
    private BlockBallHandler blockBallHandler;
    public BlockBallHandler BlockBallHandler
    {
        get
        {
            if(!blockBallHandler)blockBallHandler = GetComponent<BlockBallHandler>();
            return blockBallHandler;
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
    public PropsData.PropsState propsState = PropsData.PropsState.None;
}