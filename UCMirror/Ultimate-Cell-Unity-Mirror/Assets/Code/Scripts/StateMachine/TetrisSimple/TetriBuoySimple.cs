using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class TetriBuoySimple : MonoBehaviour
{
    public Vector2 posId;
    public BlockBuoyHandler blockBuoyHandler;
    PlayerData.Player player;
    public TetriBlockSimple tetriBlockSimple;
    public TetrisBuoySimple tetrisBuoySimple;
    TetriDisplayRange tetriDisplayRange;
    
    void Start()
    {
        Invoke(nameof(LateStart),0.1f);
        
    }
    void LateStart()
    {
        tetriBlockSimple = transform.GetComponent<TetriBlockSimple>();
        Transform displayGo = transform.Find("Display_Range");
        if(!displayGo)return;
        tetriDisplayRange = displayGo.GetComponent<TetriDisplayRange>();
        tetrisBuoySimple.tetrisBlockSimple.OnUpdatDisplay += Display_Evaluate;
    }
    public bool DoDropDragingCheck()
    {
        return DoDropCanPutCheck();
    }
    public bool DoDropDragingCheck(List<TetriBuoySimple> BuoyTetriBuoys)
    {
        return DoDropCanPutCheck(BuoyTetriBuoys);
    }
    public bool DoDropCanPutCheck(List<TetriBuoySimple> BuoyTetriBuoys)
    {
        // 发射射线向下进行检测
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, tetriBlockSimple.blockTargetMask);
        if (!hitBlock)return false;
        // 进一步的处理
        BlockBuoyHandler block;
        hit.collider.transform.TryGetComponent(out block);
        if(!block)return false;
        if(BuoyTetriBuoys.Contains(block.tetriBuoySimple))
        {
            return true;
        }
        if(block.tetriBuoySimple)
        {
            return false;
        }
        if(block.blockTetriHandler.tetriBlockSimpleHolder)
        {
            return false;
        }
        if(block.blockTetriHandler.State == BlockTetriHandler.BlockTetriState.Peace)
        {
            return true;
        }
        if(tetriBlockSimple.player == PlayerData.Player.Player1 && block.blockTetriHandler.State != BlockTetriHandler.BlockTetriState.Occupied_Player1)
        {
            return false;
        }
        else if(tetriBlockSimple.player == PlayerData.Player.Player2 && block.blockTetriHandler.State != BlockTetriHandler.BlockTetriState.Occupied_Player2)
        {
            return false;   
        }
        return true;
    }
    public bool DoDropCanPutCheck()
    {
        // 发射射线向下进行检测
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, tetriBlockSimple.blockTargetMask);
        if (!hitBlock)return false;
        // 进一步的处理
        BlockBuoyHandler block;
        hit.collider.transform.TryGetComponent(out block);
        if(!block)return false;
        if(tetrisBuoySimple.tetrisBuoyDragged.childTetris.Contains(block.tetriBuoySimple))
        {
            return true;
        }
        if(block.tetriBuoySimple)
        {
            return false;
        }
        if(block.blockTetriHandler.tetriBlockSimpleHolder)
        {
            return false;
        }
        if(block.blockTetriHandler.State == BlockTetriHandler.BlockTetriState.Peace)
        {
            return true;
        }
        if(tetriBlockSimple.player == PlayerData.Player.Player1 && block.blockTetriHandler.State != BlockTetriHandler.BlockTetriState.Occupied_Player1)
        {
            return false;
        }
        else if(tetriBlockSimple.player == PlayerData.Player.Player2 && block.blockTetriHandler.State != BlockTetriHandler.BlockTetriState.Occupied_Player2)
        {
            return false;   
        }
        return true;
    }
    public void InFlow()
    {
        if(!tetriDisplayRange)return;
        tetriDisplayRange.SetSortingOrder(PlayerData.Dispaly.FlowOrder+1);
    }
    public void OutFlow()
    {
        if(!tetriDisplayRange)return;
        tetriDisplayRange.SetSortingOrder(PlayerData.Dispaly.NotFlowOrder+1);
    }
    public void Display_Evaluate()
    {
        BlocksCreator blocksCreator = tetrisBuoySimple.tetrisBlockSimple.blocksCreator;
        var blockUp = blocksCreator.blocks.Find((block) => block.posId == new Vector2(posId.x,posId.y + 1));
        if(blockUp && blockUp.GetComponent<BlockBuoyHandler>().tetriBuoySimple && blockUp.GetComponent<BlockBuoyHandler>().tetriBuoySimple.player == player)
        {
            tetriDisplayRange.Up.gameObject.SetActive(false);
        }else
        {
            tetriDisplayRange.Up.gameObject.SetActive(true);
        }
        var blockDown = blocksCreator.blocks.Find((block) => block.posId == new Vector2(posId.x,posId.y - 1));
        if(blockDown && blockDown.GetComponent<BlockBuoyHandler>().tetriBuoySimple && blockDown.GetComponent<BlockBuoyHandler>().tetriBuoySimple.player == player)
        {
            tetriDisplayRange.Down.gameObject.SetActive(false);
        }else
        {
            tetriDisplayRange.Down.gameObject.SetActive(true);
        }
        var blockLeft = blocksCreator.blocks.Find((block) => block.posId == new Vector2(posId.x - 1,posId.y));
        if(blockLeft && blockLeft.GetComponent<BlockBuoyHandler>().tetriBuoySimple && blockLeft.GetComponent<BlockBuoyHandler>().tetriBuoySimple.player == player)
        {
            tetriDisplayRange.Left.gameObject.SetActive(false);
        }else
        {
            tetriDisplayRange.Left.gameObject.SetActive(true);
        }
        var blockRight = blocksCreator.blocks.Find((block) => block.posId == new Vector2(posId.x + 1,posId.y));
        if(blockRight && blockRight.GetComponent<BlockBuoyHandler>().tetriBuoySimple && blockRight.GetComponent<BlockBuoyHandler>().tetriBuoySimple.player == player)
        {
            tetriDisplayRange.Right.gameObject.SetActive(false);
        }else
        {
            tetriDisplayRange.Right.gameObject.SetActive(true);
        }

    }
}