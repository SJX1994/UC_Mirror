using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UC_PlayerData;

public class TetriBlockSimple : MonoBehaviour
{
    public TetrisBlockSimple tetrisBlockSimple;
    public UnityAction<Player> TetriPlayerChanged;
    public Player player = UC_PlayerData.Player.NotReady;
    public Player Player
    {
        get
        {
            return player;
        }
        set
        {
            if(player == value)return;
            player = value;
            TetriPlayerChanged?.Invoke(player);
        }
    }
    public LayerMask blockTargetMask;
    public UnityAction<Vector2> TetriPosIdChanged;
    public Vector2 PosId
    {
        get
        {
            return posId;
        }
        set
        {
            if(posId == value)return;
            posId = value;
            TetriPosIdChanged?.Invoke(posId);
        }
    }
    private Vector2 posId;
    public BlockTetriHandler currentBlockTetriHandler;
    public UnityAction<TetriBlockSimple> CantPutCallback;
    private bool canMove = false;
    Color spriteColor;
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
    public bool canCreate = false;
    bool canDrop = false;
    public BlockTetriHandler blockOccupying;
    SortingGroup cubeSortingGroup;
    Material sharedMaterial;
    TetriDisplayRange tetriDisplayRange;
    TetriUnitSimple tetriUnitSimple;
    public TetriUnitSimple TetriUnitSimple
    {
        get
        {
            if(!tetriUnitSimple)tetriUnitSimple = transform.GetComponent<TetriUnitSimple>();
            return tetriUnitSimple;
        }
        set
        {
            tetriUnitSimple = value;
        }
    }
    public UnityAction<TetriBlockSimple> tetriStuckEvent;
    // Start is called before the first frame update
    void Awake()
    {
        
    }
    void Start()
    {
        tetrisBlockSimple = transform.parent.GetComponent<TetrisBlockSimple>();
        tetrisBlockSimple.OnTetrisMoveing += ()=>{CanMove = false;};
        cubeSortingGroup = transform.Find("Cube").GetComponent<SortingGroup>();
        sharedMaterial = cubeSortingGroup.transform.GetComponent<MeshRenderer>().sharedMaterial;
        Display_playerColor();
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
    public void Reset_OnDie()
    {
        bool lastOne = transform.parent.GetComponentsInChildren<TetriBlockSimple>().Length == 1;
        currentBlockTetriHandler = null;
        blockOccupying = null;
        CanMove = true;
        canDrop = true;
        canCreate = false;
        if(lastOne)Destroy(transform.parent.gameObject);
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
        List<bool> condition = new();
        if(!hitBlock){CantPutCallback?.Invoke(this);return false;}
        // 进一步的处理
        BlockTetriHandler block;
        hit.collider.transform.TryGetComponent(out block);
        if(!block){CantPutCallback?.Invoke(this); return false;}
        if(block.GetComponent<BlockPropsState>().moveCollect == true)
        {
            // 道具只能靠砖块移动收集，所以不能放置
            CantPutCallback?.Invoke(this);
            return false;
        }
        if(block.State == BlockTetriHandler.BlockTetriState.Occupying)
        {
            // 在占领中的砖块，不能放置
            CantPutCallback?.Invoke(this);
            return false;
        }
        if((player == UC_PlayerData.Player.Player1 && block.State == BlockTetriHandler.BlockTetriState.Occupied_Player2) || (player == UC_PlayerData.Player.Player1 && block.State == BlockTetriHandler.BlockTetriState.Peace_Player2))
        {
            // 对方的和平砖块，和对方已经占领的砖块 不能放置
            CantPutCallback?.Invoke(this);
            return false;
        }
        else if((player == UC_PlayerData.Player.Player2 && block.State == BlockTetriHandler.BlockTetriState.Occupied_Player1) || (player == UC_PlayerData.Player.Player2 && block.State == BlockTetriHandler.BlockTetriState.Peace_Player1))
        {
            // 对方的和平砖块，和对方已经占领的砖块 不能放置
            CantPutCallback?.Invoke(this);
            return false;
        }
        if(!block.tetriBlockSimpleHolder)
        {
            // 其他没有被占领过的砖块 可以放置
            return true;
        }
        else
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
        PosId = block.posId;
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
        PosId = block.posId;
        if(player == UC_PlayerData.Player.Player1 && block.State!= BlockTetriHandler.BlockTetriState.Occupied_Player1)
        {
            block.State = BlockTetriHandler.BlockTetriState.Occupying;   
        }
        else if(player == UC_PlayerData.Player.Player2 && block.State!= BlockTetriHandler.BlockTetriState.Occupied_Player2)
        {
            block.State = BlockTetriHandler.BlockTetriState.Occupying;   
        }
        
    }
    public BlockTetriHandler NextBlock_X()
    {
        BlocksCreator_Main blocksCreator = tetrisBlockSimple.blocksCreator;
        var blockP1 = blocksCreator.blocks.Find((block) => block.posId == new Vector2(PosId.x + 1,PosId.y));
        var blockP2 = blocksCreator.blocks.Find((block) => block.posId == new Vector2(PosId.x - 1,PosId.y));
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
    public BlockTetriHandler NextBlock_Z(bool moveUp)
    {
        BlocksCreator_Main blocksCreator = tetrisBlockSimple.blocksCreator;
        BlockDisplay blockZ = null;
        int checkDir = moveUp ? 1 : -1;
        blockZ = blocksCreator.blocks.Find((block) => block.posId == new Vector2(PosId.x,PosId.y+ checkDir));
        if(!blockZ){tetriStuckEvent?.Invoke(this);return null;} // 卡住事件
        return blockZ.GetComponent<BlockTetriHandler>();
    }
    public BlockTetriHandler CurrentBlock()
    {
        BlocksCreator_Main blocksCreator = tetrisBlockSimple.blocksCreator;
        BlockTetriHandler currentBlock = blocksCreator.blocks.Find((block) => block.posId == new Vector2(PosId.x,PosId.y)).GetComponent<BlockTetriHandler>();
        return currentBlock;
    }
    public bool BlockNextCheck(BlockTetriHandler block)
    {
        if(!block)return false;
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
    public bool BlockNextCheckBuoy(BlockTetriHandler block)
    {
        if(!block)return false;
        if(block.GetComponent<BlockBuoyHandler>().tetriBuoySimple)
        {
            return false;
        }
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
        PosId = block.posId;
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
            blockOccupying = tetrisBlockSimple.blocksCreator.blocks.Find((block) => block.posId == new Vector2(PosId.x,PosId.y)).transform.GetComponent<BlockTetriHandler>();
        }
        blockOccupying.State = BlockTetriHandler.BlockTetriState.Peace;
    }
    void DoOccupied()
    {
        if(!canCreate)return;
        if(!blockOccupying)
        {
            blockOccupying = tetrisBlockSimple.blocksCreator.blocks.Find((block) => block.posId == new Vector2(PosId.x,PosId.y)).transform.GetComponent<BlockTetriHandler>();
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
        if(!cubeSortingGroup || !sharedMaterial){
            cubeSortingGroup = transform.Find("Cube").GetComponent<SortingGroup>();
            sharedMaterial = cubeSortingGroup.transform.GetComponent<MeshRenderer>().sharedMaterial;
        }
        spriteColor = transform.GetComponent<SpriteRenderer>().color;
        Material materialP1 = new(sharedMaterial);
        Material materialP2 = new(sharedMaterial);
        // materialP1.color = PlayerData.Dispaly.Player1Color;
        // materialP2.color = PlayerData.Dispaly.Player2Color;
        materialP1.color = spriteColor;
        materialP2.color = spriteColor;
        if(player == Player.Player1)
        {
            cubeSortingGroup.transform.GetComponent<MeshRenderer>().material = materialP1;
            tetriDisplayRange.SetColor(Color.red + Color.white*0.3f);
        }else if (player == Player.Player2)
        {
            cubeSortingGroup.transform.GetComponent<MeshRenderer>().material = materialP2;
            tetriDisplayRange.SetColor(Color.blue + Color.white*0.3f);
        }
    }
    public void Display_playerColor(bool Visible)
    {
        Transform displayGo = transform.Find("Display_Range");
        if(!displayGo)return;
        tetriDisplayRange = displayGo.GetComponent<TetriDisplayRange>();
        if(!cubeSortingGroup || !sharedMaterial){
            cubeSortingGroup = transform.Find("Cube").GetComponent<SortingGroup>();
            sharedMaterial = cubeSortingGroup.transform.GetComponent<MeshRenderer>().sharedMaterial;
        }
        spriteColor = transform.GetComponent<SpriteRenderer>().color;
        Material materialP1 = new(sharedMaterial);
        Material materialP2 = new(sharedMaterial);
        // materialP1.color = PlayerData.Dispaly.Player1Color;
        // materialP2.color = PlayerData.Dispaly.Player2Color;
        materialP1.color = spriteColor;
        materialP2.color = spriteColor;
        Color red = Color.red + Color.white*0.3f;
        Color blue = Color.blue + Color.white*0.3f;
        
        if(player == Player.Player1)
        {
            cubeSortingGroup.transform.GetComponent<MeshRenderer>().material = materialP1;
            tetriDisplayRange.SetColor(red);
        }else if (player == Player.Player2)
        {
            cubeSortingGroup.transform.GetComponent<MeshRenderer>().material = materialP2;
            tetriDisplayRange.SetColor(blue);
        }
        if(Visible)return;
        materialP1.color = new(materialP1.color.r,materialP1.color.g,materialP1.color.b,Dispaly.HidenAlpha);
        materialP2.color = new(materialP2.color.r,materialP2.color.g,materialP2.color.b,Dispaly.HidenAlpha);
        if(player == Player.Player1)
        {
            cubeSortingGroup.transform.GetComponent<MeshRenderer>().material = materialP1;
            tetriDisplayRange.SetColor(new(red.r,red.g,red.b,Dispaly.HidenAlpha));
        }else if (player == Player.Player2)
        {
            cubeSortingGroup.transform.GetComponent<MeshRenderer>().material = materialP2;
            tetriDisplayRange.SetColor(new(blue.r,blue.g,blue.b,Dispaly.HidenAlpha));
        }
        transform.GetComponent<SpriteRenderer>().color = new(spriteColor.r,spriteColor.g,spriteColor.b,Dispaly.HidenAlpha);
    }
    
}
