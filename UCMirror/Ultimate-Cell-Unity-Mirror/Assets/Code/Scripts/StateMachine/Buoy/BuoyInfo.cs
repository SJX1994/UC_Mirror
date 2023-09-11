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
    [HideInInspector]
    public int palyerId;
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
            MouseButtonDown();
        }
    }
    void MouseButtonDown()
    {
        if (!Input.GetMouseButtonDown(0))return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if(!hitBlock)return;
        BlockBuoyHandler block;
        hit.collider.transform.TryGetComponent(out block);
        if(!block)return;
        transform.parent = hit.collider.transform.parent;
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localPosition = new Vector3( block.posId.x, 0f, block.posId.y);
        transform.localRotation = Quaternion.Euler(Vector3.zero);
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
            palyerId = 0;
            if(Local())return;
            if(!isLocalPlayer)return;
            CmdChangePlayerSkin(true);
        }else
        {
            palyerId = 1;
            if(Local())return;
            if(!isLocalPlayer)return;
            CmdChangePlayerSkin(false);
        }
    }
    [Command]
    void CmdChangePlayerSkin(bool isPlayer1)
    {
        buoyTurnHandle = transform.Find("TurnHandles_P2").GetComponent<BuoyTurnHandle>();
        buoyBehavior = transform.Find("Behavior_P2").GetComponent<BuoyBehavior>();
        buoyTurnHandle.gameObject.SetActive(!isPlayer1);
        buoyBehavior.gameObject.SetActive(!isPlayer1);
        buoyTurnHandle = transform.Find("TurnHandles").GetComponent<BuoyTurnHandle>();
        buoyBehavior = transform.Find("Behavior").GetComponent<BuoyBehavior>();
        buoyTurnHandle.gameObject.SetActive(isPlayer1);
        buoyBehavior.gameObject.SetActive(isPlayer1);
        ChangePlayerSkin(isPlayer1);
    }
    [ClientRpc]
    void ChangePlayerSkin(bool isPlayer1)
    {
        buoyTurnHandle = transform.Find("TurnHandles_P2").GetComponent<BuoyTurnHandle>();
        buoyBehavior = transform.Find("Behavior_P2").GetComponent<BuoyBehavior>();
        buoyTurnHandle.gameObject.SetActive(!isPlayer1);
        buoyBehavior.gameObject.SetActive(!isPlayer1);
        buoyTurnHandle = transform.Find("TurnHandles").GetComponent<BuoyTurnHandle>();
        buoyBehavior = transform.Find("Behavior").GetComponent<BuoyBehavior>();
        buoyTurnHandle.gameObject.SetActive(isPlayer1);
        buoyBehavior.gameObject.SetActive(isPlayer1);
    }
    
}
