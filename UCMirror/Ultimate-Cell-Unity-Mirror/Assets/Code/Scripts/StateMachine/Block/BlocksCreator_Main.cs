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

public class BlocksCreator_Main : SingletonNetwork<BlocksCreator_Main>
{

#region 数据对象
    public UnityAction OnBlocksInitEnd;
    public SpriteRenderer flowMask;
    public SpriteRenderer FlowMask
    {
        get
        {
            if(!flowMask)flowMask = transform.Find("FlowMask").GetComponent<SpriteRenderer>();
            return flowMask;
        }
        set
        {
            flowMask = value;
        }
    }
    public BlockDisplay block;
    public int x = 10;
    public int z = 20;
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
#endregion 数据对象
#region 联网数据对象
    public struct Server_BockChanged
    {
        public Vector2 PosId;
        public int State;
    }
    [Header("联网")]
    [SyncVar(hook = nameof(OnBlockNeedChange))]
    public Server_BockChanged sync_blockChanged;
    Server_BockChanged server_blockChanged;
    private Stack<Server_BockChanged> dataStack;
#endregion 联网数据对象
#region 数据关系
    public void Start()
    {   
        FlowMask.color = new Color(0.0f,0.0f,0.0f,0.0f);
        dataStack = new Stack<Server_BockChanged>();
        CreateBlocks();

        Invoke(nameof(LateStart),0.1f);
        OnBlocksInitEnd += () =>
        {
            
            // 初始化可放置区域
            InitPutzone();
            if(Local())
            {
                Event_ReflashPlayerBlocksOccupied();
                BlocksReferee.Active();
                blocks.ForEach((block) => {
                    block.GetComponent<BlockTetriHandler>().OnBlockTetriStateChanged += Event_ReflashPlayerBlocksOccupied;
                    // block.GetComponent<BlockTetriHandler>().OnBlockTetriStateChanged += BlocksReferee.CheckLose;
                });
                blocks.ForEach((block) => {
                    block.GetComponent<BlockTetriHandler>().OnBlockTetriStateChanged += Event_BlocksCounterInvoke;
                });
                // 道具生成
                // BlocksProps.Generate(PropsData.PropsState.ChainBall);
                BlocksProps.Generate(PropsData.PropsState.MoveDirectionChanger);
                BlocksProps.Generate(PropsData.PropsState.Obstacle);
            }else
            {
                ServerLogic.Local_palayer = Player.NotReady;
                ServerLogic.On_Local_palayer_ready += Client_Event_On_Local_player_ready;
                if(!isServer)return;
                
                Event_ReflashPlayerBlocksOccupied();
                blocks.ForEach((block) => {
                    block.GetComponent<BlockTetriHandler>().OnBlockTetriStateChanged += Server_Event_OnBlockTetriStateChanged;
                });
                blocks.ForEach((block) => {
                    block.GetComponent<BlockTetriHandler>().OnBlockTetriStateChanged += Event_ReflashPlayerBlocksOccupied;
                });
                blocks.ForEach((block) => {
                    block.GetComponent<BlockTetriHandler>().OnBlockTetriStateChanged += Event_BlocksCounterInvoke;
                });
                // 道具生成
                // BlocksProps.Generate(PropsData.PropsState.ChainBall);
                BlocksProps.Generate(PropsData.PropsState.MoveDirectionChanger);
                BlocksProps.Generate(PropsData.PropsState.Obstacle);

                InvokeRepeating(nameof(ProcessDataFromStack),0.1f,0.1f);
            }
        };
        UIData.OnPlayer1MoraleAccumulationMaxed += BlocksProps.Event_GenerateChainBall_MoraleAccumulationMaxed;
        UIData.OnPlayer2MoraleAccumulationMaxed += BlocksProps.Event_GenerateChainBall_MoraleAccumulationMaxed;
    }
    void OnDisable()
    {
        UIData.OnPlayer1MoraleAccumulationMaxed -= BlocksProps.Event_GenerateChainBall_MoraleAccumulationMaxed;
        UIData.OnPlayer2MoraleAccumulationMaxed -= BlocksProps.Event_GenerateChainBall_MoraleAccumulationMaxed;
        ServerLogic.On_Local_palayer_ready -= Client_Event_On_Local_player_ready;
    }
#endregion 数据关系
#region 数据操作
    public void Event_BlocksCounterInvoke(Vector2 posId = default(Vector2), int state = 0)
    {
        BlocksCounter.CheckFullRows();
        BlocksCounter.OnStateChange();
    }
    public void Event_ReflashPlayerBlocksOccupied(Vector2 posId = default(Vector2), int state = 0)
    {
        BlocksData.peace_numb = blocks.Where((block) => block.GetComponent<BlockTetriHandler>().State == BlockTetriHandler.BlockTetriState.Peace).Count();
        BlocksData.Player1_numb = blocks.Where((block) => block.GetComponent<BlockTetriHandler>().State == BlockTetriHandler.BlockTetriState.Occupied_Player1).Count();
        BlocksData.Player2_numb = blocks.Where((block) => block.GetComponent<BlockTetriHandler>().State == BlockTetriHandler.BlockTetriState.Occupied_Player2).Count();
        if(!blocksUI)blocksUI = FindObjectOfType<BlocksUI>();
        blocksUI.Display_Process();
    }
    void LateStart()
    {
        // Debug.Log(FindObjectsOfType<IdelBox>().Length);
        if(FindObjectsOfType<IdelBox>().Length == 0)Invoke(nameof(LateStart),0.1f);
        FindObjectsOfType<IdelBox>().ToList().ForEach((box) => {
            box.OnTetriBeginDrag += Event_OnListenBlocksMoveStart;
            box.OnTetriEndDrag += Event_OnListenBlocksMoveEnd;
        });
        FindObjectsOfType<BuoyInfo>().ToList().ForEach((buoy) => {
            buoy.OnBuoyDrag += Event_OnListenBlocksMoveStart;
            buoy.OnBuoyEndDrag += Event_OnListenBlocksMoveEnd;
        });
    }
    public void Event_OnListenBlocksMoveStart()
    {
        if(outflowTweener!=null){outflowTweener.Kill(); outflowTweener = null;}
        flowMask.color = new Color(0.0f,0.0f,0.0f,0.55f);
        // foreach(BlockDisplay block in blocks)
        // {
        //     block.InFlow();
        // }
        FindObjectsOfType<TetriBlockSimple>().ToList().ForEach((tetri) => {    
                tetri.InFlow();
                tetri.GetComponent<TetriBlockSimple>().InFlow();
        });
        transform.position = originPos;
        inflowTweener = transform.DOMoveY(originPos.y+0.1f,0.5f).SetEase(Ease.OutCirc);
    }
    public void Event_OnListenBlocksMoveEnd()
    {
        if(inflowTweener!=null){inflowTweener.Kill(); inflowTweener = null;}
        if(!FlowMask)return;
        FlowMask.color = new Color(0.0f,0.0f,0.0f,0.0f);
        // foreach(BlockDisplay block in blocks)
        // {
        //     block.OutFlow();
        // }
        FindObjectsOfType<TetriBlockSimple>().ToList().ForEach((tetri) => {    
                tetri.OutFlow();
                tetri.GetComponent<TetriBlockSimple>().OutFlow();
        });
       
        outflowTweener = transform.DOMoveY(originPos.y,0.5f).SetEase(Ease.OutBounce);
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
               // 以左下角为原点生成（方便计算）
               InstantiateBlock(i,j);

            }
        }
        Invoke(nameof(SetPosition),0.1f);
    }
    void SetPosition()
    {
        Vector3 pos = new Vector3(-15f,0,-8f);
        Vector3 scale = new Vector3(1.62f,1.5f,1.37f);
        Vector3 rot = new Vector3(5.88f,0f,0f);
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
    
#endregion 数据操作
#region 联网数据操作
    public bool Local()
    {
        if(RunModeData.CurrentRunMode == RunMode.Local)return true;
        return false;
    }
    public void BlocksUIActive()
    {
        
        if(!isServer)return;
        string music_BattlefieldBackground = "Music_BattlefieldBackground";
        AudioSystemManager.Instance.PlayMusic(music_BattlefieldBackground, 99);
        BlocksReferee.Active();
        Client_PlayMusic(music_BattlefieldBackground, 99);
    }
    [ClientRpc]
    void Client_PlayMusic(string musicName, int loopTime)
    {
        AudioSystemManager.Instance.PlayMusic(musicName, loopTime);
    }
    [Client]
    void OnBlockNeedChange(Server_BockChanged previousData, Server_BockChanged newData)
    {
        blocks.Find((block) => block.posId == newData.PosId).GetComponent<BlockTetriHandler>().state = (BlockTetriHandler.BlockTetriState)newData.State;
    }
    [Server]
    void Server_Event_OnBlockTetriStateChanged(Vector2 posId, int state)
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
    [Client]
    public void Client_Event_On_Local_player_ready()
    {
        if(ServerLogic.Local_palayer == Player.Player1)
        {
            transform.localPosition = new Vector3(transform.localPosition.x+2,transform.localPosition.y,transform.localPosition.z);
            originPos = transform.position;
        }else if(ServerLogic.Local_palayer == Player.Player2)
        {
           transform.localPosition = new Vector3(transform.localPosition.x-2,transform.localPosition.y,transform.localPosition.z);
           originPos = transform.position;
        }

    }
#endregion 联网数据操作
}
