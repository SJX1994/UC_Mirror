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
    IdelHolder IdelHolder
    {
        get
        {
            if(idelHolder)return idelHolder;
            root = transform.parent.parent.parent;
            idelHolder = root.GetComponent<IdelHolder>();
            return IdelHolder;
        }
    }
    public IdelUI idelUI;
    public Player player = Player.NotReady;
    public LayerMask blockTargetMask;
    public IdelTemplate idelTemplate;
    public GameObject idelContainer;
    public GameObject IdelContainer
    {
        get
        {
            if(!idelContainer)idelContainer = transform.Find("Idel").gameObject;
            return idelContainer;
        }
        set
        {
            idelContainer = value;
        }
    }
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
    ChangeLiquid ChangeLiquid
    {
        get
        {
            if(changeLiquid)return changeLiquid;
            changeLiquid = transform.GetComponent<ChangeLiquid>();
            return changeLiquid;
        }
    }

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
    public UnityAction OnTheCheckerboard;
    public UnityAction OnTetriEndDrag;
    Transform root;
    bool inRotationEditMode = false;
    Tween flowAnchorPosTween;
    Tween tetrominoeScaleTween;
    Tween tetrominoePosTween;
    Tween tetrominoCreatJump;
    Sequence idelContainersequence;
    Sequence tetrominoePosTweenSequence;
    // 联网
    // [SyncVar]
    // int savedTetrisGroupID = -1;
   
    
#endregion 数据对象
#region 联网数据对象
     Vector3 mousePos_Temp;
    List<TetriDifferentStatusDisplay> tetriDifferentStatusDisplays = new();
    List<TetriDifferentStatusDisplay> TetriDifferentStatusDisplays
    {
        get
        {
            if(tetriDifferentStatusDisplays.Count!=0)return tetriDifferentStatusDisplays;
            if(!tetrominoe)return null;
            tetriDifferentStatusDisplays = tetrominoe.GetComponentsInChildren<TetriDifferentStatusDisplay>().ToList();
            return tetriDifferentStatusDisplays;
        }
    }
    BlocksCreator_Main blocksCreator;
    public BlocksCreator_Main BlocksCreator
    {
        get
        {
            if(!blocksCreator)blocksCreator = FindObjectOfType<BlocksCreator_Main>();
            return blocksCreator;
        }
    }
    NetworkManagerUC_PVP networkManagerUC_PVP;
    NetworkManagerUC_PVP NetworkManagerUC_PVP
    {
        get{
            if(!isServer)return null;
            if(!networkManagerUC_PVP)networkManagerUC_PVP = FindObjectOfType<NetworkManagerUC_PVP>();
            return networkManagerUC_PVP;
        }
    }
