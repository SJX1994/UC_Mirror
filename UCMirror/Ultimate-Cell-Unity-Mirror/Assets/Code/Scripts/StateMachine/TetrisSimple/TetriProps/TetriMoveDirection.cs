using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using UC_PlayerData;
public class TetriMoveDirection : MonoBehaviour,ITetriProp
{
    [SerializeField]
    public bool MoveCollect{get;set;} = false;
    public Vector2 posId;
    public KeyValuePair<TetriMoveDirection, BlockMoveDirection> tetriPairBlock = new();
    public LayerMask blockTargetMask;
    public UnityAction<TetriMoveDirection> OnTetriMoveDirectionCollected;
    public BlocksCreator blocksCreator;
    public BlocksCreator BlocksCreator { 
        get 
        {
            if(!blocksCreator)blocksCreator = FindObjectOfType<BlocksCreator>();
            return blocksCreator;
        }  
        set 
        {
            if(!value)blocksCreator = FindObjectOfType<BlocksCreator>();
            blocksCreator = value;
        } 
    }
    public PropsData.MoveDirection moveDirection = PropsData.MoveDirection.NotReady;
    Transform display;
    Transform checker;
    Player turn = Player.NotReady;
    void Start()
    {
        checker = transform.GetChild(0);
        display = checker.Find("Display"); 
    }
    public bool Ray_TetriPairBlock()
    {
        // 发射射线向下进行检测
        if(!checker)checker = transform.GetChild(0);
        Ray ray = new Ray(checker.transform.position, Vector3.down);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if (!hitBlock) {return false;}
        // 进一步的处理
        BlockMoveDirection block;
        hit.collider.transform.TryGetComponent(out block);
        tetriPairBlock = new(this, block);
        block.BlockPairTetri = new(block, this);
        block.MoveCollect = this.MoveCollect;
        posId = block.PosId;
        ResetRotation();
        return true;
    }
    // 看向摄像机
    public void ResetRotation()
    {
        if(!display)Start();
        display.localRotation = Quaternion.Euler(Vector3.zero);
        Vector3 directionToCamera = Camera.main.transform.position - display.position;
        directionToCamera.x = 0f;
        Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera);
        display.rotation = rotationToCamera;
        Display_Direction();
    }
    public bool Generate(Player turn)
    {
        this.turn = turn;
        if(this.turn == Player.NotReady){Debug.LogError("道具“方向改变者”需要初始化“玩家”");return false;}
        return Generate();
    }
    public bool Generate()
    {
        transform.SetParent(blocksCreator.transform);
        int width =  Random.Range(1,8); 
        int height = turn == Player.Player1 ? Random.Range(3,6) : Random.Range(13,16);
        Vector2 checkId = new Vector2(height, width);
        var block = blocksCreator.blocks.Where(b => b.posId == checkId).FirstOrDefault();
        if(block.GetComponent<BlockBuoyHandler>().tetriBuoySimple || block.GetComponent<BlockPropsState>().propsState != PropsData.PropsState.None )
        {
            Generate();
            return false;
        }
        transform.localPosition = new Vector3(block.posId.x, 0.3f, block.posId.y);
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        moveDirection = Random.value <= 0.5f? PropsData.MoveDirection.Up : PropsData.MoveDirection.Down;
        bool getDataSuccess = Ray_TetriPairBlock();
        while(!getDataSuccess)
        {
            Generate();
        }
        return getDataSuccess;
    }

    public void Collect()
    {
        tetriPairBlock.Value.BlockPropsState.propsState = PropsData.PropsState.None;
        // 特效
        if(!blocksCreator)Start();
        blocksCreator.GetComponent<BlocksEffects>().LoadAttentionEffect(tetriPairBlock.Value.BlockDisplay);
        // 重置
        tetriPairBlock.Value.BlockPropsState.moveCollect = false;
        tetriPairBlock.Value.BlockPairTetri = new();
        tetriPairBlock = new();
        OnTetriMoveDirectionCollected?.Invoke(this);
        Destroy(gameObject);
    }
    void Display_Direction()
    {
        // 旋转
        if(!display)Start();
        float zAngel = 0;
        switch(moveDirection)
        {
            case PropsData.MoveDirection.Up:
                zAngel = 90;
                break;
            case PropsData.MoveDirection.Down:
                zAngel = 270;
                break;
        }
        display.localRotation = Quaternion.Euler(new Vector3(display.localRotation.eulerAngles.x,display.localRotation.eulerAngles.y,zAngel));
        
    }
}