using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EventType;
using Button = UnityEngine.UI.Button;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Linq;
using PlayerData;
using UnityEngine.Events;

public class IdelBox : MonoBehaviour,
  IBeginDragHandler, 
  IDragHandler, 
  IEndDragHandler,
  IPointerDownHandler
{
   

    #region 数据对象
    public int idelId;
    public Player player = Player.NotReady;
    public LayerMask blockTargetMask;
    /// <summary>
    /// idel模板
    /// </summary>
    public IdelTemplate idelTemplate;
    /// <summary>
    /// idel按钮
    /// </summary>
    public Button IdelBtn;
    /// <summary>
    /// idel容器
    /// </summary>
    public GameObject Idel;
    public GameObject IdelWorldSpace;
    private Vector3 idelAnchoredPos;
    /// <summary>
    /// idel点击屏蔽
    /// </summary>
    public GameObject ImageBlock;
    /// <summary>
    /// idel加载倒计时
    /// </summary>
    public GameObject blockText;
    /// <summary>
    /// idel等级
    /// </summary>
    public EventType.BlocksGrade blockGrade;

    private ChangeLiquid changeLiquid;

    Vector3 originScale;

    const float X_space = 6.9f;
    const float Y_space = 10f;

    const float Max_space_X = X_space * 2;
    const float Min_space_X = - (X_space * 2);
    const float Max_space_Y = Y_space * 2;
    const float Min_space_Y = - (Y_space * 2);
    const int NotFlowOrder = 22;

    // 通信类
    // public BroadcastClass broadcastClass;
    MechanismInPut mechanismInPut;
    MechanismInPut.ModeTest modeTest;
    
    public TetrisBlockSimple[] tetrominoes;
    TetrisBlockSimple tetrominoe;

    public UnityAction<IdelBox> OnTetriBeginDrag;
    public UnityAction<IdelBox> OnTetriEndDrag;
    // 通信管理器
    // private CommunicationInteractionManager CommunicationManager;

    // 当前位置的砖块种类
    // private BlocksClass BlocksInfo;
    #endregion

    #region 数据关系
    void Start()
    {
        
        // 通信获取
        // TODO 暂时获取方式
        // CommunicationManager = GameObject.Find("LanNetWorkManager").gameObject.GetComponent<CommunicationInteractionManager>();
        // broadcastClass = CommunicationManager.GetComponent<BroadcastClass>();
        Init();
        Invoke(nameof(LateStart),0.1f);
    }
    void LateStart()
    {
        // mechanismInPut = FindObjectOfType<MechanismInPut>();
        switch(idelId)
        {
            case 1:
                IdelWorldSpace = transform.parent.parent.parent.Find("IdelBox1_GameObject").gameObject;
                break;
            case 2:
                IdelWorldSpace = transform.parent.parent.parent.Find("IdelBox2_GameObject").gameObject;
                break;
            case 3:
                IdelWorldSpace = transform.parent.parent.parent.Find("IdelBox3_GameObject").gameObject;
                break;
        }
        
        
    }
    Sequence sequence;
    public void OnBeginDrag(PointerEventData data)
    {
        
        // CommunicationManager.BlocksReady(BlocksInfo.TetrisInfo);
        
        // broadcastClass.BlockCreateDone += RmoveItemAndRefresh;
        
        // 心流模式
        OnTetriBeginDrag?.Invoke(this);
        Canvas idelCanvas = Idel.GetComponent<Canvas>();
        idelCanvas.sortingLayerName = "Flow";
        idelCanvas.sortingOrder = Dispaly.FlowOrder;
        RectTransform idelRT = Idel.GetComponent<RectTransform>();
        idelRT.localScale = originScale;
        idelRT.anchoredPosition3D = idelAnchoredPos;
        idelRT.DOScale(2.0f, 0.5f);
        idelRT.DOAnchorPos3D(new Vector3(idelAnchoredPos.x+20.0f,idelAnchoredPos.y,idelAnchoredPos.z),0.5f);
        // 机制事件
        MechanismInPut.Instance.modeTest = MechanismInPut.ModeTest.Reflash;
    }
    
    public void OnDrag(PointerEventData data)
    {
        //传输鼠标当前位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // CommunicationManager.OnBlocksMove(hit.point);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask))
        {
            tetrominoe.transform.parent = hit.collider.transform.parent;
            tetrominoe.transform.localPosition = Vector3.zero;
            tetrominoe.transform.localScale = Vector3.one;
            
            if(hit.collider.transform.TryGetComponent(out BlockTetriHandler block))
            {
                tetrominoe.transform.localPosition = new Vector3( block.posId.x, 0.3f, block.posId.y);
                
                foreach(var childTetri in tetrominoe.childTetris)
                {
                    childTetri.CheckCollider();
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitBlock = false;
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.transform.TryGetComponent(out BlockDisplay block))
            {
                hitBlock = true;
            }
        }
        if (hits[0].transform)
        {
            if (!hitBlock)
            {
                TetrisBlockSimple tetrominoeTemp = Instantiate(tetrominoe.gameObject, IdelWorldSpace.transform.position,IdelWorldSpace.transform.rotation).GetComponent<TetrisBlockSimple>();
                DestroyImmediate(tetrominoe.gameObject);
                tetrominoe = tetrominoeTemp;
                if(sequence!=null)
                {
                    sequence.Kill();
                }
                // CommunicationManager.OnHeroUICancel("");
                MechanismInPut.Instance.warningSystem.changeWarningTypes = WarningSystem.WarningType.CancelOperation;
            }
            else
            {
                // CommunicationManager.BlocksMoveEnd(1);
                
                // 使用 DOTween.Sequence 创建一个序列
                    sequence = DOTween.Sequence();

                // 在序列中添加需要执行的动画
                    sequence.Append(tetrominoe.transform.DOLocalMoveY(0.2f, tetrominoe.occupyingTime/4).SetEase(Ease.Linear));
                    sequence.Append(tetrominoe.transform.DOLocalMoveY(0f, tetrominoe.occupyingTime/4).SetEase(Ease.Linear));

                // 设置循环模式为 PingPong
                    sequence.SetLoops(-1, LoopType.Yoyo);
                changeLiquid.DoCount();
                tetrominoe.Active();
                tetrominoe = null;
            }
        }
        // 退出心流
        OnTetriEndDrag?.Invoke(this);
        Canvas idelCanvas = Idel.GetComponent<Canvas>();
        idelCanvas.sortingLayerName = "Default";
        idelCanvas.sortingOrder = NotFlowOrder;
        RectTransform idelRT = Idel.GetComponent<RectTransform>();
        idelRT.DOScale(originScale, 0.5f).SetEase(Ease.OutBounce);
        idelRT.DOAnchorPos3D(idelAnchoredPos,0.5f).SetEase(Ease.OutBounce);
        // 机制事件
        Invoke(nameof(ReflashMechanism),0.2f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
  
       // Debug.LogError("按钮被点击");   
    }

    #endregion

    #region 数据操作
    void ReflashMechanism()
    {
        MechanismInPut.Instance.modeTest = MechanismInPut.ModeTest.FourDirectionsLinks;
        MechanismInPut.Instance.modeTest = MechanismInPut.ModeTest.ChainTransfer;
    }
    void Init()
    {
        transform.TryGetComponent<ChangeLiquid>(out changeLiquid);
        RectTransform rt = Idel.GetComponent<RectTransform>();
        idelAnchoredPos = rt.anchoredPosition3D;
        originScale = rt.localScale;
        OnGameObjCreate();
         // 通讯协议绑定触发方法
        // broadcastClass.CreateNewIdea += OnGameObjCreate;

        // 触发生成方块方法
        // CommunicationManager.OnCreateNewIdea(1);
        
    }
    /// <summary>
    /// 初始化生成砖块GameObject
    /// </summary>
    /// <param name="BlocksObject"></param>
    public void OnGameObjCreate()
    {
        Idel.GetComponent<RectTransform>().anchoredPosition3D = idelAnchoredPos;
        
        tetrominoe = Instantiate(tetrominoes[Random.Range(0, tetrominoes.Length)].gameObject, IdelWorldSpace.transform.position,IdelWorldSpace.transform.rotation).GetComponent<TetrisBlockSimple>();
        tetrominoe.player = player;
    }
    private void OnGameObjCreate(List<IdeaClass> IdeaClass, EventType.BlocksGrade Level)
    {
        foreach (IdeaClass info in IdeaClass)
        {
            var tetrisInfo = info.BlocksInfo.TetrisInfo;

            foreach (int key in tetrisInfo.Keys)
            {
                var infoObj = GameObject.Instantiate(tetrisInfo[key].BlocksGameObject, Idel.transform);
                Image infoObjImg = infoObj.GetComponent<Image>();
                infoObjImg.sprite = SetIdelElement(tetrisInfo[key].Color);
                infoObjImg.raycastTarget = false;
                infoObj.transform.localPosition = new Vector3(tetrisInfo[key].posx * X_space, tetrisInfo[key].posy * Y_space, 0);
                //if(!yMoved)
                {
                    
                    if( infoObj.GetComponent<RectTransform>().anchoredPosition3D.y >= Max_space_Y )
                    {
                        // Debug.LogError("y轴移动");
                        Idel.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(idelAnchoredPos.x, idelAnchoredPos.y - 10f , idelAnchoredPos.z );
                        // yMoved = true;
                    }
                    if( infoObj.GetComponent<RectTransform>().anchoredPosition3D.y <= Min_space_Y )
                    {
                        // Debug.LogError("y轴移动");
                        Idel.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(idelAnchoredPos.x, idelAnchoredPos.y + 10f , idelAnchoredPos.z );
                        // yMoved = true;
                    }
                }
                //if(!xMoved)
                {
                    
                    if( infoObj.GetComponent<RectTransform>().anchoredPosition3D.x >= Max_space_X )
                    {
                        // Debug.LogError("x轴移动");
                        Idel.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(idelAnchoredPos.x - 10f, idelAnchoredPos.y , idelAnchoredPos.z );
                        // xMoved = true;
                    }
                    if( infoObj.GetComponent<RectTransform>().anchoredPosition3D.x <= Min_space_X )
                    {
                        // Debug.LogError("x轴移动");
                        Idel.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(idelAnchoredPos.x + 10f, idelAnchoredPos.y , idelAnchoredPos.z );
                        // xMoved = true;
                    }
                }
                

            }
        }

        // BlocksInfo = IdeaClass[0].BlocksInfo;

        blockGrade = Level;
        changeLiquid.ChangeColor();

        // broadcastClass.CreateNewIdea -= OnGameObjCreate;
    }
    Sprite SetIdelElement(EventType.UnitColor color)
    {
        
        switch(color)
        {
            case UnitColor.red:
                return idelTemplate.red;
    
            case UnitColor.blue:
                return idelTemplate.blue;
         
            case UnitColor.green:
                return idelTemplate.green;
       
            case UnitColor.purple:
                return idelTemplate.purple;
     
        }
        return null;
    }
    /// <summary>
    /// 刷新想法
    /// </summary>
    public void RefreshGameObj()
    {
        // RemoveItem(0);

        // broadcastClass.CreateNewIdea += OnGameObjCreate;

        // CommunicationManager.OnCreateNewIdea(1);

        // broadcastClass.CreateNewIdea -= OnGameObjCreate;
    }

    /// <summary>
    /// 移除想法元素
    /// </summary>
    public void RemoveItem(int info)
    {
        foreach (Transform child in Idel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void RmoveItemAndRefresh(int info)
    {
        foreach (Transform child in Idel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // broadcastClass.BlockCreateDone -= RmoveItemAndRefresh;

        ImageBlock.SetActive(true);

        InvokeRepeating(nameof(UpdateTime), 1, 1);
    }

    private int timeinfo = 6;

    /// <summary>
    /// 更新时间
    /// </summary>
    private void UpdateTime()
    {

        blockText.GetComponent<TextMeshProUGUI>().text = timeinfo.ToString();

        if (timeinfo != 0)
        {
            timeinfo--;

            
            if(timeinfo==5f)
            {
                RemoveItem(0);

                // broadcastClass.CreateNewIdea += OnGameObjCreate;

                // CommunicationManager.OnCreateNewIdea(1);

                // broadcastClass.CreateNewIdea -= OnGameObjCreate;

                Idel.gameObject.SetActive(false);
            }
            
            blockText.GetComponent<TextMeshProUGUI>().text = timeinfo.ToString();
        }
        else
        {
            timeinfo = 6;

            Idel.gameObject.SetActive(true);
            ImageBlock.SetActive(false);

            CancelInvoke(nameof(UpdateTime));
        }
    }
    public void DoRotate()
    {
        if(tetrominoe)
        {
            tetrominoe.Rotate(tetrominoe.transform.forward);
        }
    }
    #endregion

}
