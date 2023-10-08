using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using UC_PlayerData;
using DG.Tweening;
public class TetriBall : MonoBehaviour , ITetriProp
{
    [SerializeField]
    public bool MoveCollect{get;set;} = true;
    public Vector2 posId;
    public LayerMask blockTargetMask;
    public KeyValuePair<TetriBall, BlockBallHandler> tetriPairBlock = new();
    public UnityAction<TetriBall> OnTetriBallCollected;
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
    Transform checker;
    Player turn = Player.NotReady;
    bool locked = true;
    float lockTime = 3.5f;
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
    private GameObject icon;
    public GameObject Icon
    {
        get
        {
            if(!icon)icon = transform.GetChild(0).Find("Display").gameObject;
            return icon;
        }
    }
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
        if(!checker)checker = transform.GetChild(0);
        if(!BlocksCreator)BlocksCreator = FindObjectOfType<BlocksCreator_Main>();    
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
        BlockBallHandler block;
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
        Transform target = checker.Find("Display");
        target.localRotation = Quaternion.Euler(Vector3.zero);
        Vector3 directionToCamera = Camera.main.transform.position - target.position;
        directionToCamera.x = 0f;
        directionToCamera.z = -1f;
        Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera);
        target.rotation = rotationToCamera;
    }
    public bool Generate(Player turn)
    {
        this.turn = turn;
        if(this.turn == Player.NotReady){Debug.LogError("道具“链式球”未初始化“玩家”");return false;}
        // 道具计时器
        propTimer = new();
        Icon.SetActive(false);
        propTimer.StartTimer(lockTime, () => {Locked = false;Icon.SetActive(true);});
        return Generate();
    }
    public void Collect()
    {
        if(locked)return;
        tetriPairBlock.Value.BlockPropsState.propsState = PropsData.PropsState.None;
        // 特效
        if(!BlocksCreator)Start();
        BlocksCreator.GetComponent<BlocksEffects>().LoadAttentionEffect(tetriPairBlock.Value.BlockDisplay,PropsData.PropsState.ChainBall);
        // 重置
        tetriPairBlock.Value.BlockPairTetri = new();
        tetriPairBlock.Value.BlockPropsState.moveCollect = false;
        tetriPairBlock = new();
        OnTetriBallCollected?.Invoke(this);
        Destroy(gameObject);
    }

    public bool Generate()
    {
        transform.SetParent(BlocksCreator.transform);
        List<BlockTetriHandler> blocks = new();
        Vector2 checkId = Vector2.zero;
        int width =  Random.Range(1,8); //blocksCreator.x-1);
        int height = turn == Player.Player1 ? Random.Range(6,9) : Random.Range(10,13); //blocksCreator.z-1);
        if(turn == Player.Player1)
        {
            blocks.AddRange(BlocksCreator.BlocksCounter.GetOccupiedP1Blocks());
            blocks.AddRange(BlocksCreator.BlocksCounter.GetPeaceP1Blocks());
            
        }else if(turn == Player.Player2)
        {
            blocks.AddRange(BlocksCreator.BlocksCounter.GetOccupiedP2Blocks());
            blocks.AddRange(BlocksCreator.BlocksCounter.GetPeaceP2Blocks());
        }
        if(blocks.Count!=0)
        {
            // 去除不在范围内的砖块
            blocks.Where(b => b.posId.x < 6 || b.posId.x > 13).ToList().ForEach(b => blocks.Remove(b));
            blocks.Where(b => b.posId.y > 8 || b.posId.y < 2).ToList().ForEach(b => blocks.Remove(b));
        }
        if(blocks.Count == 0)
        {
            checkId = new Vector2(height, width);
        }else
        { 
            checkId = blocks[Random.Range(0,blocks.Count)].posId;
        }
        var block = BlocksCreator.blocks.Where(b => b.posId == checkId).FirstOrDefault();
        if(block.GetComponent<BlockBuoyHandler>().tetriBuoySimple || block.GetComponent<BlockPropsState>().propsState != PropsData.PropsState.None)
        {
            Generate(turn);
            return false;
        }
        transform.localPosition = new Vector3(block.posId.x, 0.3f, block.posId.y);
        // transform.localScale = Vector3.one;
        transform.localScale = Vector3.one * 0.9f;
        transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        return true;
    }

    
}