using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
using System.Linq;
using UnityEngine.Events;
using Mirror;
using DG.Tweening;
public class TetrisBlockSimple : NetworkBehaviour
{
    [SyncVar]
    public Vector2 posId;
    public float occupyingTime = 3f;
    public Player player = Player.NotReady;
    public Player Player
    {
        get
        {
            return player;
        }
        set
        {
            // if(!isClient)return;
            if(player == value)return;
            player = value;
            Client_Reflash();
        }
    }
    [SyncVar(hook = nameof(GoOnTheBlocksCreator))]
    public bool onTheBlocksCreator;
    public bool OnTheBlocksCreator
    {
        get
        {
            return onTheBlocksCreator;
        }
        set
        {
            if(onTheBlocksCreator == value)return;
            GoOnTheBlocksCreator(onTheBlocksCreator,value);
            onTheBlocksCreator = value;
            
        }
    }
    public Vector3 rotationPoint;
    public List<TetriBlockSimple> pioneerTetris;
    public List<TetriBlockSimple> childTetris;
    public UnityAction OnTetrisMoveing;
    public BlocksCreator blocksCreator;
    public Dictionary<TetriBlockSimple,BlockTetriHandler> TB_cache = new();
    public UnityAction<Dictionary<TetriBuoySimple,BlockBuoyHandler>> OnCacheUpdateForBuoyMarkers;
    public UnityAction OnUpdatDisplay;
    public UnityAction OnStartMove;
    public UnityAction<bool> OnRotate;
    private Stack<Vector3> positionStack;
    int moveStep = 1;
    public IdelBox idelBox;
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
    public Sequence sequence;
    [Header("UC_PVP:")]
    [SyncVar]
    public int serverID = -1;
    public int ServerID
    {
        get
        {
            return serverID;
        }
        set
        {
            if(serverID == value)return;
            SyncOtherTetrisComponent(value);
            serverID = value;
        }
    }
    TetrisBuoySimple tetrisBuoySimple;
    private TetrisUnitSimple tetrisUnitSimple;
    public TetrisUnitSimple TetrisUnitSimple
    {
        get
        {
            if(!tetrisUnitSimple)tetrisUnitSimple = transform.GetComponent<TetrisUnitSimple>();
            return tetrisUnitSimple;
        }
    }
    [SyncVar]
    public int rotateTimes;
    public int RotateTimes
    {
        get
        {
            return rotateTimes;
        }
        set
        {
            // Debug.Log($"rotateTimes:{rotateTimes}");
            if(rotateTimes == value)return;
            if(!tetrisBuoySimple)tetrisBuoySimple = transform.GetComponent<TetrisBuoySimple>();
            tetrisBuoySimple.rotateTimes = value;
            rotateTimes = value;
            if(rotateTimes!=4)return;
            rotateTimes = 0;
        }
    }
    public string type;
    public bool autoID = true;
    void Awake()
    {
       color = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
       color = new Color(color.r,color.g,color.b,1f);
       onTheBlocksCreator = false;
       blocksCreator = FindObjectOfType<BlocksCreator>();
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
       if(Local())return;
       if(!isServer)return;
       if(!autoID)return;
       serverID = ServerLogic.GetTetrisGroupID();
       // Debug.Log($"serverID:{serverID}");
    }
    public void Init()
    {
       color = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
       color = new Color(color.r,color.g,color.b,1f);
       onTheBlocksCreator = false;
       blocksCreator = FindObjectOfType<BlocksCreator>();
       foreach (Transform child in transform)
       {
           childTetris.Add(child.GetComponent<TetriBlockSimple>());
           child.GetComponent<TetriBlockSimple>().CantPutCallback += CantPutAction;
           child.GetComponent<TetriBlockSimple>().player = player;
       }
       positionStack = new Stack<Vector3>();
       TB_cache = new();
       if(Local())return;
       if(!isServer)return;
       if(!autoID)return;
       serverID = ServerLogic.GetTetrisGroupID();
    }
    public bool Active()
    {
        if(!transform.parent)return false;
        blocksCreator = transform.parent.GetComponent<BlocksCreator>();
        foreach (TetriBlockSimple child in childTetris)
        {
            if(child == null)continue;
            child.player = player;
            child.Active();
            child.DoGroupMoveCheck();
        }
        EvaluatePioneers();
        EvaluateCollision();
        Move();
        transform.GetComponent<TetrisBuoySimple>().Display_Active();
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
    void CantPutAction()
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
        Dictionary<TetriBuoySimple,BlockBuoyHandler> buoyMarkersTemp = new();
        foreach (TetriBlockSimple tetriBlock in childTetris)
        {
            BlockTetriHandler blockCurrent = null;
            blockCurrent = blocksCreator.blocks.Find((block) => block.posId == new Vector2(tetriBlock.PosId.x,tetriBlock.PosId.y)).GetComponent<BlockTetriHandler>();
            if(!blockCurrent)continue;
            blockCurrent.tetriBlockSimpleHolder = tetriBlock;
            tetriBlock.currentBlockTetriHandler = blockCurrent;
            // 可视化
            // blockCurrent.transform.localScale -= Vector3.one*0.3f;
            // tetriBlock.transform.localScale -= Vector3.one*0.3f;
            if (TB_cache.ContainsKey(tetriBlock))continue;
            TB_cache.Add(tetriBlock,blockCurrent);
            TetriBuoySimple t = tetriBlock.GetComponent<TetriBuoySimple>();
            t.posId = tetriBlock.PosId;
            BlockBuoyHandler b = blockCurrent.GetComponent<BlockBuoyHandler>();
            b.posId = blockCurrent.posId;
            buoyMarkersTemp.Add(t,b);
            
        }
        OnCacheUpdateForBuoyMarkers?.Invoke(buoyMarkersTemp);
        OnUpdatDisplay?.Invoke();
    }
    public void EvaluatePioneers()
    {
        // 取前方没有砖块的砖块
        if(player == Player.Player1)
        {
            foreach(var childTetri in childTetris)
            {
                bool P1FrontObj = childTetris.FirstOrDefault(obj => obj.PosId == new Vector2(childTetri.PosId.x+1,childTetri.PosId.y));
                if(P1FrontObj)continue;
                pioneerTetris.Add(childTetri);
            }
        }else if (player == Player.Player2)
        {
            foreach(var childTetri in childTetris)
            {
                bool P2FrontObj = childTetris.FirstOrDefault(obj => obj.PosId == new Vector2(childTetri.PosId.x-1,childTetri.PosId.y));
                if(P2FrontObj)continue;
                pioneerTetris.Add(childTetri);
            }
        }
        // 可视化
        // foreach(var pioneerBlock in pioneerTetris)
        // {
        //     if(!pioneerBlock)continue;
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
        OnStartMove?.Invoke();
        foreach (TetriBlockSimple child in childTetris)
        {
            if(!child)continue;
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
        OnRotate?.Invoke(true);
        transform.RotateAround(transform.TransformPoint(rotationPoint), axis, 90);
    }
    public void RotateReverse(Vector3 axis)
    {
        OnRotate?.Invoke(false);
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
        List<bool> condition = new();
        foreach(var pineer in pioneerTetris)
        {
            if(!pineer)continue;
            BlockTetriHandler blockCurrent = pineer.CurrentBlock();
            condition.Add(blockCurrent);
            BlockTetriHandler blockNext = pineer.NextBlock();
            condition.Add(blockNext);
            condition.Add(pineer.CanMove);
            condition.Add(pineer.BlockNextCheck(blockNext));
        }
        bool allTrue = condition.All(b => b);
        return allTrue;
    }
    bool DropValidMove()
    {
        OnStartMove?.Invoke();
        tetrisCheckMode = TetrisCheckMode.Normal;
        moveStep = 0;
        foreach(var child in childTetris)
        {
            if(!child)continue;
            child.DropCheck();
        }
        return true;
    }
    bool CreateValidMove()
    {
        OnStartMove?.Invoke();
        tetrisCheckMode = TetrisCheckMode.Normal;
        List<bool> condition = new();
        foreach(var pineer in pioneerTetris)
        {
            if(!pineer)continue;
            BlockTetriHandler blockCurrent = pineer.CurrentBlock();
            condition.Add(blockCurrent);
            BlockTetriHandler blockNext = pineer.NextBlock();
            condition.Add(blockNext);
            condition.Add(pineer.CanMove);
            condition.Add(pineer.BlockNextCheck(blockNext));
           
        }
        foreach (TetriBlockSimple child in childTetris)
        {
            if(!child)continue;
            condition.Add(child.CanMove);
        }
        bool allTrue = condition.All(b => b);
        return allTrue;
    }
    /// <summary>
    /// 在创建砖块后 创建一个 Dotween 动画
    /// </summary>
    public void Display_AfterCreate()
    {
        // 使用 DOTween.Sequence 创建一个序列
        sequence = DOTween.Sequence();
        // 在序列中添加需要执行的动画
        sequence.Append(transform.DOLocalMoveY(0.2f, occupyingTime/4).SetEase(Ease.Linear));
        sequence.Append(transform.DOLocalMoveY(0f, occupyingTime/4).SetEase(Ease.Linear));
        // 设置循环模式为 PingPong
        sequence.SetLoops(-1, LoopType.Yoyo);
    }
    public bool OnBuoyDrop()
    {
        if(childTetris.Count==0)Init();
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
    /// <summary>
    /// 检查是否是本地模式
    /// </summary>
    /// <returns></returns>
    public bool Local()
    {
        if(RunModeData.CurrentRunMode == RunMode.Local)return true;
        return false;
    }
    public void Client_Reflash()
    {
        // if(!isClient)return;
        if(Local())return;
        foreach(var tetri in childTetris)
        {
            tetri.player = player;
            tetri.Display_playerColor();
        }
        transform.GetComponent<TetrisBuoySimple>().player = player;
    }
    public void GoOnTheBlocksCreator(bool oldValue,bool newValue)
    {
        if(Local())return;
        if(!blocksCreator){blocksCreator = FindObjectOfType<BlocksCreator>();}
        if(newValue)
        {
            transform.parent = blocksCreator.transform;
        }else
        {
            transform.parent = null;
        }
    }
    public void DisPlayOnline(bool isVisble)
    {
        if(!isClient)return;
        if(Local())return;
        foreach(var tetri in childTetris)
        {
            tetri.Display_playerColor(isVisble);
        }
    }
    
    void SyncOtherTetrisComponent(int sID)
    {
        if(Local())return;
        transform.GetComponent<TetrisBuoySimple>().serverID = sID;
    }
    
}

