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

    public UnityAction OnTetriBeginDrag;
    public UnityAction OnTetriEndDrag;
    // 通信管理器
    // private CommunicationInteractionManager CommunicationManager;

    // 当前位置的砖块种类
    // private BlocksClass BlocksInfo;
    #endregion

    #region 数据关系
    void Start()
    {
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
    Vector2 pointerDownPosition; // 放置短暂下落的位置
    public void OnPointerDown(PointerEventData eventData)
    {
       Invoke(nameof(DoRotate),0.3f);
       pointerDownPosition = eventData.pointerCurrentRaycast.screenPosition;
    }
    public void OnBeginDrag(PointerEventData data)
    {
        CancelInvoke(nameof(DoRotate));
        InFlow();
    }
    public void OnDrag(PointerEventData data)
    {
        
        if(!tetrominoe)return;
        Vector2 currentPosition = data.pointerCurrentRaycast.screenPosition;
        float distance = Vector2.Distance(currentPosition, pointerDownPosition);
        float threshold = 55f;
        if (distance <= threshold)return;  

        //传输鼠标当前位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if (!hitBlock)return;
        hitBlock = hit.collider.transform.TryGetComponent(out BlockTetriHandler block);
        if (!hitBlock)return;
        tetrominoe.transform.parent = hit.collider.transform.parent;
        tetrominoe.transform.localPosition = Vector3.zero;
        tetrominoe.transform.localScale = Vector3.one;
        tetrominoe.transform.localPosition = new Vector3( block.posId.x, 0.3f, block.posId.y);
        tetrominoe.ColliderCheck();
        // 如果不在棋盘上 子物体 需要错位挪动
        tetrominoe.posId = new Vector2(tetrominoe.transform.localPosition.x,tetrominoe.transform.localPosition.z);
        bool isInCheckerboard = Dispaly.IsInCheckerboard(tetrominoe.posId);
        if(!isInCheckerboard)
        {
            tetrominoe.transform.localPosition = new Vector3( block.posId.x, 0.3f, block.posId.y);
            return;
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OutFlow();
        if(!tetrominoe)return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if(!hitBlock){FailToCreat();return;}
        if(!tetrominoe.ColliderCheck()){FailToCreat();return;}
        SuccessToCreat();
    }
    
    #endregion

    #region 数据操作
    void SuccessToCreat()
    {
        tetrominoe.SuccessToCreat();
        tetrominoe.tetrisCheckMode = TetrisBlockSimple.TetrisCheckMode.Create;
        if(!tetrominoe.Active()){ FailToCreat(); return;};
        if(!tetrominoe.ColliderCheckOnEndDrag()){ FailToCreat(); return;}
        tetrominoe.posId = new Vector2(tetrominoe.transform.localPosition.x,tetrominoe.transform.localPosition.z);
        // 使用 DOTween.Sequence 创建一个序列
        sequence = DOTween.Sequence();
        // 在序列中添加需要执行的动画
        sequence.Append(tetrominoe.transform.DOLocalMoveY(0.2f, tetrominoe.occupyingTime/4).SetEase(Ease.Linear));
        sequence.Append(tetrominoe.transform.DOLocalMoveY(0f, tetrominoe.occupyingTime/4).SetEase(Ease.Linear));
        // 设置循环模式为 PingPong
        sequence.SetLoops(-1, LoopType.Yoyo);
        // 开始计时
        changeLiquid.DoCount();
        tetrominoe = null;
    }
    void FailToCreat()
    {
        if(sequence!=null)sequence.Kill();
        TetrisBlockSimple tetrominoeTemp = Instantiate(tetrominoe.gameObject, IdelWorldSpace.transform.position,IdelWorldSpace.transform.rotation).GetComponent<TetrisBlockSimple>();
        tetrominoe.FailToCreat();
        DestroyImmediate(tetrominoe.gameObject);
        tetrominoe = tetrominoeTemp;
        MechanismInPut.Instance.warningSystem.changeWarningTypes = WarningSystem.WarningType.CancelOperation;
    }
    Tween flowScaleTween;
    Tween flowAnchorPosTween;
    void InFlow()
    {
        
        // 心流模式
        OnTetriBeginDrag?.Invoke();
        Canvas idelCanvas = Idel.GetComponent<Canvas>();
        idelCanvas.sortingLayerName = "Flow";
        idelCanvas.sortingOrder = Dispaly.FlowOrder;
        // 清理
        RectTransform idelRT = Idel.GetComponent<RectTransform>();
        idelRT.localScale = originScale;
        idelRT.anchoredPosition3D = idelAnchoredPos;
        if(flowScaleTween!=null)flowScaleTween.Kill();
        if(flowAnchorPosTween!=null)flowAnchorPosTween.Kill();
        // 播放动画
        flowScaleTween = idelRT.DOScale(2.0f, 0.5f);
        flowAnchorPosTween = idelRT.DOAnchorPos3D(new Vector3(idelAnchoredPos.x+20.0f,idelAnchoredPos.y,idelAnchoredPos.z),0.5f);
        // 机制事件
        MechanismInPut.Instance.modeTest = MechanismInPut.ModeTest.Reflash;
    }
    void OutFlow()
    {
        // 退出心流
        OnTetriEndDrag?.Invoke();
        Canvas idelCanvas = Idel.GetComponent<Canvas>();
        idelCanvas.sortingLayerName = "Default";
        idelCanvas.sortingOrder = NotFlowOrder;
        RectTransform idelRT = Idel.GetComponent<RectTransform>();
        idelRT.DOScale(originScale, 0.5f).SetEase(Ease.OutBounce);
        idelRT.DOAnchorPos3D(idelAnchoredPos,0.5f).SetEase(Ease.OutBounce);
        // 机制事件
        Invoke(nameof(ReflashMechanism),0.2f);
    }
    void ReflashMechanism()
    {
        MechanismInPut.Instance.modeTest = MechanismInPut.ModeTest.FourDirectionsLinks;
        MechanismInPut.Instance.modeTest = MechanismInPut.ModeTest.ChainTransfer;
    }
    void Init()
    {
        transform.TryGetComponent(out changeLiquid);
        RectTransform rt = Idel.GetComponent<RectTransform>();
        idelAnchoredPos = rt.anchoredPosition3D;
        originScale = rt.localScale;
        OnGameObjCreate();
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
        changeLiquid.ChangeColor(tetrominoe.color);
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
        if(!tetrominoe)return;
        DestroyImmediate(tetrominoe.gameObject);
        changeLiquid.DoCount();
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
        bool reverse = false;
        if(!tetrominoe)return;
        if(!reverse){tetrominoe.Rotate(tetrominoe.transform.forward);}
        else{tetrominoe.RotateReverse(tetrominoe.transform.forward);}
        
    }
    #endregion

}
