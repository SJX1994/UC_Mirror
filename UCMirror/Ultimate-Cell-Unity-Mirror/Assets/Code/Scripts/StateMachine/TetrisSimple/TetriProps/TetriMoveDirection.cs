using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using UC_PlayerData;
using DG.Tweening;
public class TetriMoveDirection : MonoBehaviour , ITetriProp
{
    [SerializeField]
    public bool MoveCollect{get;set;} = false;
    public Vector2 posId;
    public KeyValuePair<TetriMoveDirection, BlockMoveDirection> tetriPairBlock = new();
    public LayerMask blockTargetMask;
    public UnityAction<TetriMoveDirection> OnTetriMoveDirectionCollected;
    public BlocksCreator_Main blocksCreator;
    public BlocksCreator_Main BlocksCreator { 
        get 
        {
            if(!blocksCreator)blocksCreator = FindObjectOfType<BlocksCreator_Main>();
            return blocksCreator;
        }  
        set 
        {
            if(!value)blocksCreator = FindObjectOfType<BlocksCreator_Main>();
            blocksCreator = value;
        } 
    }
    public PropsData.MoveDirection moveDirection = PropsData.MoveDirection.NotReady;
    Transform display;
    Transform checker;
    Player turn = Player.NotReady;
    private GameObject icon;
    public GameObject Icon
    {
        get
        {
            if(!icon)icon = transform.GetChild(0).Find("Display").gameObject;
            return icon;
        }
    }
    bool locked = true;
    float lockTime = 4.5f;
    public bool Locked
    {
        get
        {
            return locked;
        }
        set
        {
            locked = value;
            if(locked)return;
            transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce).OnComplete(() => 
            {
                bool getDataSuccess = Ray_TetriPairBlock();
                while(!getDataSuccess) { Ray_TetriPairBlock(); }
            });
            
        }
    }
    public PropTimer propTimer = new();
    public struct PropTimer
    {
        public float totalTime;
        public float currentTime;
        public bool isTimerRunning;
        public UnityAction OnTimerComplete;
        public void StartTimer(float totalTime, UnityAction OnTimerComplete)
        {
            this.totalTime = totalTime;
            this.currentTime = totalTime;
            this.OnTimerComplete = OnTimerComplete;
            isTimerRunning = true;
        }
        public void UpdateTimer()
        {
            if(!isTimerRunning)return;
            currentTime -= Time.deltaTime;
            if (currentTime > 0) return;
            isTimerRunning = false;
            OnTimerComplete?.Invoke();
        }
        public float NormalizedTime()
        {
            return Remap(currentTime, 0, totalTime, 0.3f, -1);
        }
        static float Remap(float input, float oldLow, float oldHigh, float newLow, float newHigh)
        {
            float t = Mathf.InverseLerp(oldLow, oldHigh, input);
            return Mathf.Lerp(newLow, newHigh, t);
        }
    }
    void Start()
    {
        checker = transform.GetChild(0);
        display = checker.Find("Display"); 
    }
    void LateUpdate()
    {
        propTimer.UpdateTimer();
        transform.localPosition = new Vector3(transform.localPosition.x, propTimer.NormalizedTime(), transform.localPosition.z);
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
        // 道具计时器
        lockTime = Random.Range(1.5f, 3.0f);
        propTimer = new();
        Icon.SetActive(false);
        propTimer.StartTimer(lockTime, () => {Locked = false;Icon.SetActive(true);});
        return Generate();
    }
    public bool Generate()
    {
        transform.SetParent(BlocksCreator.transform);
        int width =  Random.Range(1,8); 
        int height = turn == Player.Player1 ? Random.Range(3,6) : Random.Range(13,16);
        Vector2 checkId = new Vector2(height, width);
        var block = BlocksCreator.blocks.Where(b => b.posId == checkId).FirstOrDefault();
        if(block.GetComponent<BlockBuoyHandler>().tetriBuoySimple || block.GetComponent<BlockPropsState>().propsState != PropsData.PropsState.None )
        {
            Generate();
            return false;
        }
        transform.localPosition = new Vector3(block.posId.x, 0.3f, block.posId.y);
        // transform.localScale = Vector3.one;
        transform.localScale = Vector3.one * 0.9f;
        transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        moveDirection = Random.value <= 0.5f? PropsData.MoveDirection.Up : PropsData.MoveDirection.Down;
        return true;
    }

    public void Collect()
    {
        if(locked)return;
        tetriPairBlock.Value.BlockPropsState.propsState = PropsData.PropsState.None;
        // 特效
        if(!BlocksCreator)Start();
        BlocksCreator.GetComponent<BlocksEffects>().LoadAttentionEffect(tetriPairBlock.Value.BlockDisplay,PropsData.PropsState.MoveDirectionChanger);
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