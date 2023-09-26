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
                IdelWorldSpace = root.Find("IdelBox1_GameObject").gameObject;
                break;
            case 2:
                IdelWorldSpace = root.Find("IdelBox2_GameObject").gameObject;
                break;
            case 3:
                IdelWorldSpace = root.Find("IdelBox3_GameObject").gameObject;
                break;
        }
        
        
    }
    // Sequence sequence;
    Vector2 pointerDownPosition; // 放置短暂下落的位置
    public void OnPointerDown(PointerEventData eventData)
    {
        
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
    void Online_OnEndDrag(Vector3 mousePos_Temp)
    {
        Cmd_SuccessToCreat(mousePos_Temp);
    }
    void Local_OnEndDrag()
    {
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
        tetrominoe.SuccessToCreat();
        tetrominoe.tetrisCheckMode = TetrisBlockSimple.TetrisCheckMode.Create;
        // if(!tetrominoe.Active_X()){ FailToCreat(); return;};
        if(!tetrominoe.Active()){ FailToCreat(); return;};
        if(!tetrominoe.ColliderCheckOnEndDrag()){ FailToCreat(); return;}
        tetrominoe.posId = new Vector2(tetrominoe.transform.localPosition.x,tetrominoe.transform.localPosition.z);
        tetrominoe.Display_AfterCreate();
        changeLiquid.DoCount();
        // 道具检测
        //tetrominoe.TetrisUnitSimple.UnitActionInit();
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
        tetrominoe.sequence?.Kill();
        tetrominoe.transform.parent = null;
        tetrominoe.transform.SetPositionAndRotation(IdelWorldSpace.transform.position,IdelWorldSpace.transform.rotation);
        tetrominoe.transform.localScale = Vector3.one;
        tetrominoe.FailToCreat();
        // Unit
        tetrominoe.transform.GetComponent<TetrisUnitSimple>().FailToCreat();
        // DestroyImmediate(tetrominoe.gameObject);
        // tetrominoe = tetrominoeTemp;
        if(!MechanismInPut.Instance.warningSystem)return;
        MechanismInPut.Instance.warningSystem.changeWarningTypes = WarningSystem.WarningType.CancelOperation;
    }
    void Server_FailToCreat()
    {
        if(!isServer)return;
        //tetrominoe.transform.parent = null;
        tetrominoe.OnTheBlocksCreator = false;
        tetrominoe.transform.SetPositionAndRotation(IdelWorldSpace.transform.position,IdelWorldSpace.transform.rotation);
        tetrominoe.transform.localScale = Vector3.one;
        
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
        // 不可以战斗
        if(!tetrominoe)return;
        tetrominoe.transform.GetComponent<TetrisUnitSimple>().CheckUnitTag(false);
        // 机制事件
        if(!idelHolder.Local())return;
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
        // 可以战斗
        if(!tetrominoe)return;
        tetrominoe.transform.GetComponent<TetrisUnitSimple>().CheckUnitTag();
        // 机制事件
        Invoke(nameof(ReflashMechanism),0.2f);
    }
    void ReflashMechanism()
    {
        if(!MechanismInPut.Instance)return;
        MechanismInPut.Instance.modeTest = MechanismInPut.ModeTest.FourDirectionsLinks;
        MechanismInPut.Instance.modeTest = MechanismInPut.ModeTest.ChainTransfer;
    }
    void Init()
    {

        transform.TryGetComponent(out changeLiquid);
        RectTransform rt = Idel.GetComponent<RectTransform>();
        idelAnchoredPos = rt.anchoredPosition3D;
        originScale = rt.localScale;
        if(idelHolder.Local())
        {
            LocalDo();
            OnGameObjCreate();
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
        Idel.GetComponent<RectTransform>().anchoredPosition3D = idelAnchoredPos;
        if(tetrominoe){DestroyImmediate(tetrominoe.gameObject);}
        tetrominoe = Instantiate(tetrominoes[Random.Range(0, tetrominoes.Length)].gameObject, IdelWorldSpace.transform.position,IdelWorldSpace.transform.rotation).GetComponent<TetrisBlockSimple>();
        tetrominoe.player = player;
        changeLiquid.ChangeColor(tetrominoe.color);
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

    /// <summary>
    /// 移除想法元素
    /// </summary>
    public void RemoveItem()
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
            if(timeinfo!=5f)return;
            RemoveItem();
            Idel.gameObject.SetActive(false);
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
    [Command(requiresAuthority = false)]
    public void Cmd_DoRotate()
    {
        bool reverse = false;
        if(!tetrominoe)return;
        if(!reverse){tetrominoe.Rotate(tetrominoe.transform.forward);}
        else{tetrominoe.RotateReverse(tetrominoe.transform.forward);}
        tetrominoe.RotateTimes++;
    }
    #endregion

}
