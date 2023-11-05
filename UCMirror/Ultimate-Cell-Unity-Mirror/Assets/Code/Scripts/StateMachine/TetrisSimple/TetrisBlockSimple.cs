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
#region 数据对象
    [SyncVar]
    public Vector2 posId;
    public float occupyingTime = 3f;
    public float OccupyingTime
    {
        get
        {
            return occupyingTime;
        }
        set
        {
            
            if(value<=UnitData.MinOccupyingTime){value= UnitData.MinOccupyingTime;}
            if(value>=UnitData.MaxOccupyingTime){value= UnitData.MaxOccupyingTime;}
            occupyingTime = value;
        }
    }
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
            foreach(var tetri in ChildTetris)
            {
                tetri.player = player;
            }
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
    public List<TetriBlockSimple> ChildTetris
    {
        get
        {
            if(childTetris.Count != 0) return childTetris;
            foreach (Transform child in transform)
            {
                if(child.GetComponent<TetriBlockSimple>() == null)continue;
                TetriBlockSimple tetriBuoySimple = child.GetComponent<TetriBlockSimple>();
                if(childTetris.Contains(tetriBuoySimple))continue;
                childTetris.Add(tetriBuoySimple);
            }
            return childTetris;
        }
    }
    public UnityAction OnTetrisMoveing;
    public BlocksCreator_Main blocksCreator;
    public BlocksCreator_Main BlocksCreator
    {
        get
        {
            if(!blocksCreator)blocksCreator = FindObjectOfType<BlocksCreator_Main>();
            return blocksCreator;
        }
    }
    public Dictionary<TetriBlockSimple,BlockTetriHandler> TB_cache = new();
    public UnityAction<Dictionary<TetriBuoySimple,BlockBuoyHandler>> OnCacheUpdateForBuoyMarkers;
    public UnityAction OnUpdatDisplay;
    public UnityAction OnStartMove;
    public UnityAction<bool> OnRotate;
    private Stack<Vector3> positionStack;
    public int moveStepX = 1;
    public int moveStepZ = 1;
    private bool moveUp = false;
    public bool MoveUp
    {
        get
        {
            return moveUp;
        }
        set
        {
            // Debug.Log($"移动方向改变：moveUp:{moveUp}");
            // if(moveUp == value)return;
            moveUp = value;
            Stop();
            Active_Z();
            Move_Z();
        }
    }
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
    
#endregion 数据对象
#region 联网数据对象
    
#endregion 联网数据对象
#region 数据关系
    void Awake()
    {
       color = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
       color = new Color(color.r,color.g,color.b,1f);
       onTheBlocksCreator = false;
       blocksCreator = FindObjectOfType<BlocksCreator_Main>();
    }
    void Start()
    {
       foreach (Transform child in transform)
       {
           // ChildTetris.Add(child.GetComponent<TetriBlockSimple>());
           child.GetComponent<TetriBlockSimple>().CantPutCallback += Event_CantPutAction;
           child.GetComponent<TetriBlockSimple>().player = player;
       }
       positionStack = new Stack<Vector3>();
       TB_cache = new();
       if(player == Player.Player1)
       {
            UIData.OnPlayer1MoraleAccumulationMaxed += (Player player) => {
                OccupyingTime/=2;
            };
       }else if (player == Player.Player2)
       {
            UIData.OnPlayer2MoraleAccumulationMaxed += (Player player) => {
                OccupyingTime/=2;
            };
       }
       if(Local())return;
       if(!isServer)return;
      
    }
    public void Init()
    {
       color = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
       color = new Color(color.r,color.g,color.b,1f);
       onTheBlocksCreator = false;
       blocksCreator = FindObjectOfType<BlocksCreator_Main>();
       foreach (Transform child in transform)
       {
           ChildTetris.Add(child.GetComponent<TetriBlockSimple>());
           child.GetComponent<TetriBlockSimple>().CantPutCallback += Event_CantPutAction;
           child.GetComponent<TetriBlockSimple>().player = player;
       }
       positionStack = new Stack<Vector3>();
       TB_cache = new();
       
    }
