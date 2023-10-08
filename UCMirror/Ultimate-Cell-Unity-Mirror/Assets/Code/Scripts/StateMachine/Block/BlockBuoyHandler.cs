using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBuoyHandler : MonoBehaviour
{
    public Vector2 posId;
    public TetriBuoySimple tetriBuoySimple;
    BlockDisplay blockDisplay;
    public BlockDisplay BlockDisplay
    {
        get
        {
            if(!blockDisplay)blockDisplay = GetComponent<BlockDisplay>();
            return blockDisplay;
        }
    }
    public BlockTetriHandler blockTetriHandler;
    
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(LateStart), 0.1f);
    }
    void LateStart()
    {
        blockDisplay = GetComponent<BlockDisplay>();
        posId = blockDisplay.posId;
        blockTetriHandler = GetComponent<BlockTetriHandler>();
    }
    public TetrisBlockSimple GetTetris()
    {
        if(blockTetriHandler.tetriBlockSimpleHolder)
        {
            TetrisBlockSimple tetrisBlockSimple = blockTetriHandler.tetriBlockSimpleHolder.tetrisBlockSimple;
            return tetrisBlockSimple;
        }else
        {
            return null;
        }
    }
    public TetrisBuoySimple GetTetrisBuoy()
    {
        if(blockTetriHandler.tetriBlockSimpleHolder)
        {
            TetrisBlockSimple tetrisBlockSimple = blockTetriHandler.tetriBlockSimpleHolder.tetrisBlockSimple;
            return tetrisBlockSimple.GetComponent<TetrisBuoySimple>();
        }else
        {
            return null;
        }
    }
    

   

}
