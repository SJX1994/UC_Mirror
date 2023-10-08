using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using UC_PlayerData;
using DG.Tweening;

public class TetriObstacle : MonoBehaviour, ITetriProp
{
# region 数据对象
    public Vector2 posId;
    public LayerMask blockTargetMask;
    Transform checker;
    public Transform Checker
    {
        get
        {
            if(!checker)checker = transform.GetChild(0);
            return checker;
        }
    }
    public KeyValuePair<TetriObstacle, BlockObstacle> tetriPairBlock = new();
    // 道具接口对象
    public bool MoveCollect{get;set;} = true;
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
    private GameObject icon;
    public GameObject Icon
    {
        get
        {
            if(!icon)icon = transform.GetChild(0).Find("Display").gameObject;
            return icon;
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
    bool locked = true;
    float lockTime = 0.2f;
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
    
# endregion 数据对象
# region 数据关系
    void LateUpdate()
    {
        propTimer.UpdateTimer();
        transform.localPosition = new Vector3(transform.localPosition.x, propTimer.NormalizedTime(), transform.localPosition.z);
    }
    // 道具接口关系
    public void Collect()
    {
        if(locked)return;
        tetriPairBlock.Value.BlockPropsState.propsState = PropsData.PropsState.None;
        // 特效
        BlocksCreator.GetComponent<BlocksEffects>().LoadAttentionEffect(tetriPairBlock.Value.BlockDisplay, PropsData.PropsState.Obstacle);
        // 重置
        tetriPairBlock.Value.BlockPairTetri = new();
        tetriPairBlock.Value.BlockPropsState.moveCollect = false;
        tetriPairBlock = new();
        Destroy(gameObject);
    }

    public bool Generate(Player turn)
    {
        
        return Generate();
    }
    public bool Generate()
    {
        transform.SetParent(BlocksCreator.transform);
        List<BlockTetriHandler> blocks = new();
        int width =  Random.Range(0,10);
        int height = Random.Range(9,11);
        Vector2 checkId = new Vector2(height, width);
        var block = BlocksCreator.blocks.Where(b => b.posId == checkId).FirstOrDefault();
        if(block.GetComponent<BlockBuoyHandler>().tetriBuoySimple || block.GetComponent<BlockPropsState>().propsState != PropsData.PropsState.None )
        {
            Generate();
            return false;
        }
        // 道具计时器
        propTimer = new();
        Icon.SetActive(false);
        lockTime = 0.1f;
        propTimer.StartTimer(lockTime, () => {Locked = false;Icon.SetActive(true);});
        // 方位属性
        transform.localPosition = new Vector3(block.posId.x, 0.3f, block.posId.y);
        // transform.localScale = Vector3.one;
        transform.localScale = Vector3.one * 0.9f;
        transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        return true;
    }
# endregion 数据关系
# region 数据操作
    // 道具接口操作
    public bool Ray_TetriPairBlock()
    {
        // 发射射线向下进行检测
        Ray ray = new Ray(Checker.transform.position, Vector3.down);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask);
        if (!hitBlock) {return false;}
        // 进一步的处理
        BlockObstacle block;
        hit.collider.transform.TryGetComponent(out block);
        tetriPairBlock = new(this, block);
        block.BlockPairTetri = new(block, this);
        block.MoveCollect = this.MoveCollect;
        posId = block.PosId;
        ResetRotation();
        return true;
    }

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
# endregion 数据操作
}