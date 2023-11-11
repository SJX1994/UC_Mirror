using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Mirror;
public class BuoyInfo : NetworkBehaviour
{
    // BlocksCreator blocksCreator;
    [SyncVar]
    public Player player;
    public Player player_local;
    public LayerMask blockTargetMask;
    public BuoyTurnHandle buoyTurnHandle;
    public BuoyBehavior buoyBehavior;

    public BlockBuoyHandler blockBuoyHandler;
    private Vector2 currentPosID;
    public Vector2 CurrentPosID
    {
        get
        {
            return currentPosID;
        }
        set
        {
            if (value == currentPosID)return;
            currentPosID = value;
            OnBuoyPosIDChange?.Invoke(currentPosID);
        }
    }
    public UnityAction<Vector2> OnBuoyPosIDChange;
    public UnityAction OnBuoyDrag;
    public UnityAction OnBuoyEndDrag;
    // 在线模式：
    Vector3 mousePos_Temp;
    public BlocksCreator_Main blocksCreator;
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
    // Start is called before the first frame update
    void Start()
    {
        if(Local())
        {   
            buoyTurnHandle = transform.Find("TurnHandles").GetComponent<BuoyTurnHandle>();
            buoyBehavior = transform.Find("Behavior").GetComponent<BuoyBehavior>();
            Active();
        }else
        {
            if(!isLocalPlayer)return;
            Active();
        }
        
    }
    
