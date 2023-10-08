using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EventType;
using Button = UnityEngine.UI.Button;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Linq;
using UC_PlayerData;
using UnityEngine.Events;
using Mirror;

public class IdelBox : NetworkBehaviour,
  IBeginDragHandler, 
  IDragHandler, 
  IEndDragHandler,
  IPointerDownHandler
{
    #region 数据对象
    public int idelId;
    public IdelHolder idelHolder;
    public IdelUI idelUI;
    public Player player = Player.NotReady;
    public LayerMask blockTargetMask;
    public IdelTemplate idelTemplate;
    public GameObject idelContainer;
    public GameObject idelWorldSpace;
    public GameObject clickEvent;
    public GameObject ClickEvent
    {
        get
        {
            if(clickEvent)return clickEvent;
            Transform clickEventTrans = transform.Find("IdelClickEvent");
            return clickEvent;
        }
    }
    private ChangeLiquid changeLiquid;

    private Vector3 originScale;
    private Vector3 originPos;
    private Vector3 idelAnchoredPos;

    const float X_space = 6.9f;
    const float Y_space = 10f;

    const float Max_space_X = X_space * 2;
    const float Min_space_X = - (X_space * 2);
    const float Max_space_Y = Y_space * 2;
    const float Min_space_Y = - (Y_space * 2);
    const int NotFlowOrder = 22;

    MechanismInPut mechanismInPut;
    MechanismInPut.ModeTest modeTest;
    
    public TetrisBlockSimple[] tetrominoes;
    public TetrisBlockSimple tetrominoe;
    public UnityAction OnTetriBeginDrag;
    public UnityAction OnTetriEndDrag;
    Transform root;
    // 联网
    [SyncVar]
    int savedTetrisGroupID = -1;
    Vector3 mousePos_Temp;
    
    #endregion

    #region 数据关系
    void Start()
    {
        root = transform.parent.parent.parent;
        idelHolder = root.GetComponent<IdelHolder>();
        Init();
        Invoke(nameof(LateStart),0.1f);
    }
    void LateStart()
    {
        // mechanismInPut = FindObjectOfType<MechanismInPut>();
        switch(idelId)
        {
            case 1:
                idelWorldSpace = root.Find("IdelBox1_GameObject").gameObject;
                break;
            case 2:
                idelWorldSpace = root.Find("IdelBox2_GameObject").gameObject;
                break;
            case 3:
                idelWorldSpace = root.Find("IdelBox3_GameObject").gameObject;
                break;
        }
        
        
    }
    // Sequence sequence;
    Vector2 pointerDownPosition; // 放置短暂下落的位置
    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown" + eventData.pointerEnter.name);
        pointerDownPosition = eventData.pointerCurrentRaycast.screenPosition;
        if(idelHolder.Local())
        {
            Invoke(nameof(DoRotate),0.3f);
        }else
        {
            if(player!=idelHolder.playerPVP_local)return;
            Invoke(nameof(Cmd_DoRotate),0.3f);
        }
    }
    public void OnBeginDrag(PointerEventData data)
    {
        if(idelHolder.Local())
        {
            CancelInvoke(nameof(DoRotate));
            Local_OnBeginDrag();
        }else
        {
            CancelInvoke(nameof(Cmd_DoRotate));
        }
        InFlow();
    }
    public void OnDrag(PointerEventData data)
    {
        
        if(!DragChecker(data))return;
        if(idelHolder.Local())
        {
            Local_OnDrag(data);
        }else
        {
            if(!isClient)return;
            mousePos_Temp = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePos_Temp);
            RaycastHit hit;
            bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
            if (!hitBlock)return;
            hitBlock = hit.collider.transform.TryGetComponent(out BlockTetriHandler block);
            if (!hitBlock)return;
            // tetrominoe.transform.parent = hit.collider.transform.parent;
            // tetrominoe.OnTheBlocksCreator = true;
            tetrominoe.transform.localPosition = Vector3.zero;
            tetrominoe.transform.localScale = Vector3.one;
            // tetrominoe.transform.localPosition = new Vector3( block.posId.x, 0.3f, block.posId.y);
            Cmd_OnDrag(mousePos_Temp);
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        OutFlow();
        if(idelHolder.Local())
        {
            Local_OnEndDrag();
        }else
        {
            if(!tetrominoe)return;
            if(!isClient)return;
            mousePos_Temp = Input.mousePosition;
            Online_OnEndDrag(mousePos_Temp);
        }
    }
    
    #endregion

    #region 数据操作
    void Local_OnBeginDrag()
    {
        if(!tetrominoe)return;
        tetrominoe.GetComponent<TetrisUnitSimple>().OnBeginDragDisplay();
        ResetAnimation();
        PlayAnimation_OpenUp();
        OnBoardChecker();
    }
    void Online_OnEndDrag(Vector3 mousePos_Temp)
    {
        Cmd_SuccessToCreat(mousePos_Temp);
    }
    void Local_OnEndDrag()
    {
        if(!tetrominoe)return;
        tetrominoe.GetComponent<TetrisUnitSimple>().OnEndDragDisplay();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if(!hitBlock){FailToCreat();return;}
        if(!tetrominoe.ColliderCheck()){FailToCreat();return;}
        SuccessToCreat();
    }
    [Command(requiresAuthority = false)]
    void Cmd_OnDrag(Vector3 mousePos_Temp)
    {
        //传输鼠标当前位置
        Ray ray = Camera.main.ScreenPointToRay(mousePos_Temp);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if (!hitBlock)return;
        hitBlock = hit.collider.transform.TryGetComponent(out BlockTetriHandler block);
        if (!hitBlock)return;
        // tetrominoe.transform.parent = hit.collider.transform.parent;
        tetrominoe.OnTheBlocksCreator = true;
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
    void Local_OnDrag(PointerEventData data)
    {
        OnBoardChecker();
    }
    bool DragChecker(PointerEventData data)
    {
        if(!tetrominoe)return false;
        Vector2 currentPosition = data.pointerCurrentRaycast.screenPosition;
        float distance = Vector2.Distance(currentPosition, pointerDownPosition);
        float threshold = 55f;
        if (distance <= threshold)return false;
        return true;
    }
    void SuccessToCreat()
    {
        RectTransform idelRT = idelContainer.GetComponent<RectTransform>();
        tetrominoe.transform.DOScale(originScale, 0.5f).SetEase(Ease.OutBounce);
        idelRT.DOAnchorPos3D(idelAnchoredPos,0.5f).SetEase(Ease.OutBounce);
        
        tetrominoe.SuccessToCreat();
        tetrominoe.tetrisCheckMode = TetrisBlockSimple.TetrisCheckMode.Create;
        // if(!tetrominoe.Active_X()){ FailToCreat(); return;};
        if(!tetrominoe.Active()){ FailToCreat(); return;};
        if(!tetrominoe.ColliderCheckOnEndDrag()){ FailToCreat(); return;}
        tetrominoe.posId = new Vector2(tetrominoe.transform.localPosition.x,tetrominoe.transform.localPosition.z);
        tetrominoe.Display_AfterCreate();
        changeLiquid.DoCount();
        // 机制检测
        tetrominoe.BlocksCreator.BlocksCounterInvoke();
        // 置空
        tetrominoe = null;
    }
    [Command(requiresAuthority = false)]
    void Cmd_SuccessToCreat(Vector3 mousePos_Temp)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos_Temp);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if(!hitBlock){Server_FailToCreat();return;}
        if(!tetrominoe.ColliderCheck()){Server_FailToCreat();return;}
        tetrominoe.SuccessToCreat();
        tetrominoe.tetrisCheckMode = TetrisBlockSimple.TetrisCheckMode.Create;
        if(!tetrominoe.Active()){ Server_FailToCreat();return;};
        if(!tetrominoe.ColliderCheckOnEndDrag()){ Server_FailToCreat(); return;}
        tetrominoe.posId = new Vector2(tetrominoe.transform.localPosition.x,tetrominoe.transform.localPosition.z);
        tetrominoe.Display_AfterCreate();
        // 开始计时
        changeLiquid.DoCount();
        tetrominoe = null;
        Rpc_Client_SuccessToCreat();
    }
    [ClientRpc]
    void Rpc_Client_SuccessToCreat()
    {
        tetrominoe.SuccessToCreat();
        tetrominoe.tetrisCheckMode = TetrisBlockSimple.TetrisCheckMode.Create;
        tetrominoe.DisPlayOnline(true);
        changeLiquid.Client_DoCount();
        tetrominoe = null;
    }
    void FailToCreat()
    {
        PlayAnimation_CloseUp();

        tetrominoe.sequence?.Kill();
        tetrominoe.transform.parent = null;
        tetrominoe.transform.SetPositionAndRotation(idelWorldSpace.transform.position,idelWorldSpace.transform.rotation);
        tetrominoe.transform.localScale = Vector3.one;
        tetrominoe.FailToCreat();
        // Unit
        tetrominoe.transform.GetComponent<TetrisUnitSimple>().FailToCreat();
        
    }
    void Server_FailToCreat()
    {
        if(!isServer)return;
        //tetrominoe.transform.parent = null;
        tetrominoe.OnTheBlocksCreator = false;
        tetrominoe.transform.SetPositionAndRotation(idelWorldSpace.transform.position,idelWorldSpace.transform.rotation);
        tetrominoe.transform.localScale = Vector3.one;
    }
    Tween flowAnchorPosTween;
    Tween tetrominoeScaleTween;
    Tween tetrominoePosTween;
    Sequence tetrominoePosTweenSequence;
    void InFlow()
    {
        // 心流模式
        OnTetriBeginDrag?.Invoke();
        Canvas idelCanvas = idelContainer.GetComponent<Canvas>();
        idelCanvas.sortingLayerName = "Flow";
        idelCanvas.sortingOrder = Dispaly.FlowOrder;
        // 渲染层级排序
        int needSortingOrder = -2;
        SortingRenderOrders(needSortingOrder);
        // 不可以战斗
        if(!tetrominoe)return;
        tetrominoe.transform.GetComponent<TetrisUnitSimple>().CheckUnitTag(false);
        // 机制事件
        if(!idelHolder.Local())return;
        // MechanismInPut.Instance.modeTest = MechanismInPut.ModeTest.Reflash;
    }
    void OutFlow()
    {
        // 退出心流
        OnTetriEndDrag?.Invoke();
        Canvas idelCanvas = idelContainer.GetComponent<Canvas>();
        idelCanvas.sortingLayerName = "Default";
        idelCanvas.sortingOrder = NotFlowOrder;
        // 渲染层级排序
        int needSortingOrder = +2;
        SortingRenderOrders(needSortingOrder);
        // 可以战斗
        if(!tetrominoe)return;
        tetrominoe.transform.GetComponent<TetrisUnitSimple>().CheckUnitTag();
        // 机制事件
        Invoke(nameof(ReflashMechanism),0.2f);
    }
    void ReflashMechanism()
    {
        // if(!MechanismInPut.Instance)return;
        // MechanismInPut.Instance.modeTest = MechanismInPut.ModeTest.FourDirectionsLinks;
        // MechanismInPut.Instance.modeTest = MechanismInPut.ModeTest.ChainTransfer;
    }
    void Init()
    {

        transform.TryGetComponent(out changeLiquid);
        if(idelHolder.Local())
        {
            LocalDo();
            changeLiquid.OnLevelUp += OnLevelUp;
            changeLiquid.DoCount();
        }else
        {
            ServerDo();
            ServerToClient_OnGameObjCreate();
        }
        
    }
    void ServerToClient_OnGameObjCreate()
    {
        if(tetrominoe)return;
        if(savedTetrisGroupID!=-1)
        {
            ClientGetTetrisGroupID();
        }else
        {
            if(!isServer)return;
            OnGameObjCreate();
        }
    }
    public void ClientGetTetrisGroupID()
    {   
        if(!isClient)return;
        tetrominoe = FindObjectsOfType<TetrisBlockSimple>().Where(x=>x.serverID==savedTetrisGroupID).FirstOrDefault();
        if(!tetrominoe)return;
        tetrominoe.Player = player; // 刷新砖块表现
        tetrominoe.idelBox = this;
        if(player != idelHolder.playerPVP_local)
        {
            tetrominoe.DisPlayOnline(false);
            return;
        }
        changeLiquid.ChangeColor(tetrominoe.color);
    }
    /// <summary>
    /// 本地/网络 移除/加载 NetworkID
    /// </summary>
    void ServerDo()
    {
        if(!isServer)return;
        foreach (var child in tetrominoes)
        {
            child.transform.TryGetComponent(out NetworkTransformBase networkTransform);
            if(networkTransform.enabled)continue;
            networkTransform.enabled = true;
            if(child.transform.TryGetComponent(out NetworkIdentity networkIdentity))continue;
            NetworkIdentity networkIdentityTemp = child.gameObject.AddComponent<NetworkIdentity>();
            networkIdentityTemp.serverOnly = false;
            networkIdentityTemp.visible = Visibility.Default;
            
        }
    }
    void LocalDo()
    {
        foreach (var child in tetrominoes)
        {
            if(!child.transform.TryGetComponent(out NetworkTransformBase networkTransform))continue;
            networkTransform.enabled = false;
            if(!child.transform.TryGetComponent(out NetworkIdentity networkIdentity))continue;
            networkIdentity.enabled = false;
            DestroyImmediate(networkIdentity,true);
        }
    }
    /// <summary>
    /// 初始化生成砖块GameObject
    /// </summary>
    /// <param name="BlocksObject"></param>
    public void OnGameObjCreate()
    {
        idelContainer.GetComponent<RectTransform>().anchoredPosition3D = idelAnchoredPos;
        if(tetrominoe){DestroyImmediate(tetrominoe.gameObject);}
        tetrominoe = Instantiate(tetrominoes[Random.Range(0, tetrominoes.Length)].gameObject, idelWorldSpace.transform.position,idelWorldSpace.transform.rotation).GetComponent<TetrisBlockSimple>();
        tetrominoe.player = player;
        changeLiquid.ChangeColor(tetrominoe.color);
        InitAnimation();
        if(idelHolder.Local())return;
        NetworkServer.Spawn(tetrominoe.gameObject);
        Invoke(nameof(ServerLate_OnGameObjCreate),0.1f);
    }
    void ServerLate_OnGameObjCreate()
    {
        if(!isServer)return;
        savedTetrisGroupID = tetrominoe.serverID;
        tetrominoe.idelBox = this;
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
        if(idelHolder.Local())
        {
            DestroyImmediate(tetrominoe.gameObject);
            changeLiquid.DoCount();
        }else
        {
            if(!isClient)return;
            Cmd_RefreshGameObj();
        }
    }
    [Command(requiresAuthority = false)]
    public void Cmd_RefreshGameObj()
    {
        if(!tetrominoe)return;
        NetworkServer.Destroy(tetrominoe.gameObject);
        changeLiquid.DoCount();
        tetrominoe = null;
        Rpc_Client_SuccessToCreat();
    }
    public void DoRotate()
    {
        bool reverse = false;
        if(!tetrominoe)return;
        if(!reverse){tetrominoe.Rotate(tetrominoe.transform.forward);}
        else{tetrominoe.RotateReverse(tetrominoe.transform.forward);} 
    }
    [Command(requiresAuthority = false)]
    public void Cmd_DoRotate()
    {
        bool reverse = false;
        if(!tetrominoe)return;
        if(!reverse){tetrominoe.Rotate(tetrominoe.transform.forward);}
        else{tetrominoe.RotateReverse(tetrominoe.transform.forward);}
        tetrominoe.RotateTimes++;
    }
    void InitAnimation()
    {
        if(!tetrominoe)return;
        // 初始化
        RectTransform rt = idelContainer.GetComponent<RectTransform>();
        idelAnchoredPos = rt.anchoredPosition3D;
        originScale = tetrominoe.transform.localScale;
        originPos = idelWorldSpace.transform.position;
    }
    void ResetAnimation()
    {
        if(!tetrominoe)return;
        // 清理
        RectTransform idelRT = idelContainer.GetComponent<RectTransform>();
        tetrominoe.transform.localScale = originScale;
        tetrominoe.transform.position = originPos;
        idelRT.anchoredPosition3D = idelAnchoredPos;
        if(flowAnchorPosTween!=null)flowAnchorPosTween.Kill();
        if(tetrominoeScaleTween!=null)tetrominoeScaleTween.Kill();
        if(tetrominoePosTween!=null)tetrominoePosTween.Kill();
        if(tetrominoePosTweenSequence!=null)tetrominoePosTweenSequence.Kill();
    }
    void PlayAnimation_OpenUp()
    {
        RectTransform idelRT = idelContainer.GetComponent<RectTransform>();
        flowAnchorPosTween = idelRT.DOAnchorPos3D(new Vector3(idelAnchoredPos.x+20.0f,idelAnchoredPos.y,idelAnchoredPos.z),0.5f);
        tetrominoeScaleTween = tetrominoe.transform.DOScale(1.5f,0.5f).SetEase(Ease.OutBounce);
        float tetrominoePosXOffset = player == Player.Player1 ? -3.3f : 3.3f;
        tetrominoePosTween = tetrominoe.transform.DOLocalMoveX(tetrominoe.transform.localPosition.x - tetrominoePosXOffset,0.5f).SetEase(Ease.OutBounce);
    }
    void PlayAnimation_CloseUp()
    {
        RectTransform idelRT = idelContainer.GetComponent<RectTransform>();
        tetrominoeScaleTween = tetrominoe.transform.DOScale(originScale, 0.5f).SetEase(Ease.OutBounce);
        // Vector3 gatePos = player == Player.Player1 ? new Vector3(-12.5f,1f,-2.5f) : new Vector3(13f,1f,-2.5f);
        tetrominoePosTween = tetrominoe.transform.DOLocalMove(originPos, 0.5f).SetEase(Ease.OutBounce);
        flowAnchorPosTween = idelRT.DOAnchorPos3D(idelAnchoredPos,0.5f).SetEase(Ease.OutBounce);
    }
    void SortingRenderOrders(int needSortingOrder)
    {
        foreach(Canvas canvas in transform.GetComponentsInChildren<Canvas>())
        {
            if(canvas.sortingLayerName != "Flow")continue;
            // Debug.Log("canvas.sortingOrder" + canvas.transform.name);
            canvas.sortingOrder = canvas.sortingOrder + needSortingOrder;
        }
    }
    void OnBoardDefultPosChecker()
    {
        if(!tetrominoe)return;
        if(!tetrominoe.transform.parent)return;
        ResetAnimation();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if(hitBlock)return;
        Vector3 defultPos = player == Player.Player1 ? new Vector3(1f,0.3f,4f) : new Vector3(17f,0.3f,4f);
        tetrominoe.transform.localPosition = defultPos;
        tetrominoe.transform.localScale = Vector3.one;
    }
    void OnBoardChecker()
    {
        if(!tetrominoe)return;
        OnBoardDefultPosChecker();
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
        if(isInCheckerboard)return;
        tetrominoe.transform.localPosition = new Vector3( block.posId.x, 0.3f, block.posId.y);
    }
    void OnLevelUp(int level)
    {
        if(level == 0)return;
        if(!tetrominoe)return;
        tetrominoe.TetrisUnitSimple.LevelUp(level);
    }
#endregion

}
