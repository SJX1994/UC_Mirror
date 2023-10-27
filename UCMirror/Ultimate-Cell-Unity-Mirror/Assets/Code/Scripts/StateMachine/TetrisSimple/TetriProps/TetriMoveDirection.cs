using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using UC_PlayerData;
using DG.Tweening;
using Mirror;
public class TetriMoveDirection : NetworkBehaviour, ITetriProp
{
    public Sprite sprite_Up,spirte_Down,sprite_Icon_Up,sprite_Icon_Down;
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
    [SyncVar]
    public PropsData.MoveDirection moveDirection = PropsData.MoveDirection.NotReady;
    Transform checker;
    Transform Checker
    {
        get
        {
            if(!checker)checker = transform.GetChild(0);
            return checker;
        }
    }
    Player turn = Player.NotReady;
    private GameObject icon;
    public GameObject Icon
    {
        get
        {
            if(!icon)icon = Checker.Find("Display").gameObject;
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
                SetCubeAlpha(0.0f);
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
    Transform cube;
    Transform Cube
    {
        get
        {
            if (!cube)cube = Checker.Find("Cube");
            return cube;
        }
    }
    void Start()
    {
        checker = transform.GetChild(0);
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
    void Event_OnUserActionStateChanged(UserAction.State UserActionStateChanged)
    {
        switch (UserActionStateChanged)
        {
            case UserAction.State.WatchingFight:
                SetCubeAlpha(0.0f);
                SetSpriteAlpha(1.0f);
                break;
            case UserAction.State.CommandTheBattle_IdeaBox:
                SetCubeAlpha(0.72f);
                SetSpriteAlpha(0.4f);
                break;
            case UserAction.State.CommandTheBattle_Buoy:
                SetCubeAlpha(0.72f);
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
        Display_Direction();
        float fixStretch = 0;
        Icon.transform.localRotation = Quaternion.Euler(Vector3.zero);
        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 fixCameraPos = new Vector3(cameraPos.x + fixStretch, cameraPos.y, cameraPos.z);
        // Vector3 directionToCamera = fixCameraPos - Icon.transform.position;
        // // directionToCamera.x = 0f;
        // Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera);
        // Icon.transform.rotation = rotationToCamera;
        Icon.transform.LookAt(fixCameraPos);
        Icon.transform.localRotation = Quaternion.Euler(Icon.transform.localRotation.eulerAngles.x, Icon.transform.localRotation.eulerAngles.y, 0);
        SetSpriteAlpha(1.0f);
    }
    public bool Generate(Player turn)
    {
        this.turn = turn;
        if(this.turn == Player.NotReady){Debug.LogError("道具“方向改变者”需要初始化“玩家”");return false;}
        // 道具计时器
        StartTimer_Growing();
        return Generate_ForPlayer();
    }
    public bool Generate_ForPlayer()
    {
        transform.SetParent(BlocksCreator.transform);
        int width =  Random.Range(1,8); 
        int height = turn == Player.Player1 ? Random.Range(3,6) : Random.Range(13,16);
        Vector2 checkId = new Vector2(height, width);
        var block = BlocksCreator.blocks.Where(b => b.posId == checkId).FirstOrDefault();
        if(block.GetComponent<BlockBuoyHandler>().tetriBuoySimple || block.GetComponent<BlockPropsState>().propsState != PropsData.PropsState.None )
        {
            Generate_ForPlayer();
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
        // float zAngel = 0;
        switch(moveDirection)
        {
            case PropsData.MoveDirection.Up:
                Checker.GetComponent<SpriteRenderer>().sprite = sprite_Icon_Up;
                Icon.GetComponent<SpriteRenderer>().sprite = sprite_Up;
                break;
            case PropsData.MoveDirection.Down:
                Checker.GetComponent<SpriteRenderer>().sprite = sprite_Icon_Down;
                Icon.GetComponent<SpriteRenderer>().sprite = spirte_Down;
                break;
        }
        // Debug.Log("Display_Direction" + posId);
        bool Isleft = posId.x <= 9;
        if(Isleft)
        {
            Icon.GetComponent<SpriteRenderer>().flipX = true;
        }else
        {
            Icon.GetComponent<SpriteRenderer>().flipX = false;
        }
        // Icon.transform.localRotation = Quaternion.Euler(new Vector3(Icon.transform.localRotation.eulerAngles.x,Icon.transform.localRotation.eulerAngles.y,zAngel));
        
    }
    public void StartTimer_Growing()
    {
        lockTime = Random.Range(1.5f, 3.0f);
        propTimer = new();
        Icon.SetActive(false);
        propTimer.StartTimer(lockTime, (UnityAction)(() => { 
            Locked = false; 
            SetSpriteAlpha(0.0f);
            this.Icon.SetActive(true);
        }));
    }
}