using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ResManager;
using DG.Tweening;
using UnityEngine.Events;

public class BlocksCreator : Singleton<BlocksCreator>
{
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
    // 点亮方块位置暂存区
    List<Vector2> lastPosList = new List<Vector2>();

    // 销毁方块位置暂存区
    List<Vector2> lastDestoryPosList;

    // 销毁方块位置

    private void Start()
    {
        // // TODO 暂时获取方式
        // sceneLoader = GameObject.Find(nameof(LanNetWorkManager)).gameObject;

        // // 全局通信方法管理
        // CommunicationManager = sceneLoader.GetComponent<CommunicationInteractionManager>();

        // // 全局通信事件注册类
        // broadcastClass = sceneLoader.GetComponent<BroadcastClass>();

        // // 砖块信息新建监听
        // broadcastClass.TetrisInfoCreate += OnlistenTetrisUpdateLight;

        // // 砖块信息更新监听
        // broadcastClass.TetrisInfoUpdate += OnlistenTetrisUpdateLight;

        // // 砖块整体销毁监听
        // broadcastClass.BlocksInfoDestory += OnListenBlocksDestoryLight;

        // // 砖块整体移动监听方法
        // broadcastClass.BlocksInfoMove += OnListenBlocksUpdateLight;

        // broadcastClass.BlocksReady += OnListenBlocksMoveStart;
        
        // broadcastClass.BlocksMoveEnd += OnListenBlocksMoveEnd;
        
        // broadcastClass.OnHeroUICancel += OnListenBlocksMoveEnd;
        

        FlowMask.color = new Color(0.0f,0.0f,0.0f,0.0f);

        CreateBlocks();
    }

    void OnListenBlocksMoveStart(Dictionary<int, TetrisClass> dic ,  string str)
    {
        FlowMask.color = new Color(0.0f,0.0f,0.0f,0.3f);

        foreach(BlockDisplay block in blocks)
        {
            block.InFlow();
        }

        Time.timeScale = 1.0f;
        transform.position = originPos;
        transform.DOMoveY(originPos.y+0.1f,0.5f).SetEase(Ease.OutCirc).onComplete = () =>
        {
            Time.timeScale = 1.0f;
        };
    }
    void OnListenBlocksMoveEnd(int index)
    {
        OnEnd();
    }
    void OnListenBlocksMoveEnd(string index)
    {
        OnEnd();
    }
    void OnEnd()
    {
        FlowMask.color = new Color(0.0f,0.0f,0.0f,0.0f);

        foreach(BlockDisplay block in blocks)
        {
            block.OutFlow();
        }

        Time.timeScale = 1.0f;
        transform.DOMoveY(originPos.y,0.5f).SetEase(Ease.OutBounce).onComplete = () =>
        {
           Time.timeScale = 1.0f;
        };
    }

    /// <summary>
    /// 砖块销毁监听事件（运行中砖块）
    /// </summary>
    /// <param name="info"></param>
    private void OnlistenBlockDestoryLight(List<Vector2> info)
    {
        foreach (Vector2 lastPos in info)
        {
            // blocks.Find((block) => block.posId == lastPos).GetComponent<BlockDisplay>().NotBright();
            NotBrightBlock(lastPos);
        }

    }

    /// <summary>
    /// 砖块更新监听事件
    /// </summary>
    /// <param name="info"></param>
    private void OnlistenTetrisUpdateLight(List<UnitInfoClass> info)
    {
        if (!info[0].CreateUnit) 
        {
            OnlistenBlockDestoryLight(lastPosList);
        }

        lastPosList = new();

        foreach (UnitInfoClass unitInfo in info)
        {
            lastPosList.Add(unitInfo.UnitPos);

            BrightBlock(unitInfo);
        }
    }

    /// <summary>
    /// 砖块销毁监听方法（触底砖块）
    /// </summary>
    /// <param name="info"></param>
    private void OnListenBlocksDestoryLight(List<UnitInfoClass> info)
    {
        foreach (UnitInfoClass unitInfo in info)
        {
            // blocks.Find((block) => block.posId == unitInfo.UnitPos).GetComponent<BlockDisplay>().NotBright();
            NotBrightBlock(unitInfo.UnitPos);
        }
    }

    /// <summary>
    /// 砖块整体移动监听
    /// </summary>
    /// <param name="info"></param>
    private void OnListenBlocksUpdateLight(List<UnitInfoClass> info)
    {
        foreach (UnitInfoClass unitInfo in info)
        {
            // blocks.Find((block) => block.posId == unitInfo.UnitPos).GetComponent<BlockDisplay>().Bright();
            BrightBlock(unitInfo);
        }
    }

    /// <summary>
    /// 点亮砖块
    /// </summary>
    /// <param name="unitInfo">砖块通讯信息</param>
    void BrightBlock(UnitInfoClass unitInfo) 
    {
        if (blocks.Count == 0)
        {
            CreateBlocks();
        }

        var block = blocks.Find((block) => block.posId == unitInfo.UnitPos);

        if (block != null)
        {
            block.GetComponent<BlockDisplay>().Bright(unitInfo.color);

        }
    }

    /// <summary>
    /// 晚安砖块
    /// </summary>
    /// <param name="Pos"></param>
    void NotBrightBlock(Vector2 Pos) 
    {
        var block = blocks.Find((block) => block.posId == Pos);

        if (block != null)
        {
            block.GetComponent<BlockDisplay>().NotBright();
        }
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
        // transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        // transform.localPosition = new Vector3(-15f, 15f, -8f);
        
        // transform.localScale = new Vector3(1.5f, 1.5f, 1.29f);
        // transform.localPosition = new Vector3(-14f, 12.87f, -7.32f);
        Invoke(nameof(SetPosition),0.1f);
    }
    void SetPosition()
    {
        Vector3 pos = new Vector3(-14f,0,-8f);
        Vector3 scale = new Vector3(1.62f,1.5f,1.37f);
        Vector3 rot = new Vector3(7.57f,0f,0f);
        transform.DOMove(pos,0.1f).onComplete = () => {
            originPos = transform.position;
        };
        transform.DOScale(scale,0.1f);
        transform.DORotate(rot,0.1f).onComplete = () => {
            foreach(BlockDisplay block in blocks)
            {
                if(block.transform.TryGetComponent<BuildingSlot>(out BuildingSlot slot))
                {
                    slot.slotPos = block.transform.position;
                    if(OnBlocksInitEnd!=null)
                    {
                        OnBlocksInitEnd();
                    }
                }
                
            }
            
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

    
}
