using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using UC_PlayerData;
using DG.Tweening;
using Mirror;
public class TetriBall : NetworkBehaviour , ITetriProp
{
#region 数据对象
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
    public Transform Checker
    {
        get
        {
            if(!checker)checker = transform.GetChild(0);
            return checker;
        }
    }
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
                SetCubeAlpha(0.0f);
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
    Transform cube;
    Transform Cube
    {
        get
        {
            if (!cube)cube = Checker.Find("Cube");
            return cube;
        }
    }
#endregion 数据对象
#region 联网数据对象
    
#endregion 联网数据对象
#region 数据关系
    void Start()
    {
        if(!checker)checker = transform.GetChild(0);
        if(!BlocksCreator)BlocksCreator = FindObjectOfType<BlocksCreator_Main>();
        UserAction.OnPlayer1UserActionStateChanged += Event_OnUserActionStateChanged;
        UserAction.OnPlayer2UserActionStateChanged += Event_OnUserActionStateChanged;
          
    }
    void OnDisable()
    {
        UserAction.OnPlayer1UserActionStateChanged -= Event_OnUserActionStateChanged;
        UserAction.OnPlayer2UserActionStateChanged -= Event_OnUserActionStateChanged;
    }
    void LateUpdate()
    {
        propTimer.UpdateTimer();
        transform.localPosition = new Vector3(transform.localPosition.x, propTimer.NormalizedTime(), transform.localPosition.z);
    }
#endregion 数据关系
#region 数据操作
    void Event_OnUserActionStateChanged(UserAction.State UserActionStateChanged)
    {
        switch (UserActionStateChanged)
        {
            case UserAction.State.WatchingFight:
                SetCubeAlpha(0.0f);
                SetSpriteAlpha(1.0f);
                break;
            case UserAction.State.CommandTheBattle_IdeaBox:
                SetCubeAlpha(1.0f);
                SetSpriteAlpha(0.4f);
                break;
            case UserAction.State.CommandTheBattle_Buoy:
                SetCubeAlpha(1.0f);
                SetSpriteAlpha(0.4f);
                break;
            case UserAction.State.Loading:
                SetCubeAlpha(0.0f);
                break;
        }
    }
    void SetCubeAlpha(float alpha)
    {
        if(!Cube)return;
        float cubeCommandTheBattle_Alpha = alpha;
        // MeshRenderer cubeMeshRenderer = Cube.GetComponent<MeshRenderer>();
        // Color cubeBaseColor = cubeMeshRenderer.sharedMaterial.color;
        // cubeMeshRenderer.sharedMaterial.color = new Color(cubeBaseColor.r,cubeBaseColor.g,cubeBaseColor.b,cubeCommandTheBattle_Alpha);
        // Cube.GetComponent<Renderer>().sortingOrder = alpha >= 0.5f ? UC_PlayerData.Dispaly.FlowOrder : UC_PlayerData.Dispaly.NotFlowOrder;
        checker.Find("Display_Range").GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(sr => {
            sr.color = new Color(sr.color.r,sr.color.g,sr.color.b,cubeCommandTheBattle_Alpha);
            sr.sortingOrder = alpha >= 0.5f ? UC_PlayerData.Dispaly.FlowOrder : UC_PlayerData.Dispaly.NotFlowOrder;
        });
        SpriteRenderer checkerSR = checker.GetComponent<SpriteRenderer>();
        checkerSR.color = new Color(checkerSR.color.r,checkerSR.color.g,checkerSR.color.b,cubeCommandTheBattle_Alpha);
        checkerSR.sortingOrder = alpha >= 0.5f ? UC_PlayerData.Dispaly.FlowOrder : UC_PlayerData.Dispaly.NotFlowOrder;
    }
    void SetSpriteAlpha(float alpha)
    {
        float spriteCommandTheBattle_Alpha = alpha;
        SpriteRenderer tetriSpriteRenderer = Icon.GetComponent<SpriteRenderer>();
        Color tetriBaseColor = tetriSpriteRenderer.color;
        tetriSpriteRenderer.color = new Color(tetriBaseColor.r,tetriBaseColor.g,tetriBaseColor.b,spriteCommandTheBattle_Alpha);
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
        block.StopMoveProp = false;
        posId = block.PosId;
        ResetRotation();
        return true;
    }
    // 看向摄像机
    public void ResetRotation()
    {
        float fixStretch = 0;
        Transform target = checker.Find("Display");
        target.localRotation = Quaternion.Euler(Vector3.zero);
        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 fixCameraPos = new Vector3(cameraPos.x + fixStretch, cameraPos.y, cameraPos.z);
        Vector3 directionToCamera = fixCameraPos - Icon.transform.position;
        // directionToCamera.x = 0f;
        // directionToCamera.z = -1f;
        // Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera);
        // target.rotation = rotationToCamera;
        target.transform.LookAt(fixCameraPos);
        target.localRotation = Quaternion.Euler(target.localRotation.eulerAngles.x, target.localRotation.eulerAngles.y, 0);
        SetSpriteAlpha(1.0f);
    }
    public bool Generate(Player turn)
    {
        this.turn = turn;
        if(this.turn == Player.NotReady)
        {
            return Generate_First();
        }else
        {
            return Generate_ForPlayer();
        }
    }
    public bool Generate(Player turn,Vector2 specificCoordinate)
    {
        return Generate_SpecificCoordinate(specificCoordinate);
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
    public bool Generate_First()
    {
        if(transform.parent != BlocksCreator.transform)transform.SetParent(BlocksCreator.transform);
        List<BlockTetriHandler> blocks = new();
        Vector2 checkId = Vector2.zero;
        Vector2 midOfBlocksY = new(0f,10f);
        Vector2 midOfBlocksX = new(9f,11f);
        int width =  Random.Range((int)midOfBlocksY.x,(int)midOfBlocksY.y);
        int height = Random.Range((int)midOfBlocksX.x,(int)midOfBlocksX.y);
        checkId = new Vector2(height, width);
        var block = BlocksCreator.blocks.Where(b => b.posId == checkId).FirstOrDefault();
        if(block.GetComponent<BlockBuoyHandler>().tetriBuoySimple || block.GetComponent<BlockPropsState>().propsState != PropsData.PropsState.None )
        {
            Generate_First();
            return false;
        }
        StartTimer_Growing();
        transform.localPosition = new Vector3(block.posId.x, 0.3f, block.posId.y);
        transform.localScale = Vector3.one * 0.9f;
        transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        return true;
    }
    public bool Generate_ForPlayer()
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
        StartTimer_Growing();
        transform.localPosition = new Vector3(block.posId.x, 0.3f, block.posId.y);
        // transform.localScale = Vector3.one;
        transform.localScale = Vector3.one * 0.9f;
        transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        return true;
    }
    public bool Generate_SpecificCoordinate(Vector2 specificCoordinate)
    {
        if(transform.parent != BlocksCreator.transform)transform.SetParent(BlocksCreator.transform);
        List<BlockTetriHandler> blocks = new();
        Vector2 checkId = specificCoordinate;
        var block = BlocksCreator.blocks.Where(b => b.posId == checkId).FirstOrDefault();
        StartTimer_Growing();
        transform.localPosition = new Vector3(block.posId.x, 0.3f, block.posId.y);
        transform.localScale = Vector3.one * 0.9f;
        transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        return true;
    }
    public void StartTimer_Growing()
    {
        propTimer = new();
        Icon.SetActive(false);
        propTimer.StartTimer(lockTime, () => {
            Locked = false;
            SetSpriteAlpha(0.0f);
            Icon.SetActive(true);
        });
    }
#endregion 数据操作
#region 联网数据操作
    public bool Local()
    {
        if(RunModeData.CurrentRunMode == RunMode.Local)return true;
        return false;
    }
#endregion 联网数据操作
}