    // Update is called once per frame
    void LateUpdate()
    {
        if(Local())
        {
            MouseButtonDown();
        }else
        {
            if(!isLocalPlayer)return;
            Client_MouseButtonDown();
            
        }
    }
    public bool MouseButtonDown(bool updateCheck = true)
    {
        if (!Input.GetMouseButtonDown(0) && updateCheck)return false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if(!hitBlock)return false;
        BlockBuoyHandler block;
        hit.collider.transform.TryGetComponent(out block);
        if(!block)return false;
        transform.parent = hit.collider.transform.parent;
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localPosition = new Vector3( block.posId.x, 0f, block.posId.y);
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        blockBuoyHandler = block; // 尝试移动
        if(!blockBuoyHandler)return false;
        CurrentPosID = block.posId;
        if(!buoyTurnHandle.actived){buoyTurnHandle.Active();}
        buoyTurnHandle.CenterOfControl = block.posId;
        buoyTurnHandle.CountScanning(false);
        return true;
    }
    [Server]
    public bool Server_MouseButtonDown(Vector3 mousePos_Temp)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos_Temp);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if(!hitBlock)return false;
        BlockBuoyHandler block;
        hit.collider.transform.TryGetComponent(out block);
        if(!block)return false;
        transform.parent = hit.collider.transform.parent;
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localPosition = new Vector3( block.posId.x, 0f, block.posId.y);
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        blockBuoyHandler = block; // 尝试移动
        if(!blockBuoyHandler)return false;
        CurrentPosID = block.posId;
        if(!buoyTurnHandle.actived){buoyTurnHandle.Active();}
        buoyTurnHandle.CenterOfControl = block.posId;
        buoyTurnHandle.CountScanning(false);
        return true;
    }
    [Client]
    void Client_MouseButtonDown()
    {
        if (!Input.GetMouseButtonDown(0))return;
        mousePos_Temp = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos_Temp);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if(!hitBlock)return;
        BlockBuoyHandler block;
        hit.collider.transform.TryGetComponent(out block);
        if(!block)return;
        OnTheBlocksCreator = true;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        Cmd_MouseButtonDown(mousePos_Temp);
    }
    Vector3 ClientMouseOffest(Vector3 mousePos)
    {
        // Debug.Log("mousePos++"+mousePos);
        Vector3 player1MousePosition = mousePos - new Vector3(87.9f,102.2f,0);
        Vector3 player2MousePosition = mousePos + new Vector3(188.5f,50f,0);
        Vector3 mousePosition = player_local == Player.Player1 ? player1MousePosition : player_local == Player.Player2 ? player2MousePosition : Vector3.zero;
        return mousePosition;
    }
    [Command(requiresAuthority = false)]
    void Cmd_MouseButtonDown(Vector3 mousePos_Temp)
    {
        Ray ray = Camera.main.ScreenPointToRay(ClientMouseOffest(mousePos_Temp));
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if(!hitBlock)return;
        BlockBuoyHandler block;
        hit.collider.transform.TryGetComponent(out block);
        if(!block)return;
        OnTheBlocksCreator = true;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localPosition = new Vector3( block.posId.x, 0f, block.posId.y);
        blockBuoyHandler = block; // 尝试移动
        CurrentPosID = block.posId;
        if(!buoyTurnHandle.actived){buoyTurnHandle.Active();}
        buoyTurnHandle.CenterOfControl = block.posId;
        buoyTurnHandle.CountScanning(false);
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
    public void Active()
    {
        if(player == Player.Player1)
        {
            player_local = player;
            if(Local())return;
            if(!isLocalPlayer)return;
            bool isPlayer1 = true;
            CmdChangePlayerSkin(isPlayer1);
        }else
        {
            player_local = player;
            if(Local())return;
            if(!isLocalPlayer)return;
            bool isPlayer1 = false;
            CmdChangePlayerSkin(isPlayer1);
        }
    }
    /// <summary>
    /// 改变玩家皮肤
    /// </summary>
    /// <param name="isPlayer1"></param>
    [Command(requiresAuthority = false)]
    void CmdChangePlayerSkin(bool isPlayer1)
    {
        blocksCreator = FindObjectOfType<BlocksCreator_Main>();
        OnTheBlocksCreator = false;
        buoyTurnHandle = transform.Find("TurnHandles_P2").GetComponent<BuoyTurnHandle>();
        buoyBehavior = transform.Find("Behavior_P2").GetComponent<BuoyBehavior>();
        buoyTurnHandle.gameObject.SetActive(!isPlayer1);
        buoyBehavior.gameObject.SetActive(!isPlayer1);
        buoyTurnHandle = transform.Find("TurnHandles").GetComponent<BuoyTurnHandle>();
        buoyBehavior = transform.Find("Behavior").GetComponent<BuoyBehavior>();
        buoyTurnHandle.gameObject.SetActive(isPlayer1);
        buoyBehavior.gameObject.SetActive(isPlayer1);
        if(isPlayer1)
        {
            buoyTurnHandle = transform.Find("TurnHandles").GetComponent<BuoyTurnHandle>();
            buoyBehavior = transform.Find("Behavior").GetComponent<BuoyBehavior>();
        }else
        {
            buoyTurnHandle = transform.Find("TurnHandles_P2").GetComponent<BuoyTurnHandle>();
            buoyBehavior = transform.Find("Behavior_P2").GetComponent<BuoyBehavior>();
        }
        ChangePlayerSkin(isPlayer1);
    }
    [ClientRpc]
    void ChangePlayerSkin(bool isPlayer1)
    {
        blocksCreator = FindObjectOfType<BlocksCreator_Main>();
        OnTheBlocksCreator = false;
        buoyTurnHandle = transform.Find("TurnHandles_P2").GetComponent<BuoyTurnHandle>();
        buoyBehavior = transform.Find("Behavior_P2").GetComponent<BuoyBehavior>();
        buoyTurnHandle.gameObject.SetActive(!isPlayer1);
        buoyBehavior.gameObject.SetActive(!isPlayer1);
        buoyTurnHandle = transform.Find("TurnHandles").GetComponent<BuoyTurnHandle>();
        buoyBehavior = transform.Find("Behavior").GetComponent<BuoyBehavior>();
        buoyTurnHandle.gameObject.SetActive(isPlayer1);
        buoyBehavior.gameObject.SetActive(isPlayer1);
        if(isPlayer1)
        {
            buoyTurnHandle = transform.Find("TurnHandles").GetComponent<BuoyTurnHandle>();
            buoyBehavior = transform.Find("Behavior").GetComponent<BuoyBehavior>();
        }else
        {
            buoyTurnHandle = transform.Find("TurnHandles_P2").GetComponent<BuoyTurnHandle>();
            buoyBehavior = transform.Find("Behavior_P2").GetComponent<BuoyBehavior>();
        }
    }
    public void GoOnTheBlocksCreator(bool oldValue,bool newValue)
    {
        if(Local())return;
        if(!blocksCreator){blocksCreator = FindObjectOfType<BlocksCreator_Main>();}
        if(newValue)
        {
            transform.parent = blocksCreator.transform;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
        }else
        {
            transform.parent = null;
        }
    }
    
}
