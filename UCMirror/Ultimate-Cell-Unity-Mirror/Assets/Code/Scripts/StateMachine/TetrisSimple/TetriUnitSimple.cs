using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
using System;
using DG.Tweening;
public class TetriUnitSimple : MonoBehaviour
{
    [SerializeField]
    public int tetriUnitIndex;
    public TetriUnitTemplate tetriUnitTemplate;
    public UnitSimple haveUnit;
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
    public KeyValuePair<int, UnitData.Color> loadIndexPairColor = new();
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
    void Start()
    {
        dead = false;
        if(!loadUnit){loadUnit = Resources.Load<UnitSimple>("UnitSimple");}
        tetriBlock = GetComponent<TetriBlockSimple>();
        tetriBuoy = GetComponent<TetriBuoySimple>();
        tetrisUnitSimple = transform.parent.GetComponent<TetrisUnitSimple>();
        if(!newTetriUnit)return;
        haveUnit = CreatNewCellUnit() as UnitSimple;
    }
    void Init()
    {
        if(!haveUnit)return;
        haveUnit.Awake();
        haveUnit.Start();
        Start();
    }
    public Unit CreatNewCellUnit()
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
        unitSimple.tetriUnitSimple = this;
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
    }
    public Unit LoadUnit(KeyValuePair<int, UnitData.Color> loadIndexPairColor)
    {
        // 初始化
        if(!tetriBlock || !tetriBuoy){Init();}
        Unit unit = haveUnit;
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
    }
    public void LevelUp(int level)
    {
        haveUnit.LevelUpDisplay(level);
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
        if(!TetriBuoy.blockBuoyHandler)return;
        BlockDisplay blockDisplay = TetriBuoy.blockBuoyHandler.BlockDisplay;
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
        if(haveUnit.tween_running!=null)
        {
            haveUnit.tween_running?.Kill();
            haveUnit.tween_running = null;
        }
        if(haveUnit.tween_DieScale!=null)
        {
            haveUnit.tween_DieScale?.Kill();
            haveUnit.tween_DieScale = null;
        }
        if(haveUnit.tween_OnEndDragDisplay!=null)
        {
            haveUnit.tween_OnEndDragDisplay?.Kill();
            haveUnit.tween_OnEndDragDisplay = null;
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
        tetrisUnitSimple.GetComponent<TetrisBlockSimple>().childTetris.Remove(this.GetComponent<TetriBlockSimple>());
        tetrisUnitSimple.GetComponent<TetrisBuoySimple>().ChildTetris.Remove(this.GetComponent<TetriBuoySimple>());
        EvaluatePioneersWhenUnitDie();
        // 取消事件监听
        tetriBuoy.OnTetriTempChange -= Event_GetTetriTemp;
        tetriBlock.tetrisBlockSimple.OnRotate -= unitSimple.Event_OnRotate;
        tetriBlock.tetrisBlockSimple.OnStartMove -= unitSimple.Event_Display_StartRunning;
        tetriBlock.tetrisBlockSimple.OnTetrisMoveing -= unitSimple.Event_OnTetrisMoveing;
        tetriBlock.tetriStuckEvent -= unitSimple.Event_Display_OnTetriStuck;
        tetriBlock.tetrisBlockSimple.childTetris.Remove(this.GetComponent<TetriBlockSimple>());
        tetriBlock.tetrisBlockSimple.sequence?.Kill();
        tetriBlock.Reset_OnDie();
        tetriBuoy.Reset();
        dead = true;
        Destroy(gameObject,timeToDestory);
    }
    public void FailToCreat()
    {
        if(!haveUnit)return;
        haveUnit.ResetRotation();
        haveUnit.EnableSelectEffect = true;
    }
    public void UnitTemp()
    {
        haveUnit.gameObject.SetActive(false);
        this.enabled = false;
    }
    public void RemoveUnit()
    {
        Destroy(haveUnit.gameObject,0.1f);
    }
    public void SetFightTag(bool needFight = true)
    {
        if(!haveUnit)return;
        if(needFight)
        {
            haveUnit.tag = haveUnit.unitTemplate.player.ToString();
        }else
        {
            haveUnit.tag = "Untagged";
        }
        
    }
    public PropsData.PropsState Ray_PorpCollect()
    {
        if(!haveUnit) return PropsData.PropsState.None;
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
        haveUnit.Display_OnBeginDragDisplay();
    }
    public void OnEditingStatusAfterSelection()
    {
        haveUnit.Display_OnEditingStatusAfterSelection();
    }
    public void OnEndDragDisplay()
    {
        haveUnit.Display_OnEndDragDisplay();
    }
    public void Display_UserCommandTheBattle()
    {
        haveUnit.Display_UserCommandTheBattle();
    }
    public void Display_UserWatchingFight()
    {
        haveUnit.Display_UserWatchingFight();
    }
    public void Display_HideUnit()
    {
        haveUnit.Display_HideUnit();
    }
    public void Display_ShowUnit()
    {
        haveUnit.Display_ShowUnit();
    }
    public void SetUnitSortingOrderToFlow()
    {
        haveUnit.SetUnitSortingOrderToFlow();
    }
    public void SetUnitSortingOrderToNormal()
    {
        haveUnit.SetUnitSortingOrderToNormal();
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
}