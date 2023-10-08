using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ResManager;
using DG.Tweening;
using UnityEngine.Events;
using System.Linq;
using Mirror;
using UC_PlayerData;
public struct Server_BockChanged
{
    public Vector2 PosId;
    public int State;
}
public class BlocksCreator_Main : SingletonNetwork<BlocksCreator_Main>
{

#region 数据对象
    // 通讯对象
    // private GameObject sceneLoader;
    // private CommunicationInteractionManager CommunicationManager;
    // private BroadcastClass broadcastClass;
    public UnityAction OnBlocksInitEnd;
    /// <summary>
    /// 心流模式遮罩
    /// </summary>
    public SpriteRenderer FlowMask;
    /// <summary>
    /// 单个砖块表现类
    /// </summary>
    public BlockDisplay block;
    /// <summary>
    /// 宽
    /// </summary>
    public int x = 10;
    /// <summary>
    /// 长
    /// </summary>
    public int z = 20;
    Vector2 pos = new Vector2(0,3);
    public List<BlockDisplay> blocks = new();
    Vector3 originPos = Vector3.zero;
    private Tweener inflowTweener;
    private Tweener outflowTweener;
    BlocksUI blocksUI;
    public BlocksUI BlocksUI
    {
        get
        {
            if(!blocksUI)blocksUI = FindObjectOfType<BlocksUI>();
            return blocksUI;
        }
    }
    BlocksCounter blocksCounter;
    public BlocksCounter BlocksCounter
    {
        get
        {
            if(!blocksCounter)blocksCounter = transform.GetComponent<BlocksCounter>();
            return blocksCounter;
        }
    }
    BlocksProps blocksProps;
    public BlocksProps BlocksProps
    {
        get
        {
            if(!blocksProps)blocksProps = GetComponent<BlocksProps>();
            return blocksProps;
        }
    }
    private BlocksEffects blocksEffects;
    public BlocksEffects BlocksEffects
    {
        get
        {
            if(!blocksEffects)blocksEffects = GetComponent<BlocksEffects>();
            return blocksEffects;
        }
    }
    private BlocksReferee blocksReferee;
    public BlocksReferee BlocksReferee
    {
        get
        {
            if(!blocksReferee)blocksReferee = GetComponent<BlocksReferee>();
            return blocksReferee;
        }
    }
    [Header("联网")]
    [SyncVar(hook = nameof(OnBlockNeedChange))]
    public Server_BockChanged sync_blockChanged;
    Server_BockChanged server_blockChanged;
    private Stack<Server_BockChanged> dataStack;
#endregion 数据对象
#region 数据关系
    private void Start()
    {   
        FlowMask.color = new Color(0.0f,0.0f,0.0f,0.0f);
        dataStack = new Stack<Server_BockChanged>();
        CreateBlocks();

        Invoke(nameof(LateStart),0.1f);
        OnBlocksInitEnd += () =>
        {
            // 初始化可放置区域
            InitPutzone();
            ReflashPlayerBlocksOccupied();
            // 开始计时
            BlocksReferee.Active();
            // 监听砖块变化事件
            if(RunModeData.CurrentRunMode == RunMode.Local)
            {
                blocks.ForEach((block) => {
                    block.GetComponent<BlockTetriHandler>().OnBlockTetriStateChanged += ReflashPlayerBlocksOccupied;
                    // block.GetComponent<BlockTetriHandler>().OnBlockTetriStateChanged += BlocksReferee.CheckLose;
                });
                blocks.ForEach((block) => {
                    block.GetComponent<BlockTetriHandler>().OnBlockTetriStateChanged += BlocksCounterInvoke;
                });
                // 道具生成
                BlocksProps.Generate(PropsData.PropsState.ChainBall);
                BlocksProps.Generate(PropsData.PropsState.MoveDirectionChanger);
                BlocksProps.Generate(PropsData.PropsState.Obstacle);
            }else
            {
                if(!isServer)return;
                blocks.ForEach((block) => {
                    block.GetComponent<BlockTetriHandler>().OnBlockTetriStateChanged += Server_OnBlockTetriStateChanged;
                });
                InvokeRepeating(nameof(ProcessDataFromStack),0.1f,0.1f);
            }
        };
    }
#endregion 数据关系
#region 数据操作
    public void BlocksCounterInvoke(Vector2 posId = default(Vector2), int state = 0)
    {
        BlocksCounter.CheckFullRows();
        BlocksCounter.OnStateChange();
    }
    public void ReflashPlayerBlocksOccupied(Vector2 posId = default(Vector2), int state = 0)
    {
        BlocksData.peace_numb = blocks.Where((block) => block.GetComponent<BlockTetriHandler>().State == BlockTetriHandler.BlockTetriState.Peace).Count();
        BlocksData.Player1_numb = blocks.Where((block) => block.GetComponent<BlockTetriHandler>().State == BlockTetriHandler.BlockTetriState.Occupied_Player1).Count();
        BlocksData.Player2_numb = blocks.Where((block) => block.GetComponent<BlockTetriHandler>().State == BlockTetriHandler.BlockTetriState.Occupied_Player2).Count();
        if(!blocksUI)blocksUI = FindObjectOfType<BlocksUI>();
        blocksUI.Display_Process();
    }
    void LateStart()
    {
        FindObjectsOfType<IdelBox>().ToList().ForEach((box) => {
            box.OnTetriBeginDrag += OnListenBlocksMoveStart;
            box.OnTetriEndDrag += OnListenBlocksMoveEnd;
        });
        FindObjectsOfType<BuoyInfo>().ToList().ForEach((buoy) => {
            buoy.OnBuoyDrag += OnListenBlocksMoveStart;
            buoy.OnBuoyEndDrag += OnListenBlocksMoveEnd;
        });
    }
    void OnListenBlocksMoveStart()
    {
        
        if(outflowTweener!=null){outflowTweener.Kill();}
        FlowMask.color = new Color(0.0f,0.0f,0.0f,0.3f);

        foreach(BlockDisplay block in blocks)
        {
            block.InFlow();
        }
        FindObjectsOfType<TetriBlockSimple>().ToList().ForEach((tetri) => {    
                tetri.InFlow();
                tetri.GetComponent<TetriBlockSimple>().InFlow();
        });
        transform.position = originPos;
        inflowTweener = transform.DOMoveY(originPos.y+0.1f,0.5f).SetEase(Ease.OutCirc);
    }
    void OnListenBlocksMoveEnd()
    {
        
        if(inflowTweener!=null){inflowTweener.Kill();}

        FlowMask.color = new Color(0.0f,0.0f,0.0f,0.0f);

        foreach(BlockDisplay block in blocks)
        {
            block.OutFlow();
        }
        FindObjectsOfType<TetriBlockSimple>().ToList().ForEach((tetri) => {    
                tetri.OutFlow();
                tetri.GetComponent<TetriBlockSimple>().OutFlow();
        });
       
        outflowTweener = transform.DOMoveY(originPos.y,0.5f).SetEase(Ease.OutBounce);
    }

