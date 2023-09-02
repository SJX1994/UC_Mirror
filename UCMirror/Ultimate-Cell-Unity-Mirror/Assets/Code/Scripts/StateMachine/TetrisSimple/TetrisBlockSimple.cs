using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerData;
using System.Linq;
using UnityEngine.Events;
public class TetrisBlockSimple : MonoBehaviour
{
    public Vector2 posId;
    public float occupyingTime = 3f;
    public Player player = Player.NotReady;
    public Vector3 rotationPoint;
    public List<TetriBlockSimple> pioneerTetris;
    public List<TetriBlockSimple> childTetris;
    public UnityAction OnTetrisMoveing;
    public BlocksCreator blocksCreator;
    public Dictionary<TetriBlockSimple,BlockTetriHandler> TB_cache = new();
    private Stack<Vector3> positionStack;
    int moveStep = 1;
    public enum TetrisCheckMode
    {
        Create,
        Drop,
        Normal,
        Null
    }
    public TetrisCheckMode tetrisCheckMode = TetrisCheckMode.Null;
    [HideInInspector]
    public Color color;
    void Awake()
    {
       color = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
       color = new Color(color.r,color.g,color.b,1f);
    }
    void Start()
    {
       foreach (Transform child in transform)
       {
           childTetris.Add(child.GetComponent<TetriBlockSimple>());
           child.GetComponent<TetriBlockSimple>().CantPutCallback += CantPutAction;
           child.GetComponent<TetriBlockSimple>().player = player;
       }
       positionStack = new Stack<Vector3>();
       TB_cache = new();
    }
    public bool Active()
    {
        if(!transform.parent)return false;
        blocksCreator = transform.parent.GetComponent<BlocksCreator>();
        foreach (TetriBlockSimple child in childTetris)
        {
            child.player = player;
            child.Active();
            child.DoGroupMoveCheck();
        }
        EvaluatePioneers();
        EvaluateCollision();
        Move();
        return true;
    }
    public bool ColliderCheck()
    {
        List<bool> colliders = new();
        foreach(var childTetri in childTetris)
        {
            colliders.Add(childTetri.CheckCollider());
        }
        bool allTrue = colliders.All(b => b);
        return allTrue;
    }
    public bool ColliderCheckOnEndDrag()
    {
        List<bool> colliders = new();
        foreach(var childTetri in childTetris)
        {
            colliders.Add(childTetri.CheckColliderOnEndDrag());
        }
        bool allTrue = colliders.All(b => b);
        return allTrue;
    }
    // Update is called once per frame
    void Update()
    {
        if (!transform.hasChanged)return;
        // 位置发生变化
        RecordPosition();
        // 重置 hasChanged 属性
        transform.hasChanged = false; 
    }
    public void FailToCreat()
    {
        foreach(var child in childTetris)
        {
            child.FailToCreat();
        }
    }
    public void SuccessToCreat()
    {
        foreach(var child in childTetris)
        {
            child.SuccessToCreat();
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
        if(positionStack.Count <= 0) return;
        // 弹出栈顶位置并移动对象
        Vector3 previousPosition = positionStack.Pop();
        transform.localPosition = previousPosition;
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
            if(!blockCurrent)return;
            blockCurrent.tetriBlockSimpleHolder = tetriBlock;
            tetriBlock.currentBlockTetriHandler = blockCurrent;
            // 可视化
            // blockCurrent.transform.localScale -= Vector3.one*0.3f;
            // tetriBlock.transform.localScale -= Vector3.one*0.3f;
            if (TB_cache.ContainsKey(tetriBlock))return;
            TB_cache.Add(tetriBlock,blockCurrent);
        }
    }
    void EvaluatePioneers()
    {
        // 取前方没有砖块的砖块
        if(player == Player.Player1)
        {
            foreach(var childTetri in childTetris)
            {
                bool P1FrontObj = childTetris.FirstOrDefault(obj => obj.posId == new Vector2(childTetri.posId.x+1,childTetri.posId.y));
                if(P1FrontObj)return;
                pioneerTetris.Add(childTetri);
                
            }
        }else if (player == Player.Player2)
        {
            foreach(var childTetri in childTetris)
            {
                bool P2FrontObj = childTetris.FirstOrDefault(obj => obj.posId == new Vector2(childTetri.posId.x-1,childTetri.posId.y));
                if(P2FrontObj)return;
                pioneerTetris.Add(childTetri);
            }
        }
        // 可视化
        // foreach(var pioneerBlock in pioneerTetris)
        // {
        //     pioneerBlock.transform.localScale += 0.2f * Vector3.one;
        // }
    }
    public void Stop()
    {
        moveStep = 0;
        CancelInvoke(nameof(MoveActive));
    }
    public void Move()
    {
        moveStep = 1;
        InvokeRepeating(nameof(MoveActive),0,occupyingTime);
    }
    void MoveActive()
    {
        if(!ValidMove())return;

        foreach (TetriBlockSimple child in childTetris)
        {
            child.DoGroupMoveCheck();
        }
        if(player == Player.Player1)
        {
            transform.localPosition += new Vector3(moveStep,0,0);
        }
        else if(player == Player.Player2)
        {
            transform.localPosition += new Vector3(-moveStep,0,0);
        }
        posId = new Vector2(transform.localPosition.x,transform.localPosition.z);
        OnTetrisMoveing?.Invoke();
        EvaluateCollision();
    }
    public void Rotate(Vector3 axis)
    {
       transform.RotateAround(transform.TransformPoint(rotationPoint), axis, 90);
    }
    public void RotateReverse(Vector3 axis)
    {
       transform.RotateAround(transform.TransformPoint(rotationPoint), axis, -90);
    }
    bool ValidMove()
    {
        switch (tetrisCheckMode)
        {
            case TetrisCheckMode.Create:
               return CreateValidMove();
            case TetrisCheckMode.Drop:
               return DropValidMove();
            case TetrisCheckMode.Normal:
               return NormalValidMove();
            default:
                return false;
        }
        
    }
    bool NormalValidMove()
    {
        moveStep = 1;
        foreach(var pineer in pioneerTetris)
        {
            BlockTetriHandler blockCurrent = pineer.CurrentBlock();
            if(!blockCurrent) return false;
            BlockTetriHandler blockNext = pineer.NextBlock();
            if(!blockNext)return false;
            if(!pineer.CanMove)return false;
            if(!pineer.BlockNextCheck(blockNext))return false;
        }
        
        return true;
    }
    bool DropValidMove()
    {
        tetrisCheckMode = TetrisCheckMode.Normal;
        moveStep = 0;
        foreach(var child in childTetris)
        {
            child.AfterDropCheck();
        }
        return true;
    }
    bool CreateValidMove()
    {
        tetrisCheckMode = TetrisCheckMode.Normal;
        foreach(var pineer in pioneerTetris)
        {
            
            BlockTetriHandler blockCurrent = pineer.CurrentBlock();
            if(!blockCurrent) return false;
            BlockTetriHandler blockNext = pineer.NextBlock();
            if(!blockNext)return false;
            if(!pineer.CanMove)return false;
            if(!pineer.BlockNextCheck(blockNext))return false;
           
        }
        foreach (TetriBlockSimple child in childTetris)
        {
            if(!child.CanMove)return false;
        }
        
        return true;
    }
    public bool BuoyValidDrop()
    {
        foreach(var child in childTetris)
        {
            return child.DoGroupDropCheck();
        }
        return true;
    }
    public bool OnBuoyDrop()
    {
        List<bool> buoyDrop = new();
        
        // 所有未占领的砖块恢复和平状态
        foreach(var tetri in childTetris)
        {
            if(!tetri.currentBlockTetriHandler){buoyDrop.Add(false);}
            if(tetri.currentBlockTetriHandler.State != BlockTetriHandler.BlockTetriState.Occupying)continue;
            tetri.CancleOccupied();
            if(!tetri.currentBlockTetriHandler){buoyDrop.Add(false); break;}
            if(tetri.currentBlockTetriHandler.State == BlockTetriHandler.BlockTetriState.Occupying){buoyDrop.Add(false);}
            buoyDrop.Add(true);
        }
       
        bool allTrue = buoyDrop.All(b => b);
        return allTrue;
    }
    public void Reset()
    {
        foreach(var tetri in childTetris)
        {
            tetri.Reset();
        }
    }
    
}

