using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerData;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class BuoyInfo : MonoBehaviour
{
    BlocksCreator blocksCreator;
    public Player player;
    [HideInInspector]
    public int palyerId;
    public LayerMask blockTargetMask;
    public BuoyTurnHandle buoyTurnHandle;
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
        buoyTurnHandle = transform.Find("TurnHandles").GetComponent<BuoyTurnHandle>();
        if(player == Player.Player1)
        {
            palyerId = 0;
        }else
        {
            palyerId = 1;
        }

    }
    
    // Update is called once per frame
    void LateUpdate()
    {
        // 设置位置
        if (Input.GetMouseButtonDown(palyerId))
        {
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
    }
    
    
}