    void BrightBlock()
    {
        // 假数据
        Vector2 lastPos = pos;
        pos.x += 1;
        
        if(blocks.Count == 0)
        {
            CreateBlocks();
        }else
        {
            blocks.Find((block) => block.posId == pos).GetComponent<BlockDisplay>().Bright(EventType.UnitColor.green);
            blocks.Find((block) => block.posId == lastPos).GetComponent<BlockDisplay>().NotBright();
        }
    }

    void CreateBlocks()
    {
        blocks.Clear();
        blocks = new();
        for(int i = 0; i < z; i++)
        {
            for(int j = 0; j < x; j++)
            {
               // 以中间为原点生成
                // InstantiateBlock(i/2+1,j/2+1);
                // InstantiateBlock(-i/2,j/2+1);
                // InstantiateBlock(i/2+1,-j/2);
                // InstantiateBlock(-i/2,-j/2);
               // 以左下角为原点生成（方便和逻辑层通讯）
               InstantiateBlock(i,j);

            }
        }
        Invoke(nameof(SetPosition),0.1f);
    }
    void SetPosition()
    {
        Vector3 pos = new Vector3(-15f,0,-8f);
        Vector3 scale = new Vector3(1.62f,1.5f,1.37f);
        Vector3 rot = new Vector3(7.57f,0f,0f);
        transform.DOMove(pos,0.1f).onComplete = () => {
            originPos = transform.position;
        };
        transform.DOScale(scale,0.1f);
        transform.DORotate(rot,0.1f).onComplete = () => {
            OnBlocksInitEnd?.Invoke();
        };
    }
    void DestoryBlocks()
    {
        foreach(BlockDisplay block in blocks)
        {
            DestroyImmediate(block.gameObject);
        }
        blocks.Clear();
        blocks = new();
        transform.localScale = Vector3.one;
        transform.localPosition = new Vector3(0f, 0f, 0f);
    }
    void InstantiateBlock(int i, int j)
    {
        BlockDisplay blockTemp = Instantiate(block, new Vector3(i, 0f, j), Quaternion.Euler(new Vector3(90f, 0f, 0f)),transform);
        blockTemp.posId = new Vector2(i, j);
        blockTemp.finalHigh = 0.15f;
        blocks.Add(blockTemp);
    }
    void InitPutzone()
    {
        foreach(var blockTemp in blocks)
        {
            float i = blockTemp.posId.x;
            // 和平但不可以被对方放置
            if(i>=0 && i<=9-1)
            {
                blockTemp.transform.GetComponent<BlockTetriHandler>().State = BlockTetriHandler.BlockTetriState.Peace_Player1;
            }else if(i>= 10+1 && i<=20)
            {
                blockTemp.transform.GetComponent<BlockTetriHandler>().State = BlockTetriHandler.BlockTetriState.Peace_Player2;
            }
            // 初始放置区域 中线是 9 和 10 列
            if(i>=0 && i<=1)
            {
                blockTemp.transform.GetComponent<BlockTetriHandler>().State = BlockTetriHandler.BlockTetriState.Occupied_Player1;
            }else if(i>=18 && i<=20)
            {
                blockTemp.transform.GetComponent<BlockTetriHandler>().State = BlockTetriHandler.BlockTetriState.Occupied_Player2;
            }
        }
    }
    // ------------------联网------------------
    [Client]
    void OnBlockNeedChange(Server_BockChanged previousData, Server_BockChanged newData)
    {
        blocks.Find((block) => block.posId == newData.PosId).GetComponent<BlockTetriHandler>().state = (BlockTetriHandler.BlockTetriState)newData.State;
    }
    [Server]
    void Server_OnBlockTetriStateChanged(Vector2 posId, int state)
    {
        server_blockChanged = new Server_BockChanged()
        {
            PosId = posId,
            State = state,
        };
        // 将收到的数据存入栈中
        dataStack.Push(server_blockChanged);
    }
    [Server]
    public void ProcessDataFromStack()
    {
        if(dataStack.Count == 0) return;
        sync_blockChanged = dataStack.Pop();
    }
#endregion 数据操作
}