#endregion 数据关系
#region 数据操作
    public bool Active()
    {
        Stop_X();
        Stop_Z();
        if(!transform.parent)return false;
        blocksCreator = transform.parent.GetComponent<BlocksCreator_Main>();
        foreach (TetriBlockSimple child in ChildTetris)
        {
            if(child == null)continue;
            child.player = player;
            child.Active();
            child.DoGroupMoveCheck();
        }
        EvaluateCollision();
        // 道具检测
        TetrisUnitSimple.InitPropDoing();
        return true;
    }
    public bool Active_X()
    {
        
        if(!transform.parent)return false;
        blocksCreator = transform.parent.GetComponent<BlocksCreator_Main>();
        foreach (TetriBlockSimple child in ChildTetris)
        {
            if(child == null)continue;
            child.player = player;
            child.Active();
            child.DoGroupMoveCheck();
        }
        Stop_Z();
        EvaluatePioneers_X();
        EvaluateCollision();
        Move_X();
        transform.GetComponent<TetrisBuoySimple>().Display_Active();
        return true;
    }
    public bool Active_Z()
    {
        if(!transform.parent)return false;
        blocksCreator = transform.parent.GetComponent<BlocksCreator_Main>();
        foreach (TetriBlockSimple child in ChildTetris)
        {
            if(child == null)continue;
            child.player = player;
            child.Active();
            child.DoGroupMoveCheck();
        }
        Stop_X();
        EvaluatePioneers_Z();
        EvaluateCollision();
        Move_Z();
        transform.GetComponent<TetrisBuoySimple>().Display_Active();
        return true;
    }
    public bool ColliderCheck()
    {
        List<bool> colliders = new();
        foreach(var childTetri in ChildTetris)
        {
            colliders.Add(childTetri.CheckCollider());
        }
        bool allTrue = colliders.All(b => b);
        return allTrue;
    }
    public bool ColliderCheckOnEndDrag()
    {
        List<bool> colliders = new();
        foreach(var childTetri in ChildTetris)
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
        foreach(var child in ChildTetris)
        {
            child.FailToCreat();
        }
    }
    public void SuccessToCreat()
    {
        foreach(var child in ChildTetris)
        {
            child.SuccessToCreat();
        }
    }
    void Event_CantPutAction(TetriBlockSimple tetriBlock)
    {
        RollbackPosition();
    }
    public void CantPutAction()
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
        foreach (TetriBlockSimple tetriBlock in ChildTetris)
        {
            if(!tetriBlock)continue;
            BlockTetriHandler blockCurrent = null;
            blockCurrent = blocksCreator.blocks.Find((block) => block.posId == new Vector2(tetriBlock.PosId.x,tetriBlock.PosId.y)).GetComponent<BlockTetriHandler>();
            if(!blockCurrent)continue;
            blockCurrent.tetriBlockSimpleHolder = tetriBlock;
            tetriBlock.currentBlockTetriHandler = blockCurrent;
            // 可视化
            // blockCurrent.transform.localScale -= Vector3.one*0.1f;
            // tetriBlock.transform.localScale -= Vector3.one*0.1f;
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
    public void EvaluatePioneers_X()
    {
        pioneerTetris.Clear();
        // 取前方没有砖块的砖块
        if(player == Player.Player1)
        {
            foreach(var childTetri in ChildTetris)
            {
                bool P1FrontObj = ChildTetris.FirstOrDefault(obj => obj.PosId == new Vector2(childTetri.PosId.x+1,childTetri.PosId.y));
                if(P1FrontObj)continue;
                pioneerTetris.Add(childTetri);
            }
        }else if (player == Player.Player2)
        {
            foreach(var childTetri in ChildTetris)
            {
                bool P2FrontObj = ChildTetris.FirstOrDefault(obj => obj.PosId == new Vector2(childTetri.PosId.x-1,childTetri.PosId.y));
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
    public void EvaluatePioneers_Z()
    {
        pioneerTetris.Clear();
        // 取上下方没有砖块的砖块
        if(MoveUp)
        {
            foreach(var childTetri in ChildTetris)
            {
                bool upFrontObj = ChildTetris.FirstOrDefault(obj => obj.PosId == new Vector2(childTetri.PosId.x,childTetri.PosId.y+1));
                if(upFrontObj)continue;
                pioneerTetris.Add(childTetri);
            }
        }else
        {
            foreach(var childTetri in ChildTetris)
            {
                bool downFrontObj = ChildTetris.FirstOrDefault(obj => obj.PosId == new Vector2(childTetri.PosId.x,childTetri.PosId.y-1));
                if(downFrontObj)continue;
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
    public void Stop(bool cancleOccupied = true)
    {
        Stop_X();
        Stop_Z();
        if(!cancleOccupied)return;
        CancleOccupied();
    }
    public void Stop_X()
    {
        moveStepX = 0;
        if(!IsInvoking(nameof(MoveActive_X)))return;
        CancelInvoke(nameof(MoveActive_X));
    }
    public void Stop_Z()
    {
        moveStepZ = 0;
        if(!IsInvoking(nameof(MoveActive_Z)))return;
        CancelInvoke(nameof(MoveActive_Z));
    }
    public void Move()
    {
        if(IsMoving())return;
        Move_X();
    }
    public void Move_X()
    {
        moveStepX = 1;
        InvokeRepeating(nameof(MoveActive_X),0,OccupyingTime);
    }
    public void Move_Z()
    {
        moveStepZ = MoveUp ? 1 : -1;
        InvokeRepeating(nameof(MoveActive_Z),0,OccupyingTime);
    }
    void MoveActive_X()
    {
        // Debug.Log($"moveStepX!!!:{ValidMove_X()}");
        if(!ValidMove_X())return;
        OnStartMove?.Invoke();
        foreach (TetriBlockSimple child in ChildTetris)
        {
            if(!child)continue;
            child.DoGroupMoveCheck();
        }
        if(player == Player.Player1)
        {
            transform.localPosition += new Vector3(moveStepX,0,0);
        }
        else if(player == Player.Player2)
        {
            transform.localPosition += new Vector3(-moveStepX,0,0);
        }
        posId = new Vector2(transform.localPosition.x,transform.localPosition.z);
        OnTetrisMoveing?.Invoke();
        EvaluateCollision();
        // EvaluatePioneers_X();
        
    }
    void MoveActive_Z()
    {
        if(!ValidMove_Z())return;
        // Debug.Log($"moveStepZ!!!:{moveStepZ}");
        OnStartMove?.Invoke();
        foreach (TetriBlockSimple child in ChildTetris)
        {
            if(!child)continue;
            child.DoGroupMoveCheck();
        }
        transform.localPosition += new Vector3(0,0,moveStepZ);
        posId = new Vector2(transform.localPosition.x,transform.localPosition.z);
        OnTetrisMoveing?.Invoke();
        EvaluateCollision();
        // EvaluatePioneers_Z();
        
    }
    public bool IsMoving()
    {
        return IsInvoking(nameof(MoveActive_X)) || IsInvoking(nameof(MoveActive_Z));
    }
    public void Rotate(Vector3 axis)
    {
        
        bool reverse = false;
        OnRotate?.Invoke(reverse);
        transform.RotateAround(transform.TransformPoint(rotationPoint), axis, 90);
        foreach (TetriBlockSimple child in ChildTetris)
        {
            if(!child)continue;
            child.RotateForRecalculateDisplayRange(reverse);
        }
    }
    public void RotateReverse(Vector3 axis)
    {
        bool reverse = true;
        OnRotate?.Invoke(reverse);
        transform.RotateAround(transform.TransformPoint(rotationPoint), axis, -90);
        foreach (TetriBlockSimple child in ChildTetris)
        {
            if(!child)continue;
            child.RotateForRecalculateDisplayRange(reverse);
        }
    }
    bool ValidMove_X()
    {
        switch (tetrisCheckMode)
        {
            case TetrisCheckMode.Create:
               return CreateValidMove();
            case TetrisCheckMode.Drop:
               return DropValidMove();
            case TetrisCheckMode.Normal:
               return NormalValidMove_X();
            default:
                return false;
        }
        
    }
    bool ValidMove_Z()
    {
        if(tetrisCheckMode != TetrisCheckMode.Normal)
        {
            OnStartMove?.Invoke();
            tetrisCheckMode = TetrisCheckMode.Normal;
            return false;
        }
        List<bool> condition = new();
        foreach(var pineer in pioneerTetris)
        {
            if(!pineer)continue;
            BlockTetriHandler blockCurrent = pineer.CurrentBlock();
            condition.Add(blockCurrent);
            BlockTetriHandler blockNext = pineer.NextBlock_Z(MoveUp);
            condition.Add(blockNext);
            condition.Add(pineer.CanMove);
            condition.Add(pineer.BlockNextCheckBuoy(blockNext));
            if(!blockNext)continue;
            condition.Add(!blockNext.GetComponent<BlockPropsState>().stopMoveProp);
        }
        bool allTrue = condition.All(b => b);
        return allTrue;
    }
    bool NormalValidMove_X()
    {
        moveStepX = 1;
        List<bool> condition = new();
        foreach(var pineer in pioneerTetris)
        {
            if(!pineer)continue;
            // pineer.transform.localScale = Vector3.one*3f;
            BlockTetriHandler blockCurrent = pineer.CurrentBlock();
            condition.Add(blockCurrent);
            BlockTetriHandler blockNext = pineer.NextBlock_X();
            condition.Add(blockNext);
            condition.Add(pineer.CanMove);
            condition.Add(pineer.BlockNextCheckBuoy(blockNext));
            if(!blockNext)continue;
            condition.Add(!blockNext.GetComponent<BlockPropsState>().stopMoveProp);
            // Debug.Log($"cccccc:{pineer.CanMove}");
        }
        bool allTrue = condition.All(b => b);
        return allTrue;
    }
    bool DropValidMove()
    {
        moveStepX = 0;
        OnStartMove?.Invoke();
        tetrisCheckMode = TetrisCheckMode.Normal;
        foreach(var child in ChildTetris)
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
            BlockTetriHandler blockNext = moveStepZ == 1 ? pineer.NextBlock_Z(MoveUp):pineer.NextBlock_X();
            condition.Add(blockNext);
            condition.Add(pineer.CanMove);
            condition.Add(pineer.BlockNextCheckBuoy(blockNext));
        }
        foreach (TetriBlockSimple child in ChildTetris)
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
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(0.2f, OccupyingTime/4).SetEase(Ease.Linear));
        sequence.Append(transform.DOLocalMoveY(0f, OccupyingTime/4).SetEase(Ease.Linear));
        sequence.SetLoops(-1, LoopType.Yoyo);
    }
    public bool OnBuoyDrop()
    {
        if(ChildTetris.Count==0)Init();
        List<bool> buoyDrop = new();
        // 所有未占领的砖块恢复和平状态
        foreach(var tetri in ChildTetris)
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
    void CancleOccupied()
    {
        foreach(var tetri in ChildTetris)
        {
            tetri.CancleOccupied();
        }
    }
    public void Reset()
    {
        foreach(var tetri in ChildTetris)
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
        foreach(var tetri in ChildTetris)
        {
            tetri.player = player;
            tetri.Display_playerColor();
        }
        transform.GetComponent<TetrisBuoySimple>().player = player;
    }
    public void GoOnTheBlocksCreator(bool oldValue,bool newValue)
    {
        if(Local())return;
        if(!blocksCreator){blocksCreator = FindObjectOfType<BlocksCreator_Main>();}
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
        foreach(var tetri in ChildTetris)
        {
            tetri.Display_playerColor(isVisble);
        }
    }
    
    void SyncOtherTetrisComponent(int sID)
    {
        if(Local())return;
        transform.GetComponent<TetrisBuoySimple>().serverID = sID;
    }
#endregion 数据操作
#region 联网数据操作
    public void Client_Rotate(Vector3 axis)
    {
        bool reverse = false;
        OnRotate?.Invoke(reverse);
        // transform.RotateAround(transform.TransformPoint(rotationPoint), axis, 90);
        foreach (TetriBlockSimple child in ChildTetris)
        {
            if(!child)continue;
            child.RotateForRecalculateDisplayRange(reverse);
        }
    }
    public void Server_Rotate(Vector3 axis)
    {
        bool reverse = false;
        OnRotate?.Invoke(reverse);
        transform.RotateAround(transform.TransformPoint(rotationPoint), axis, 90);
        // Debug.Log($"rotateTimes:{ChildTetris.Count}");
        foreach (TetriBlockSimple child in ChildTetris)
        {
            if(!child)continue;
            child.RotateForRecalculateDisplayRange(reverse);
        }
    }
    
#endregion 联网数据操作
}