#endregion 联网数据对象
#region 数据关系
    void Start()
    {
        idelHolder = IdelHolder;
        Init();
        Invoke(nameof(LateStart),0.1f);

    }
    public void Init(bool loadTetrominos = false)
    {
        
        transform.TryGetComponent(out changeLiquid);
        if(IdelHolder.Local())
        {
            LocalDo();
            changeLiquid.OnLevelUp += Event_OnLevelUp;
            changeLiquid.DoCount();
            idelContainersequence_Loop(true);
            if(player == Player.Player1)
            {
                UserAction.OnPlayer1UserActionStateChanged += Event_OnUserActionStateChanged;
            }else if(player == Player.Player2)
            {
                UserAction.OnPlayer2UserActionStateChanged += Event_OnUserActionStateChanged;
            }
        }else
        {
            ServerDo();
            if(!isServer)return;
            if(!loadTetrominos)return;
            changeLiquid.OnLevelUp += Server_Event_OnLevelUp;
            changeLiquid.DoCount();
            idelContainersequence_Loop(true);
            // Debug.Log("ServerDo++" + player);
            if(idelHolder.playerPVP_local == Player.Player1)
            {
                UserAction.OnPlayer1UserActionStateChanged += Event_OnUserActionStateChanged;
                UserAction.Player1UserState = UserAction.State.Loading;
            }else if(idelHolder.playerPVP_local == Player.Player2)
            {
                UserAction.OnPlayer2UserActionStateChanged += Event_OnUserActionStateChanged;
                UserAction.Player2UserState = UserAction.State.Loading;
            }
        }
        
    }
    void OnDisable()
    {
        if(!changeLiquid)return;
        changeLiquid.OnLevelUp -= Event_OnLevelUp;
        if(player == Player.Player1)
        {
            UserAction.OnPlayer1UserActionStateChanged -= Event_OnUserActionStateChanged;
        }else if(player == Player.Player2)
        {
            UserAction.OnPlayer2UserActionStateChanged -= Event_OnUserActionStateChanged;
        }
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
        if(IdelHolder.Local())
        {
            if(!inRotationEditMode)
            {
                PlayAnimation_OpenUp();
                InFlow();
            }else
            {
                Invoke(nameof(DoRotate),0.3f);
            }
            
        }else
        {
            if(!ServerLogic.isGameStart)return;
            if(!tetrominoe)return;
            if(!isClient)return;
            if(idelHolder.playerPVP_local != idelHolder.player)return;
            Invoke(nameof(Cmd_DoRotate),0.3f);

            if(inRotationEditMode)return;
            InFlow();
        }
    }
    public void OnBeginDrag(PointerEventData data)
    {
        if(IdelHolder.Local())
        {
            CancelInvoke(nameof(DoRotate));
            Local_OnBeginDrag();
        }else
        {
            CancelInvoke(nameof(Cmd_DoRotate));
            CancelInvoke(nameof(DoRotate));
            if(!tetrominoe)return;
            if(!isClient)return;
            if(idelHolder.playerPVP_local != idelHolder.player)return;
            Local_OnBeginDrag();
            Cmd_OnBeginDrag();
            
            if(inRotationEditMode)return;
            InFlow();
        }
        OnTheCheckerboard?.Invoke();
    }
    public void OnDrag(PointerEventData data)
    {
        
        if(!DragChecker(data))return;
        if(IdelHolder.Local())
        {
            Local_OnDrag(data);
        }else
        {
            if(!isClient)return;
            if(idelHolder.playerPVP_local != idelHolder.player)return;
            mousePos_Temp = Input.mousePosition;
            Cmd_OnDrag(mousePos_Temp);
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        
        if(IdelHolder.Local())
        {
            OutFlow();
            Local_OnEndDrag();
        }else
        {
            if(!tetrominoe)return;
            if(!isClient)return;
            if(idelHolder.playerPVP_local != idelHolder.player)return;
            mousePos_Temp = Input.mousePosition;
            Cmd_OnEndDrag(mousePos_Temp);
            OutFlow();
        }
    }
    
#endregion 数据关系
#region 数据操作
    void Local_OnBeginDrag()
    {
        if(!tetrominoe)return;
        tetrominoe.GetComponent<TetrisUnitSimple>().OnBeginDragDisplay();
        ResetAnimation();
        // PlayAnimation_OpenUp();
        OnBoardChecker();
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
        if(!IdelContainer)return;
        RectTransform idelRT = IdelContainer.GetComponent<RectTransform>();
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
        tetrominoe.BlocksCreator.Event_BlocksCounterInvoke();
        // 置空
        tetrominoe = null;
    }
    
    void FailToCreat()
    {
        if(!tetrominoe)return;
        PlayAnimation_CloseUp();
        tetrominoe.sequence?.Kill();
        tetrominoe.sequence = null;
        tetrominoe.transform.parent = null;
        tetrominoe.OnTheBlocksCreator = false;
        tetrominoe.transform.SetPositionAndRotation(idelWorldSpace.transform.position,idelWorldSpace.transform.rotation);
        tetrominoe.transform.localScale = Vector3.one;
        tetrominoe.FailToCreat();
        // Unit
        tetrominoe.transform.GetComponent<TetrisUnitSimple>().FailToCreat();
    }
    void InFlow()
    {
        // Debug.Log("InFlow!");
        if(player == Player.Player1)UserAction.Player1UserState = UserAction.State.CommandTheBattle_IdeaBox;
        else if(player == Player.Player2)UserAction.Player2UserState = UserAction.State.CommandTheBattle_IdeaBox;
        // 心流模式
        inRotationEditMode = true;
        OnTetriBeginDrag?.Invoke();
        
        // Canvas idelCanvas = IdelContainer.GetComponent<Canvas>();
        // idelCanvas.sortingLayerName = "Flow";
        // idelCanvas.sortingOrder = Dispaly.FlowOrder;
        // 渲染层级排序
        int needSortingOrder = -2;
        SortingRenderOrders(needSortingOrder);
        // 不可以战斗
        if(!tetrominoe)return;
        tetrominoe.transform.GetComponent<TetrisUnitSimple>().CheckUnitTag(false);
        tetrominoe.TetrisUnitSimple.SetUnitSortingOrderToFlow();
    }
    void OutFlow(bool setPlayerUserStateByBouy = false)
    {
        if(!setPlayerUserStateByBouy)
        {
            if(player == Player.Player1)UserAction.Player1UserState = UserAction.State.WatchingFight;
            else if(player == Player.Player2)UserAction.Player2UserState = UserAction.State.WatchingFight;
        }
        // 退出心流
        inRotationEditMode = false;
        OnTetriEndDrag?.Invoke();

        idelContainersequence_Loop(true);
        // Canvas idelCanvas = IdelContainer.GetComponent<Canvas>();
        // idelCanvas.sortingLayerName = "Default";
        // idelCanvas.sortingOrder = NotFlowOrder;
        // 渲染层级排序
        int needSortingOrder = +2;
        SortingRenderOrders(needSortingOrder);
        // 可以战斗
        if(!tetrominoe)return;
        tetrominoe.transform.GetComponent<TetrisUnitSimple>().CheckUnitTag();
        tetrominoe.TetrisUnitSimple.SetUnitSortingOrderToNotNormal();
    }
    void idelContainersequence_Loop(bool play)
    {
        if(!play)
        {
            idelContainersequence?.Kill();
            return;
        }
        idelContainersequence?.Kill();
        idelContainersequence = DOTween.Sequence();
        Tween go = IdelContainer.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(idelAnchoredPos.x + 6.0f,idelAnchoredPos.y,idelAnchoredPos.z),0.5f);
        Tween back = IdelContainer.GetComponent<RectTransform>().DOAnchorPos3D(idelAnchoredPos,0.5f);
        idelContainersequence.Append(back);
        idelContainersequence.Append(go);
        idelContainersequence.SetLoops(-1, LoopType.Yoyo);
    }
   
    
    void Event_OnUserActionStateChanged(UserAction.State userState)
    {
        switch(userState)
        {
            case UserAction.State.CommandTheBattle_Buoy:
                FailToCreat();
                if(!inRotationEditMode)return;
                bool setPlayerUserStateByBouy = true;
                OutFlow(setPlayerUserStateByBouy);
            break;
        }
    }
    
    /// <summary>
    /// 初始化生成砖块GameObject
    /// </summary>
    /// <param name="BlocksObject"></param>
    public void OnGameObjCreate()
    {
        if(IdelHolder.Local())
        {
            IdelContainer.GetComponent<RectTransform>().anchoredPosition3D = idelAnchoredPos;
            if(tetrominoe){DestroyImmediate(tetrominoe.gameObject);}
            tetrominoe = Instantiate(tetrominoes[Random.Range(0, tetrominoes.Length)].gameObject, idelWorldSpace.transform.position,idelWorldSpace.transform.rotation).GetComponent<TetrisBlockSimple>();
            tetrominoe.player = player;
            Color player1Color = new Color32(255,65,0,204);
            Color player2Color = new Color32(0,194,255,204);
            Color changeLiquidColor = player == Player.Player1 ? player1Color : player == Player.Player2 ? player2Color: Color.white;
            changeLiquid.ChangeColor(changeLiquidColor);
            InitAnimation();
            Invoke(nameof(LocalLate_OnGameObjCreate),0.1f);
            
            // if(IdelHolder.Local())return;
            // NetworkServer.Spawn(tetrominoe.gameObject);
            // Invoke(nameof(ServerLate_OnGameObjCreate),0.1f);
        }else
        {
            if(!isServer)return;
            string tetrominoeName = ServerLogic.tetrominoesName[Random.Range(0,(int)ServerLogic.tetrominoesName.Length)];
            Server_OnGameObjCreate(tetrominoeName);
            tetrominoe.Player = player;
            Color player1Color = new Color32(255,65,0,204);
            Color player2Color = new Color32(0,194,255,204);
            Color changeLiquidColor = player == Player.Player1 ? player1Color : player == Player.Player2 ? player2Color: Color.white;
            changeLiquid.ChangeColor(changeLiquidColor);
            InitAnimation();
            Invoke(nameof(LocalLate_OnGameObjCreate),0.1f);
        }
        
    }
    void LocalLate_OnGameObjCreate()
    {
        if(!tetrominoe)return;
        //tetrominoe.GetComponent<TetrisUnitSimple>().Display_HideUnit();
        // DoRotate();
        // tetrominoe.GetComponent<TetrisUnitSimple>().OnEndDragDisplay();
        tetrominoe.GetComponent<TetrisUnitSimple>().Display_ShowForPlayerScreen();
        tetrominoe.GetComponent<TetrisUnitSimple>().Display_ShowUnit();
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
        if(IdelHolder.Local())
        {
            DestroyImmediate(tetrominoe.gameObject);
            changeLiquid.DoCount();
        }else
        {
            if(idelHolder.playerPVP_local != idelHolder.player)return;
            Cmd_RefreshGameObj();
        }
    }
    
    public void DoRotate()
    {
        if(!tetrominoe)return;
        tetrominoe.Rotate(tetrominoe.transform.forward);
    }
    void DoReverseRotate()
    {
        if(!tetrominoe)return;
        tetrominoe.RotateReverse(tetrominoe.transform.forward);
    }
    void InitAnimation()
    {
        if(!tetrominoe)return;
        // 初始化
        RectTransform rt = IdelContainer.GetComponent<RectTransform>();
        idelAnchoredPos = rt.anchoredPosition3D;
        originScale = tetrominoe.transform.localScale;
        originPos = idelWorldSpace.transform.position;
    }
    void ResetAnimation()
    {
        if(!tetrominoe)return;
        // 清理
        RectTransform idelRT = IdelContainer.GetComponent<RectTransform>();
        tetrominoe.transform.localScale = originScale;
        tetrominoe.transform.position = originPos;
        idelRT.anchoredPosition3D = idelAnchoredPos;
        if(flowAnchorPosTween!=null){flowAnchorPosTween.Kill(); flowAnchorPosTween = null;}
        if(tetrominoeScaleTween!=null){tetrominoeScaleTween.Kill(); tetrominoeScaleTween = null;}
        if(tetrominoePosTween!=null){tetrominoePosTween.Kill(); tetrominoePosTween = null;}
        if(tetrominoePosTweenSequence!=null){ tetrominoePosTweenSequence.Kill(); tetrominoePosTweenSequence = null;}
    }
    void PlayAnimation_OpenUp()
    {
        idelContainersequence_Loop(false);
        RectTransform idelRT = IdelContainer.GetComponent<RectTransform>();
        flowAnchorPosTween = idelRT.DOAnchorPos3D(new Vector3(idelAnchoredPos.x+20.0f,idelAnchoredPos.y,idelAnchoredPos.z),0.5f);
        tetrominoeScaleTween = tetrominoe.transform.DOScale(1.5f,0.5f).SetEase(Ease.OutBounce);
        float tetrominoePosXOffset = player == Player.Player1 ? -2.0f : 2.0f;
        tetrominoePosTween = tetrominoe.transform.DOLocalMoveX(tetrominoe.transform.localPosition.x - tetrominoePosXOffset,0.5f).SetEase(Ease.OutBounce);
        tetrominoe.GetComponent<TetrisUnitSimple>().OnEditingStatusAfterSelection();
    }
    void PlayAnimation_CloseUp()
    {
        float animationDelay = 0.5f;
        idelContainersequence_Loop(false);
        RectTransform idelRT = IdelContainer.GetComponent<RectTransform>();
        tetrominoeScaleTween = tetrominoe.transform.DOScale(originScale, animationDelay).SetEase(Ease.OutBounce);
        // Vector3 gatePos = player == Player.Player1 ? new Vector3(-12.5f,1f,-2.5f) : new Vector3(13f,1f,-2.5f);
        tetrominoePosTween = tetrominoe.transform.DOLocalMove(originPos, animationDelay).SetEase(Ease.OutBounce);
        flowAnchorPosTween = idelRT.DOAnchorPos3D(idelAnchoredPos,animationDelay).SetEase(Ease.OutBounce);
        // Invoke(nameof(DoRotate),animationDelay);
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
    void Event_OnLevelUp(int level)
    {
        if(level == 0)return;
        if(!tetrominoe)return;
        tetrominoe.TetrisUnitSimple.LevelUp(level);
    }
#endregion 数据操作
#region 联网数据操作
    public bool isMyScreen()
    {
        return (idelHolder.playerPVP_local == idelHolder.player);
    }
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
    [Server]
    void Server_Event_OnLevelUp(int level)
    {
        if(level == 0)return;
        if(!tetrominoe)return;
        tetrominoe.TetrisUnitSimple.LevelUp(level);
        Client_OnLevelUp(tetrominoe.netId,level);
    }
    [ClientRpc]
    void Client_OnLevelUp(uint tetrominoeNetId,int level)
    {
        if(!tetrominoe)tetrominoe = FindObjectsOfType<TetrisBlockSimple>().ToList().Find(tetrominoe => tetrominoe.netId == tetrominoeNetId);
        if(!tetrominoe)return;
        tetrominoe.TetrisUnitSimple.LevelUp(level);
    }
    [Command(requiresAuthority = false)]
    void Cmd_OnBeginDrag()
    {
        if(!tetrominoe)return;
        tetrominoe.GetComponent<TetrisUnitSimple>().OnBeginDragDisplay();
        ResetAnimation();
        // PlayAnimation_OpenUp();
        OnBoardChecker();
        if(inRotationEditMode)return;
        InFlow();
        // Client_InFlow(tetrominoe.netId,player);
    }
    [Server]
    void Server_OnGameObjCreate(string tetrominoeName)
    {
        tetrominoe = Instantiate(NetworkManagerUC_PVP.spawnPrefabs.Find(prefab => prefab.name == tetrominoeName),idelWorldSpace.transform.position,idelWorldSpace.transform.rotation).GetComponent<TetrisBlockSimple>();
        NetworkServer.Spawn(tetrominoe.gameObject);
        Client_OnGameObjCreate(tetrominoe.netId,tetrominoe.Player);
    }
    [ClientRpc]
    void Client_OnGameObjCreate(uint tetrominoeNetId,Player player)
    {
        if(!tetrominoe)tetrominoe = FindObjectsOfType<TetrisBlockSimple>().ToList().Find(tetrominoe => tetrominoe.netId == tetrominoeNetId);
        if(!tetrominoe)return;
        tetrominoe.Player = player;
        if(!isMyScreen())return;
        Invoke(nameof(LocalLate_OnGameObjCreate),0.1f);
    }
    [Command(requiresAuthority = false)]
    void Cmd_OnEndDrag(Vector3 mousePos_Temp)
    {
        OutFlow();
        if(!tetrominoe)return;
        tetrominoe.GetComponent<TetrisUnitSimple>().OnEndDragDisplay();
        // Client_OutFlow(tetrominoe.netId,player);
        Ray ray = Camera.main.ScreenPointToRay(mousePos_Temp);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if(!hitBlock){FailToCreat();Client_FailToCreat();return;}
        if(!tetrominoe.ColliderCheck()){FailToCreat();Client_FailToCreat();return;}
        SuccessToCreat();
        Client_SuccessToCreat();
    }
    
    [ClientRpc]
    void Client_FailToCreat()
    {
        if(!tetrominoe)return;
        PlayAnimation_CloseUp();
        tetrominoe.sequence?.Kill();
        tetrominoe.sequence = null;
        tetrominoe.transform.parent = null;
        tetrominoe.transform.localScale = Vector3.one;
        tetrominoe.FailToCreat();
        // Unit
        tetrominoe.transform.GetComponent<TetrisUnitSimple>().FailToCreat();
    }
    [ClientRpc]
    void Client_SuccessToCreat()
    {
        if(!IdelContainer)return;
        RectTransform idelRT = IdelContainer.GetComponent<RectTransform>();
        tetrominoe.transform.DOScale(originScale, 0.5f).SetEase(Ease.OutBounce);
        idelRT.DOAnchorPos3D(idelAnchoredPos,0.5f).SetEase(Ease.OutBounce);
        tetrominoe.posId = new Vector2(tetrominoe.transform.localPosition.x,tetrominoe.transform.localPosition.z);
        tetrominoe.Display_AfterCreate();
        LocalLate_OnGameObjCreate();
        // 置空
        tetrominoe = null;
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
        if(!tetrominoe.transform.parent)
        {
            tetrominoe.transform.parent = hit.collider.transform.parent;
            Client_SetParent();
        }
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
    [ClientRpc]
    void Client_SetParent()
    {
        if(!tetrominoe)return;
        tetrominoe.transform.parent = BlocksCreator.transform;
    }
    [ClientRpc]
    void Rpc_Client_SuccessToCreat()
    {
        tetrominoe.SuccessToCreat();
        tetrominoe.tetrisCheckMode = TetrisBlockSimple.TetrisCheckMode.Create;
        tetrominoe.DisPlayOnline(true);
        // changeLiquid.Client_DoCount();
        tetrominoe = null;
    }
    [Command(requiresAuthority = false)]
    public void Cmd_RefreshGameObj()
    {
        if(!tetrominoe)return;
        NetworkServer.Destroy(tetrominoe.gameObject);
        changeLiquid.DoCount();
        tetrominoe = null;
    }
    [Command(requiresAuthority = false)]
    public void Cmd_DoRotate()
    {
        if(!inRotationEditMode)
        {
            PlayAnimation_OpenUp();
            InFlow();
            // Client_InFlow(tetrominoe.netId,player);
        }else
        {
            
            Server_DoRotate();
            Client_DoRotate();
            // Invoke(nameof(Server_DoRotate),0.3f);
            // Invoke(nameof(Client_DoRotate),0.3f);
        }
    }
    [ClientRpc]
    void Client_InFlow(uint tetrominoeID,Player player)
    {
        if(player == Player.Player1)
        {
            UserAction.Player1UserState = UserAction.State.CommandTheBattle_IdeaBox;
        }
        else if(player == Player.Player2)
        {
            UserAction.Player2UserState = UserAction.State.CommandTheBattle_IdeaBox;
        }
        // if(!tetrominoe)tetrominoe = FindObjectsOfType<TetrisBlockSimple>().Where(x=>x.netId==tetrominoeID).FirstOrDefault();
        // if(!tetrominoe)return;
        // TetriDifferentStatusDisplays.ForEach(x=>x.Event_OnUserActionStateChanged(UserAction.State.CommandTheBattle_IdeaBox));
        // // 不知道为啥 Event_OnUserActionStateChanged 没有接收到
        // tetrominoe.TetrisUnitSimple.SetUnitSortingOrderToFlow();
        // // 心流模式
        // inRotationEditMode = true;
        // // 渲染层级排序
        // int needSortingOrder = -2;
        // SortingRenderOrders(needSortingOrder);
        // BlocksCreator.Event_OnListenBlocksMoveStart();
    }
    [ClientRpc]
    void Client_OutFlow(uint tetrominoeID,Player player)
    {
        if(player == Player.Player1)
        {
            UserAction.Player1UserState = UserAction.State.WatchingFight;
        }
        else if(player == Player.Player2)
        {
            UserAction.Player2UserState = UserAction.State.WatchingFight;
        }
        // if(!tetrominoe)tetrominoe = FindObjectsOfType<TetrisBlockSimple>().Where(x=>x.netId==tetrominoeID).FirstOrDefault();
        // if(!tetrominoe)return;
        // tetrominoe.GetComponent<TetrisUnitSimple>().OnEndDragDisplay();
        // TetriDifferentStatusDisplays.ForEach(x=>x.Event_OnUserActionStateChanged(UserAction.State.WatchingFight));
        // inRotationEditMode = false;
        // idelContainersequence_Loop(true);
        // // 渲染层级排序
        // int needSortingOrder = +2;
        // SortingRenderOrders(needSortingOrder);
        // BlocksCreator.Event_OnListenBlocksMoveEnd();
    }
    [ClientRpc]
    void Client_DoRotate()
    {
        if(!tetrominoe)return;
        tetrominoe.Client_Rotate(tetrominoe.transform.forward);
    }
    [Server]
    public void Server_DoRotate()
    {
        
        if(!tetrominoe)return;
        tetrominoe.Server_Rotate(tetrominoe.transform.forward);
    }
#endregion 联网数据操作

}
