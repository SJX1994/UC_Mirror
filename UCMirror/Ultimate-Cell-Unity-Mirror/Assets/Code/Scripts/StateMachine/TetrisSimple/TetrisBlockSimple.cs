using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerData;
using System.Linq;
using UnityEngine.Events;
public class TetrisBlockSimple : MonoBehaviour
{
    public float occupyingTime = 3f;
    public Player player = Player.NotReady;
    public Vector3 rotationPoint;
    public List<TetriBlockSimple> pioneerTetris;
    public List<TetriBlockSimple> childTetris;
    public UnityAction OnTetrisMoveing;
    public BlocksCreator blocksCreator;
    public Dictionary<TetriBlockSimple,BlockTetriHandler> TB_cache = new();
    private Stack<Vector3> positionStack;
    void Start()
    {
       foreach (Transform child in transform)
       {
           childTetris.Add(child.GetComponent<TetriBlockSimple>());
           child.GetComponent<TetriBlockSimple>().CantPutCallback += CantPutAction;
           child.GetComponent<TetriBlockSimple>().player = player;
       }
       positionStack = new Stack<Vector3>();
       // occupyingTime = 3f;
       TB_cache = new();
    }
    public void Active()
    {
        blocksCreator = transform.parent.GetComponent<BlocksCreator>();
        foreach (TetriBlockSimple child in childTetris)
        {
            child.player = player;
            child.Active();
            child.DoGroupMoveCheck();
        }
        EvaluatePioneers();
        EvaluateCollision();
        InvokeRepeating(nameof(Move),0,occupyingTime);
    }
    public void Draging()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (transform.hasChanged)
        {
            // 位置发生变化
            RecordPosition();
            transform.hasChanged = false; // 重置 hasChanged 属性
        }
       
    }
    void CantPutAction(TetriBlockSimple tetriBlock)
    {
        RollbackPosition();
    }
    void RecordPosition()
    {
        // 将当前位置入栈
        positionStack.Push(transform.localPosition);
    }
    void RollbackPosition()
    {
        // 检查栈是否为空
        if (positionStack.Count > 0)
        {
            // 弹出栈顶位置并移动对象
            Vector3 previousPosition = positionStack.Pop();
            transform.localPosition = previousPosition;
        }
    }
    public void EvaluateCollision()
    {
        
        if(TB_cache.Count>0)
        {
            foreach(var item in TB_cache)
            {
                item.Key.currentBlockTetriHandler = null;
                item.Value.tetriBlockSimpleHolder = null;
            }
            TB_cache.Clear();
        }
        foreach (TetriBlockSimple tetriBlock in childTetris)
        {
            BlockTetriHandler blockCurrent = null;
            blockCurrent = blocksCreator.blocks.Find((block) => block.posId == new Vector2(tetriBlock.posId.x,tetriBlock.posId.y)).GetComponent<BlockTetriHandler>();
            if(blockCurrent)
            {
                blockCurrent.tetriBlockSimpleHolder = tetriBlock;
                tetriBlock.currentBlockTetriHandler = blockCurrent;
                // 可视化
                // blockCurrent.transform.localScale -= Vector3.one*0.3f;
                // tetriBlock.transform.localScale -= Vector3.one*0.3f;
                if (!TB_cache.ContainsKey(tetriBlock))
                {
                    TB_cache.Add(tetriBlock,blockCurrent);
                }
                
            }
            BlockTetriHandler blockCurrentTeri = blockCurrent.GetComponent<BlockTetriHandler>();
        }
    }
    void EvaluatePioneers()
    {
        // 取前方没有砖块的砖块
        if(player == Player.Player1)
        {
            foreach(var childTetri in childTetris)
            {
                if(!childTetris.FirstOrDefault(obj => obj.posId == new Vector2(childTetri.posId.x+1,childTetri.posId.y)))
                {
                    pioneerTetris.Add(childTetri);
                }
            }
        }else if (player == Player.Player2)
        {
            foreach(var childTetri in childTetris)
            {
                if(!childTetris.FirstOrDefault(obj => obj.posId == new Vector2(childTetri.posId.x-1,childTetri.posId.y)))
                {
                    pioneerTetris.Add(childTetri);
                }
            }
        }
        // 可视化
        // foreach(var pioneerBlock in pioneerTetris)
        // {
        //     pioneerBlock.transform.localScale += 0.2f * Vector3.one;
        // }
    }
    void Move()
    {
        if(!ValidMove())return;

        foreach (TetriBlockSimple child in childTetris)
        {
            child.DoGroupMoveCheck();
        }
        if(player == Player.Player1)
        {
            transform.localPosition += new Vector3(1,0,0);
        }
        else if(player == Player.Player2)
        {
            transform.localPosition += new Vector3(-1,0,0);
        }

        OnTetrisMoveing?.Invoke();

        EvaluateCollision();
    }
    public void Rotate(Vector3 axis)
    {
       transform.RotateAround(transform.TransformPoint(rotationPoint), axis, 90);
    }
    bool ValidMove()
    {
        foreach(var pineer in pioneerTetris)
        {
            BlockDisplay blockNext = null;
            BlockDisplay blockCurrent = null;
            blockCurrent = blocksCreator.blocks.Find((block) => block.posId == new Vector2(pineer.posId.x,pineer.posId.y));
            BlockTetriHandler blockCurrentTeri = blockCurrent.GetComponent<BlockTetriHandler>();
            if(blockCurrentTeri.State == BlockTetriHandler.BlockTetriState.Peace)
            {
                blockCurrentTeri.State = BlockTetriHandler.BlockTetriState.Occupying;
            }
            if(player == Player.Player1)
            {
                blockNext = blocksCreator.blocks.Find((block) => block.posId == new Vector2(pineer.posId.x + 1,pineer.posId.y));
            }else if(player == Player.Player2)
            {
                blockNext = blocksCreator.blocks.Find((block) => block.posId == new Vector2(pineer.posId.x - 1,pineer.posId.y));
            }
            if(blockNext == null)
            {
                return false;
            }else
            {
                if(blockNext.GetComponent<BlockTetriHandler>().tetriBlockSimpleHolder!=null)
                {
                    return false;
                }
            }
            BlockTetriHandler blockTetriHandler = blockNext.GetComponent<BlockTetriHandler>();
            if(blockTetriHandler.State == BlockTetriHandler.BlockTetriState.Occupied_Player2 && player == Player.Player1)
            {
                return false;
            }
            if(blockTetriHandler.State == BlockTetriHandler.BlockTetriState.Occupied_Player1 && player == Player.Player2)
            {
                return false;
            }
            if(blockTetriHandler.State == BlockTetriHandler.BlockTetriState.Occupying)
            {
                return false;
            }
            if(!pineer.CanMove)
            {
                return false;
            }
        }
        foreach (TetriBlockSimple child in childTetris)
        {
            if(!child.CanMove)
            {
                return false;
            }
            
        }
        
        return true;
    }
}

