using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class TetriBlockSimple : MonoBehaviour
{
    public TetrisBlockSimple tetrisBlockSimple;
    public UC_PlayerData.Player player = UC_PlayerData.Player.NotReady;
    public LayerMask blockTargetMask;
    public Vector2 posId;
    public BlockTetriHandler currentBlockTetriHandler;
    public UnityAction<TetriBlockSimple> CantPutCallback;
    private bool canMove = false;
    public bool CanMove
    {
        get 
        { 
            return canMove; 
        }
        set
        {
            if(value == canMove)return;
            canMove = value;
            DoGroupMoveCheck();
        }
    }
    bool canCreate = false;
    bool canDrop = false;
    BlockTetriHandler blockOccupying;
    SortingGroup cubeSortingGroup;
    Material sharedMaterial;
    TetriDisplayRange tetriDisplayRange;
    // Start is called before the first frame update
    void Start()
    {
        tetrisBlockSimple = transform.parent.GetComponent<TetrisBlockSimple>();
        tetrisBlockSimple.OnTetrisMoveing += ()=>{CanMove = false;};
        cubeSortingGroup = transform.Find("Cube").GetComponent<SortingGroup>();
        sharedMaterial = cubeSortingGroup.transform.GetComponent<MeshRenderer>().sharedMaterial;
        Display_playerColor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void FailToCreat()
    {
        canCreate = false;
        CancelInvoke(nameof(DoOccupied));
    }
    public void SuccessToCreat()
    {
        canCreate = true;
    }
    public void Reset()
    {
        currentBlockTetriHandler = null;
        blockOccupying = null;
        CanMove = false;
        canDrop = false;
        canCreate = false;
    }
    public bool CheckColliderOnEndDrag()
    {
        if(!canCreate)return false;
        // 发射射线向下进行检测
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if(!hitBlock)return false;
        // 进一步的处理
        BlockTetriHandler block;
        hit.collider.transform.TryGetComponent(out block);
        if(!block)return false;
        return true;
            
    }
    public bool CheckCollider()
    {
        // 发射射线向下进行检测
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if(hitBlock)
        {
            // 进一步的处理
            BlockTetriHandler block;
            hit.collider.transform.TryGetComponent(out block);
            if(!block)return false;
            if(block.State == BlockTetriHandler.BlockTetriState.Occupying)
            {
                // 不能放置
                CantPutCallback?.Invoke(this);
                return false;
            }
            if(player == UC_PlayerData.Player.Player1 && block.State == BlockTetriHandler.BlockTetriState.Occupied_Player2)
            {
                // 不能放置
                CantPutCallback?.Invoke(this);
                return false;
            }
            else if(player == UC_PlayerData.Player.Player2 && block.State == BlockTetriHandler.BlockTetriState.Occupied_Player1)
            {
                // 不能放置
                CantPutCallback?.Invoke(this);
                return false;
            }
            if(!block.tetriBlockSimpleHolder)
            {
                // 可以放置
                return true;
            }
            else
            {
                CantPutCallback?.Invoke(this);
                return false;
            }
            
        }else
        {
            CantPutCallback?.Invoke(this);
            return false;
        }
    }
    public void Active()
    {
        if(!canCreate)return;
        // 发射射线向下进行检测
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if (!hitBlock)return;
        BlockTetriHandler block;
        hit.collider.transform.TryGetComponent(out block);
        if(!block)return;
        blockOccupying = block;
        posId = block.posId;
    }
    public void DropCheck()
    {
        // 占领
        // 发射射线向下进行检测
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if (!hitBlock)return;
        // 进一步的处理
        BlockTetriHandler block;
        hit.collider.transform.TryGetComponent(out block);
        if(!block)return;
        blockOccupying = block;
        posId = block.posId;
        if(player == UC_PlayerData.Player.Player1 && block.State!= BlockTetriHandler.BlockTetriState.Occupied_Player1)
        {
            block.State = BlockTetriHandler.BlockTetriState.Occupying;   
        }
        else if(player == UC_PlayerData.Player.Player2 && block.State!= BlockTetriHandler.BlockTetriState.Occupied_Player2)
        {
            block.State = BlockTetriHandler.BlockTetriState.Occupying;   
        }
        
    }
    public BlockTetriHandler NextBlock()
    {
        BlocksCreator blocksCreator = tetrisBlockSimple.blocksCreator;
        var blockP1 = blocksCreator.blocks.Find((block) => block.posId == new Vector2(posId.x + 1,posId.y));
        var blockP2 = blocksCreator.blocks.Find((block) => block.posId == new Vector2(posId.x - 1,posId.y));
        if(player == UC_PlayerData.Player.Player1)
        {
            if(!blockP1)return null;
            return blockP1.GetComponent<BlockTetriHandler>();
        }
        else if(player == UC_PlayerData.Player.Player2)
        {
            if(!blockP2)return null;
            return blockP2.GetComponent<BlockTetriHandler>();
        }
        return null;
    }
    public BlockTetriHandler CurrentBlock()
    {
        BlocksCreator blocksCreator = tetrisBlockSimple.blocksCreator;
        BlockTetriHandler currentBlock = blocksCreator.blocks.Find((block) => block.posId == new Vector2(posId.x,posId.y)).GetComponent<BlockTetriHandler>();
        return currentBlock;
        
    }
    public bool BlockNextCheck(BlockTetriHandler block)
    {
        if(block.State == BlockTetriHandler.BlockTetriState.Occupied_Player2 && player == UC_PlayerData.Player.Player1 && block.tetriBlockSimpleHolder != null)
        {
            return false;
        }
        if(block.State == BlockTetriHandler.BlockTetriState.Occupied_Player1 && player== UC_PlayerData.Player.Player2 && block.tetriBlockSimpleHolder != null)
        {
            return false;
        }
        if(block.State == BlockTetriHandler.BlockTetriState.Occupying)
        {
            return false;
        }
        if(block.tetriBlockSimpleHolder != null)
        {
            return false;
        }
        return true;
            
    }
    public void DoGroupMoveCheck()
    {
        if(!canCreate)return;
        // 占领
        // 发射射线向下进行检测
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if (!hitBlock)return;
        BlockTetriHandler block;
        hit.collider.transform.TryGetComponent(out block);
        // 进一步的处理
        if(!block)return;
        blockOccupying = block;
        posId = block.posId;
        if(player == UC_PlayerData.Player.Player1 && block.State!= BlockTetriHandler.BlockTetriState.Occupied_Player1)
        {
            block.State = BlockTetriHandler.BlockTetriState.Occupying;
            Invoke(nameof(DoOccupied), tetrisBlockSimple.occupyingTime-0.1f);
        }
        else if(player == UC_PlayerData.Player.Player1 && block.State == BlockTetriHandler.BlockTetriState.Occupied_Player1)
        {
            Invoke(nameof(DoOccupied), tetrisBlockSimple.occupyingTime-0.5f);
        }
        else if(player == UC_PlayerData.Player.Player2 && block.State!= BlockTetriHandler.BlockTetriState.Occupied_Player2)
        {
            block.State = BlockTetriHandler.BlockTetriState.Occupying;
            Invoke(nameof(DoOccupied), tetrisBlockSimple.occupyingTime-0.1f);
        }
        else if(player == UC_PlayerData.Player.Player2 && block.State == BlockTetriHandler.BlockTetriState.Occupied_Player2)
        {
            Invoke(nameof(DoOccupied), tetrisBlockSimple.occupyingTime-0.5f);
        }
        
       
    }
    
    public void CancleOccupied()
    {
        CancelInvoke(nameof(DoOccupied));

        if(!blockOccupying)
        {
            blockOccupying = tetrisBlockSimple.blocksCreator.blocks.Find((block) => block.posId == new Vector2(posId.x,posId.y)).transform.GetComponent<BlockTetriHandler>();
        }

        blockOccupying.State = BlockTetriHandler.BlockTetriState.Peace;
    }
    void DoOccupied()
    {
        if(!canCreate)return;
        if(!blockOccupying)
        {
            blockOccupying = tetrisBlockSimple.blocksCreator.blocks.Find((block) => block.posId == new Vector2(posId.x,posId.y)).transform.GetComponent<BlockTetriHandler>();
        }
        if(!blockOccupying)return;
        
        if(player == UC_PlayerData.Player.Player1)
        {
            blockOccupying.State = BlockTetriHandler.BlockTetriState.Occupied_Player1;
        }
        else if(player == UC_PlayerData.Player.Player2)
        {
            blockOccupying.State = BlockTetriHandler.BlockTetriState.Occupied_Player2;
        }
        CanMove = true;
    }
    /// <summary>
    /// 进入心流模式表现
    /// </summary>
    public void InFlow()
    {
        if(!cubeSortingGroup)return;
        cubeSortingGroup.sortingOrder = UC_PlayerData.Dispaly.FlowOrder;
    }
    /// <summary>
    /// 退出心流模式表现
    /// </summary>
    public void OutFlow()
    {
        if(!cubeSortingGroup)return;
        cubeSortingGroup.sortingOrder = UC_PlayerData.Dispaly.NotFlowOrder;
    }
    public void Display_playerColor()
    {
        Transform displayGo = transform.Find("Display_Range");
        if(!displayGo)return;
        tetriDisplayRange = displayGo.GetComponent<TetriDisplayRange>();
        Material materialP1 = new(sharedMaterial);
        Material materialP2 = new(sharedMaterial);
        // materialP1.color = PlayerData.Dispaly.Player1Color;
        // materialP2.color = PlayerData.Dispaly.Player2Color;
        materialP1.color = transform.GetComponent<SpriteRenderer>().color;
        materialP2.color = transform.GetComponent<SpriteRenderer>().color;
        if(player == UC_PlayerData.Player.Player1)
        {
            cubeSortingGroup.transform.GetComponent<MeshRenderer>().material = materialP1;
            tetriDisplayRange.SetColor(Color.red + Color.white*0.3f);
        }else if (player == UC_PlayerData.Player.Player2)
        {
            cubeSortingGroup.transform.GetComponent<MeshRenderer>().material = materialP2;
            tetriDisplayRange.SetColor(Color.blue + Color.white*0.3f);
        }
    }
    
}
