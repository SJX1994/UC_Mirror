using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
using System;
using DG.Tweening;
using Mirror;
using System.Linq;
public class TetriUnitSimple : NetworkBehaviour
{
#region 数据对象
    [SerializeField]
    public int tetriUnitIndex;
    public TetriUnitTemplate tetriUnitTemplate;
    public UnitSimple haveUnit;
    public UnitSimple HaveUnit
    {
        get
        {
            if(haveUnit)return haveUnit;
            if(!haveUnit)haveUnit = FindUnitSimple();
            if(!haveUnit)
            {
                Start();
                FindUnitSimple();
            }
            return haveUnit;
        }
        set
        {
            haveUnit = value;
        }
    }
    UnitSimple loadUnit;
    public Vector2 PosId
    {
        get
        {
            if(dead)return default(Vector2);
            return TetriBlock.PosId;
        }
        
    }
    TetriBlockSimple tetriBlock;
    public TetriBlockSimple TetriBlock
    {
        get
        {
            if(dead)return null;
            if(!tetriBlock)tetriBlock = GetComponent<TetriBlockSimple>();
            return tetriBlock;
        }
    }
    public TetrisBlockSimple TetrisBlockSimple
    {
        get
        {
            if(dead)return null;
            if(!tetriBlock)tetriBlock = GetComponent<TetriBlockSimple>();
            return tetriBlock.tetrisBlockSimple;
        }
    }
    private TetriBuoySimple tetriBuoy;
    public TetriBuoySimple TetriBuoy
    {
        get
        {
            if(!tetriBuoy)tetriBuoy = GetComponent<TetriBuoySimple>();
            return tetriBuoy;
        }
    }
    public TetriBuoySimple tetriTemp;
    public TetrisUnitSimple tetrisUnitSimple;
    public TetrisUnitSimple TetrisUnitSimple
    {
        get
        {
            if(!tetrisUnitSimple)tetrisUnitSimple = transform.parent.GetComponent<TetrisUnitSimple>();
            return tetrisUnitSimple;
        }
    }
    public UnitData.Color type = UnitData.Color.notReady;
    // 创建键值对
    public KeyValuePair<int, UnitData.Color> indexPairColor = new();
    [HideInInspector]
    public bool newTetriUnit = true;
    // 道具变量暂存
    private PropsData.MoveDirection moveDirectionCatch = PropsData.MoveDirection.NotReady;
    public PropsData.MoveDirection MoveDirectionCatch
    {
        get
        {
            return moveDirectionCatch;
        }
        set
        {
            moveDirectionCatch = value;
        }
    }
    bool dead = false;
#endregion 数据对象
#region 联网数据对象
    NetworkManagerUC_PVP networkManagerUC_PVP;
    NetworkManagerUC_PVP NetworkManagerUC_PVP
    {
        get{
            if(!isServer)return null;
            if(!networkManagerUC_PVP)networkManagerUC_PVP = FindObjectOfType<NetworkManagerUC_PVP>();
            return networkManagerUC_PVP;
        }
    }
#endregion 联网数据对象
#region 数据关系
    public void Start()
    {
        if(Local())
        {
            dead = false;
            if(!loadUnit){loadUnit = Resources.Load<UnitSimple>("UnitSimple");}
            tetriBlock = GetComponent<TetriBlockSimple>();
            tetriBuoy = GetComponent<TetriBuoySimple>();
            tetrisUnitSimple = transform.parent.GetComponent<TetrisUnitSimple>();
            if(!newTetriUnit)return;
            HaveUnit = CreatNewCellUnit() as UnitSimple;
        }else
        {
            if(!isServer)return;
            dead = false;
            tetriBlock = GetComponent<TetriBlockSimple>();
            tetriBuoy = GetComponent<TetriBuoySimple>();
            tetrisUnitSimple = transform.parent.GetComponent<TetrisUnitSimple>();
            if(!newTetriUnit)return;
            HaveUnit = CreatNewCellUnit() as UnitSimple;
        }
        
    }
    public void Init()
    {
        if(!HaveUnit)return;
        HaveUnit.Awake();
        HaveUnit.Start();
        Start();
    }
    public UnitSimple FindUnitSimple()
    {
        UnitSimple findUnit = GetComponentInChildren<UnitSimple>();
        if(!findUnit)return null;
        HaveUnit = findUnit;
        return findUnit;
    }
    public Unit CreatNewCellUnit()
    {
        if(Local())
        {
            // 初始化
            if(!tetriBlock || !tetriBuoy){Init();}
            Unit unit = Instantiate(loadUnit,transform);
            unit.unitTemplate.player = tetriBlock.tetrisBlockSimple.player;
            // OnUnitCreat.Invoke(cell.GetComponent<Soldier>());
            string[] skinName = {"red","green","blue","purple"};
            unit.skinName = skinName[UnityEngine.Random.Range(0, 4)];
            type = (UnitData.Color)Enum.Parse(typeof(UnitData.Color), unit.skinName);
            unit.tag = "Untagged";
            unit.SkeletonRenderer.Skeleton.SetSkin(unit.skinName);
            unit.idV2 = tetriBlock.PosId;
            UnitSimple unitSimple = unit as UnitSimple;
            unitSimple.ShaderInit();
            unitSimple.TetriUnitSimple = this;
            unitSimple.DurationRunning = tetriBlock.tetrisBlockSimple.OccupyingTime;
            unitSimple.ResetRotation();
            unitSimple.SetFlip();
            indexPairColor = new KeyValuePair<int, UnitData.Color>(tetriUnitIndex,type);
            // 事件监听 Unit
            unit.OnDie += Event_UnitDie;
            // 事件监听 Tetri
            tetriBlock.tetrisBlockSimple.OnRotate += unitSimple.Event_OnRotate;
            tetriBlock.tetrisBlockSimple.OnStartMove += unitSimple.Event_Display_StartRunning;
            tetriBlock.tetrisBlockSimple.OnTetrisMoveing += unitSimple.Event_OnTetrisMoveing;
            tetriBlock.TetriPosIdChanged += unitSimple.Event_OnTetriPosIdChanged;
            tetriBlock.tetriStuckEvent += unitSimple.Event_Display_OnTetriStuck;
            tetriBuoy.OnTetriTempChange += Event_GetTetriTemp;
            // 武器
            Weapon weapon;
            if (!unit.SkeletonRenderer.transform.TryGetComponent(out weapon))return null;
            weapon.SetWeapon(type);
            tetrisUnitSimple.InitProcess++;
            return unit;
        }else
        {
            
            return Server_InstantiateNewCellUnit();
        }
        
    }
    public Unit LoadUnit(KeyValuePair<int, UnitData.Color> loadIndexPairColor)
    {
        if(Local())
        {
            // 初始化
            if(!tetriBlock || !tetriBuoy){Init();}
            Unit unit = HaveUnit;
            unit.unitTemplate.player = tetriBlock.tetrisBlockSimple.player;
            unit.skinName = loadIndexPairColor.Value.ToString();
            type = (UnitData.Color)Enum.Parse(typeof(UnitData.Color), unit.skinName);
            unit.tag = "Untagged";
            unit.SkeletonRenderer.Skeleton.SetSkin(unit.skinName);
            unit.idV2 = tetriBlock.PosId;
            UnitSimple unitSimple = unit as UnitSimple;
            unitSimple.ShaderInit();
            unitSimple.DurationRunning = tetriBlock.tetrisBlockSimple.OccupyingTime;
            unitSimple.ResetRotation();
            unitSimple.SetFlip();
            indexPairColor = new KeyValuePair<int, UnitData.Color>(tetriUnitIndex,type);
            // 事件监听 Unit
            unit.OnDie += Event_UnitDie;
            // 事件监听 Tetri
            tetriBlock.tetrisBlockSimple.OnRotate += unitSimple.Event_OnRotate;
            tetriBlock.tetrisBlockSimple.OnStartMove += unitSimple.Event_Display_StartRunning;
            tetriBlock.tetrisBlockSimple.OnTetrisMoveing += unitSimple.Event_OnTetrisMoveing;
            tetriBlock.TetriPosIdChanged += unitSimple.Event_OnTetriPosIdChanged;
            tetriBlock.tetriStuckEvent += unitSimple.Event_Display_OnTetriStuck;
            tetriBuoy.OnTetriTempChange += Event_GetTetriTemp;
            // 武器
            Weapon weapon;
            if (!unit.SkeletonRenderer.transform.TryGetComponent(out weapon))return null;
            weapon.SetWeapon(type);
            tetrisUnitSimple.InitProcess++;
            return unit;
        }else
        {
            
            return null;
        }
    }
#endregion 数据关系
#region 数据操作
    public void LevelUp(int level)
    {
        HaveUnit.LevelUpDisplay(level);
    }
    public void TetrisSpeedModify(float slowDown)
    {
        TetrisBlockSimple.OccupyingTime += slowDown;
        TetrisBlockSimple.Stop(false);
        TetrisBlockSimple.Move();
    }
    public void TetrisSpeedNormal()
    {
        float normalSpeed = 3.0f;
        TetrisBlockSimple.OccupyingTime = normalSpeed;
        TetrisBlockSimple.Move();
    }
    public void PlayBlockEffect()
    {
        if(Local())
        {
            if(!TetriBuoy.blockBuoyHandler)return;
            BlockDisplay blockDisplay = TetriBuoy.blockBuoyHandler.BlockDisplay;
            BlocksEffects bE =  TetrisBlockSimple.blocksCreator.BlocksEffects;
            bE.LoadAttentionEffect(blockDisplay,TetrisBlockSimple.player);
        }else
        {
            if(!isServer)return;
            if(!TetriBuoy.blockBuoyHandler)return;
            BlockDisplay blockDisplay = TetriBuoy.blockBuoyHandler.BlockDisplay;
            BlocksEffects bE =  TetrisBlockSimple.blocksCreator.BlocksEffects;
            bE.Server_LoadAttentionEffect(blockDisplay,TetrisBlockSimple.player);
            // Client_PlayBlockEffect(blockDisplay.posId);
        }
        
    }
    public void PlayBlockEffect(string floatingwordToShow , Color color32 = default)
    {
        if(Local())
        {
            if(!TetriBuoy.blockBuoyHandler)return;
            BlockDisplay blockDisplay = TetriBuoy.blockBuoyHandler.BlockDisplay;
            BlocksEffects bE =  TetrisBlockSimple.blocksCreator.BlocksEffects;
            bE.LoadAttentionEffect(blockDisplay,TetrisBlockSimple.player,floatingwordToShow,color32);
        }else
        {
            if(!isServer)return;
            if(!TetriBuoy.blockBuoyHandler)return;
            BlockDisplay blockDisplay = TetriBuoy.blockBuoyHandler.BlockDisplay;
            BlocksEffects bE =  TetrisBlockSimple.blocksCreator.BlocksEffects;
            bE.Server_LoadAttentionEffect(blockDisplay,TetrisBlockSimple.player,floatingwordToShow,color32);
            // Client_PlayBlockEffect(blockDisplay.posId);
        }
        
    }
    [ClientRpc]
    void Client_PlayBlockEffect(Vector2 posId)
    {
        BlocksCreator_Main blocksCreator_Main = FindObjectOfType<BlocksCreator_Main>();
        BlockDisplay blockDisplay = blocksCreator_Main.blocks.Find((block) => block.posId == posId);
        if(!blockDisplay || !blocksCreator_Main)return;
        BlocksEffects bE =  TetrisBlockSimple.blocksCreator.BlocksEffects;
        bE.LoadAttentionEffect(blockDisplay,TetrisBlockSimple.player);
    }
    void Event_GetTetriTemp(TetriBuoySimple temp)
    {
        if(temp==null)return;
        tetriTemp = temp;
    }
    public void Event_UnitDie(Unit whoDie)
    {
        float timeToDestory = 1.0f;
        // 取消Dotween
        if(HaveUnit.tween_running!=null)
        {
            HaveUnit.tween_running?.Kill();
            HaveUnit.tween_running = null;
        }
        if(HaveUnit.tween_DieScale!=null)
        {
            HaveUnit.tween_DieScale?.Kill();
            HaveUnit.tween_DieScale = null;
        }
        if(HaveUnit.tween_OnEndDragDisplay!=null)
        {
            HaveUnit.tween_OnEndDragDisplay?.Kill();
            HaveUnit.tween_OnEndDragDisplay = null;
        }
        UnitSimple unitSimple = whoDie as UnitSimple;
        // 取消协程
        unitSimple.Soldier.ChainTransfer.ClearChainTransferBeforDie();
        unitSimple.Soldier.StopAllCoroutines();
        unitSimple.Soldier.ChainTransfer.StopAllCoroutines();
        unitSimple.Soldier.WeakAssociation.StopAllCoroutines();
        unitSimple.Soldier.FourDirectionsLinks.StopAllCoroutines();
        // 表现
        ParticleSystem dieParticle = unitSimple.Soldier.Effect.effectCollection.Display_setSkine(unitSimple.Soldier);
        dieParticle = Instantiate(dieParticle);
        dieParticle.transform.position = unitSimple.transform.position;
        dieParticle.Play();
        Destroy(dieParticle.gameObject, 1.0f);
        // 恢复砖块状态
        // tetriBlock.canCreate = false;
        tetriBlock.CancleOccupied();
        // TetrisBlockSimple.Move();
        // 取消后续事件检测
        tetrisUnitSimple.TetriUnits.Remove(this);
        tetrisUnitSimple.GetComponent<TetrisBlockSimple>().ChildTetris.Remove(this.GetComponent<TetriBlockSimple>());
        tetrisUnitSimple.GetComponent<TetrisBuoySimple>().ChildTetris.Remove(this.GetComponent<TetriBuoySimple>());
        EvaluatePioneersWhenUnitDie();
        // 取消事件监听
        tetriBuoy.OnTetriTempChange -= Event_GetTetriTemp;
        tetriBlock.tetrisBlockSimple.OnRotate -= unitSimple.Event_OnRotate;
        tetriBlock.tetrisBlockSimple.OnStartMove -= unitSimple.Event_Display_StartRunning;
        tetriBlock.tetrisBlockSimple.OnTetrisMoveing -= unitSimple.Event_OnTetrisMoveing;
        tetriBlock.tetriStuckEvent -= unitSimple.Event_Display_OnTetriStuck;
        tetriBlock.tetrisBlockSimple.ChildTetris.Remove(this.GetComponent<TetriBlockSimple>());
        tetriBlock.tetrisBlockSimple.sequence?.Kill();
        tetriBlock.Reset_OnDie();
        tetriBuoy.Reset();
        dead = true;
        Destroy(gameObject,timeToDestory);
    }
    public void FailToCreat()
    {
        if(!HaveUnit)return;
        HaveUnit.ResetRotation();
        HaveUnit.EnableSelectEffect = true;
    }
    public void UnitTemp()
    {
        HaveUnit.gameObject.SetActive(false);
        this.enabled = false;
    }
    public void RemoveUnit()
    {
        Destroy(HaveUnit.gameObject,0.1f);
    }
    public void SetFightTag(bool needFight = true)
    {
        if(!HaveUnit)return;
        if(needFight)
        {
            HaveUnit.tag = HaveUnit.unitTemplate.player.ToString();
        }else
        {
            HaveUnit.tag = "Untagged";
        }
        
    }
    public PropsData.PropsState Ray_PorpCollect()
    {
        if(!HaveUnit) return PropsData.PropsState.None;
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        bool hitBlock = Physics.Raycast(ray, out hit, Mathf.Infinity, tetriBlock.blockTargetMask);
        if (!hitBlock) return PropsData.PropsState.None;
        // 进一步的处理
        BlockPropsState block;
        hit.collider.transform.TryGetComponent(out block);
        switch(block.propsState)
        {
            case PropsData.PropsState.ChainBall:
                block.BlockBallHandler.Collect();
            break;
            case PropsData.PropsState.MoveDirectionChanger:
                if(block.BlockMoveDirection.BlockPairTetri.Value != null)
                {
                    MoveDirectionCatch = block.BlockMoveDirection.BlockPairTetri.Value.moveDirection;
                }
                block.BlockMoveDirection.Collect();
            break;
            case PropsData.PropsState.Obstacle:
                block.BlockObstacle.Collect();
            break;
        }   
        return block.propsState;
    }
    public void OnBeginDragDisplay()
    {
        if(Local())
        {
            HaveUnit.Display_OnBeginDragDisplay();
        }else
        {
            if(!isServer)return;
            HaveUnit.Server_Display_OnBeginDragDisplay();
        }
        
    }
    public void OnEditingStatusAfterSelection()
    {
        HaveUnit.Display_OnEditingStatusAfterSelection();
    }
    public void OnEndDragDisplay()
    {
        if(Local())
        {
            HaveUnit.Display_OnEndDragDisplay();
        }else
        {
            if(!isServer)return;
            HaveUnit.Server_Display_OnEndDragDisplay();
        }
        
    }
    public void Display_UserCommandTheBattle()
    {
        HaveUnit.Display_UserCommandTheBattle();
    }
    public void Display_UserWatchingFight()
    {
        HaveUnit.Display_UserWatchingFight();
    }
    public void Display_HideUnit()
    {
        HaveUnit.Display_HideUnit();
    }
    public void Display_ShowForPlayerScreen()
    {
        HaveUnit.ShowForPlayerScreen();
    }
    public void Display_ShowUnit()
    {
        HaveUnit.Display_ShowUnit();
    }
    public void SetUnitSortingOrderToFlow()
    {
        HaveUnit.SetUnitSortingOrderToFlow();
    }
    public void SetUnitSortingOrderToNormal()
    {
        HaveUnit.SetUnitSortingOrderToNormal();
    }
    void EvaluatePioneersWhenUnitDie()
    {
        tetriBlock.tetrisBlockSimple.pioneerTetris.Remove(this.GetComponent<TetriBlockSimple>());
        tetriBlock.tetrisBlockSimple.Stop(false);
        tetriBlock.tetrisBlockSimple.EvaluatePioneers_X();
        tetriBlock.tetrisBlockSimple.Move();
        // if(tetriBlock.tetrisBlockSimple.moveStepX!=0)
        // {
           
        // }else if(tetriBlock.tetrisBlockSimple.moveStepZ != 0)
        // {
        //     tetriBlock.tetrisBlockSimple.Stop(false);
        //     tetriBlock.tetrisBlockSimple.EvaluatePioneers_Z();
        //     tetriBlock.tetrisBlockSimple.Move();
        // }
    }
#endregion 数据操作
#region 联网数据操作
    public bool Local()
    {
        if(RunModeData.CurrentRunMode == RunMode.Local)return true;
        return false;
    }
    struct Server_Client_InstantiateNewCellUnit_Data
    {
        public uint unitNetId;
        public Vector3 localPosition;
        public Vector3 localScale;
        public string skinName;
    }
    [Server]
    Unit Server_InstantiateNewCellUnit()
    {
        if(!tetriBlock || !tetriBuoy){Init();}
        if(haveUnit)return haveUnit;
        Unit unit = haveUnit;
        if(!FindUnitSimple())
        {
            unit = Instantiate(NetworkManagerUC_PVP.spawnPrefabs.Find(prefab => prefab.name == "UnitSimple"),transform).GetComponent<Unit>();
        }
        HaveUnit = unit as UnitSimple;
        unit.unitTemplate.player = tetriBlock.TetrisBlockSimple.player;
        string[] skinName = {"red","green","blue","purple"};
        unit.skinName = skinName[UnityEngine.Random.Range(0, 4)];
        type = (UnitData.Color)Enum.Parse(typeof(UnitData.Color), unit.skinName);
        unit.tag = "Untagged";
        unit.SkeletonRenderer.Skeleton.SetSkin(unit.skinName);
        unit.idV2 = tetriBlock.PosId;
        UnitSimple unitSimple = unit as UnitSimple;
        unitSimple.DifferentPlayerInit();
        // unitSimple.ShaderInit();
        unitSimple.TetriUnitSimple = this;
        unitSimple.DurationRunning = tetriBlock.tetrisBlockSimple.OccupyingTime;
        // unitSimple.ResetRotation();
        // unitSimple.SetFlip();
        indexPairColor = new KeyValuePair<int, UnitData.Color>(tetriUnitIndex,type);
        // 事件监听 Unit
        unit.OnDie += Event_UnitDie;
        // 事件监听 Tetri
        tetriBlock.tetrisBlockSimple.OnRotate += unitSimple.Event_OnRotate;
        tetriBlock.tetrisBlockSimple.OnStartMove += unitSimple.Event_Display_StartRunning;
        tetriBlock.tetrisBlockSimple.OnTetrisMoveing += unitSimple.Event_OnTetrisMoveing;
        tetriBlock.TetriPosIdChanged += unitSimple.Event_OnTetriPosIdChanged;
        tetriBlock.tetriStuckEvent += unitSimple.Event_Display_OnTetriStuck;
        tetriBuoy.OnTetriTempChange += Event_GetTetriTemp;
        // 武器
        Weapon weapon;
        if (!unit.SkeletonRenderer.transform.TryGetComponent(out weapon))return null;
        weapon.SetWeapon(type);
        tetrisUnitSimple.InitProcess++;
        NetworkServer.Spawn(unit.gameObject);
        Server_Client_InstantiateNewCellUnit_Data data = new();
        data.unitNetId = unit.netId;
        data.localPosition = unit.transform.localPosition;
        data.localScale = unit.transform.localScale;
        data.skinName = unit.skinName;
        Client_InstantiateNewCellUnit(data);
        return unit;
    }
    [ClientRpc]
    void Client_InstantiateNewCellUnit(Server_Client_InstantiateNewCellUnit_Data dataIn)
    {
        HaveUnit = FindObjectsOfType<UnitSimple>().ToList().Find(ball => ball.netId == dataIn.unitNetId);
        HaveUnit.TetriUnitSimple = this;
        HaveUnit.transform.SetParent(transform);
        HaveUnit.transform.localPosition = dataIn.localPosition;
        HaveUnit.transform.localScale = dataIn.localScale;
        HaveUnit.SkeletonRenderer.Skeleton.SetSkin(dataIn.skinName);
        UnitSimple unitSimple = HaveUnit as UnitSimple;
        // unitSimple.ShaderInit();
        // unitSimple.ResetRotation();
        // unitSimple.SetFlip();
        // 武器
        Weapon weapon;
        type = (UnitData.Color)Enum.Parse(typeof(UnitData.Color), dataIn.skinName);
        if(!unitSimple.SkeletonRenderer.transform.TryGetComponent(out weapon))return;
        weapon.SetWeapon(type);
        // Display_ShowUnit();
    }
    [Server]
    public Unit Server_LoadCellUnit(string skinName)
    {
        if(skinName == "notReady")return null;
        FindUnitSimple();
        if(!HaveUnit)return null;
        // 初始化
        if(!tetriBlock || !tetriBuoy){Init();}
        Unit unit = HaveUnit;
        if(!unit)return null;
        unit.unitTemplate.player = tetriBlock.tetrisBlockSimple.player;
        unit.skinName = skinName;
        type = (UnitData.Color)Enum.Parse(typeof(UnitData.Color), unit.skinName);
        unit.tag = "Untagged";
        if(!unit.skeletonRenderer)return null;
        unit.SkeletonRenderer.Skeleton.SetSkin(unit.skinName);
        unit.idV2 = tetriBlock.PosId;
        UnitSimple unitSimple = unit as UnitSimple;
        unitSimple.DifferentPlayerInit();
        // unitSimple.ShaderInit();
        unitSimple.DurationRunning = tetriBlock.tetrisBlockSimple.OccupyingTime;
        // unitSimple.ResetRotation();
        // unitSimple.SetFlip();
        indexPairColor = new KeyValuePair<int, UnitData.Color>(tetriUnitIndex,type);
        // 事件监听 Unit
        unit.OnDie += Event_UnitDie;
        // 事件监听 Tetri
        tetriBlock.tetrisBlockSimple.OnRotate += unitSimple.Event_OnRotate;
        tetriBlock.tetrisBlockSimple.OnStartMove += unitSimple.Event_Display_StartRunning;
        tetriBlock.tetrisBlockSimple.OnTetrisMoveing += unitSimple.Event_OnTetrisMoveing;
        tetriBlock.TetriPosIdChanged += unitSimple.Event_OnTetriPosIdChanged;
        tetriBlock.tetriStuckEvent += unitSimple.Event_Display_OnTetriStuck;
        tetriBuoy.OnTetriTempChange += Event_GetTetriTemp;
        // 武器
        Weapon weapon;
        if (!unit.SkeletonRenderer.transform.TryGetComponent(out weapon))return null;
        weapon.SetWeapon(type);
        tetrisUnitSimple.InitProcess++;
        Client_LoadCellUnit(unit.netId,skinName);
        return unit;
    }
    [ClientRpc]
    void Client_LoadCellUnit(uint serverUnitNetId,string skinName)
    {
        HaveUnit = FindObjectsOfType<UnitSimple>().ToList().Find(x => x.netId == serverUnitNetId);
        HaveUnit.TetriUnitSimple = this;
        HaveUnit.transform.SetParent(transform);
        HaveUnit.transform.localPosition = Vector3.zero;
        HaveUnit.transform.localScale = Vector3.one;
        HaveUnit.SkeletonRenderer.Skeleton.SetSkin(skinName);
        UnitSimple unitSimple = HaveUnit as UnitSimple;
        // unitSimple.ShaderInit();
        // unitSimple.ResetRotation();
        // unitSimple.SetFlip();
        // 武器
        Weapon weapon;
        type = (UnitData.Color)Enum.Parse(typeof(UnitData.Color), skinName);
        if(!unitSimple.SkeletonRenderer.transform.TryGetComponent(out weapon))return;
        weapon.SetWeapon(type);
        Display_ShowUnit();
    }
   
#endregion 联网数据操作
}