using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
using System;
using DG.Tweening;
using System.Linq;
public class TetriUnitSimple : MonoBehaviour
{
    [SerializeField]
    public int tetriUnitIndex;
    public TetriUnitTemplate tetriUnitTemplate;
    public UnitSimple haveUnit;
    UnitSimple loadUnit;
    TetriBlockSimple tetriBlock;
    public TetrisBlockSimple TetrisBlockSimple
    {
        get
        {
            if(!tetriBlock)tetriBlock = GetComponent<TetriBlockSimple>();
            return tetriBlock.tetrisBlockSimple;
        }
    }
    TetriBuoySimple tetriBuoy;
    public TetriBuoySimple tetriTemp;
    public TetrisUnitSimple tetrisUnitSimple;
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
    void Start()
    {
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
        unit.skeletonRenderer.Skeleton.SetSkin(unit.skinName);
        unit.idV2 = tetriBlock.PosId;
        UnitSimple unitSimple = unit as UnitSimple;
        unitSimple.tetriUnitSimple = this;
        unitSimple.durationRunning = tetriBlock.tetrisBlockSimple.occupyingTime;
        unitSimple.ResetRotation();
        unitSimple.SetFlip();
        indexPairColor = new KeyValuePair<int, UnitData.Color>(tetriUnitIndex,type);
        // 事件监听 Unit
        unit.OnDie += UnitDie;
        // 事件监听 Tetri
        tetriBlock.tetrisBlockSimple.OnRotate += unitSimple.OnRotate;
        tetriBlock.tetrisBlockSimple.OnStartMove += unitSimple.Display_StartRunning;
        tetriBlock.tetrisBlockSimple.OnTetrisMoveing += unitSimple.OnTetrisMoveing;
        tetriBlock.TetriPosIdChanged += unitSimple.OnTetriPosIdChanged;
        tetriBuoy.OnTetriTempChange += GetTetriTemp;
        // 武器
        Weapon weapon;
        if (!unit.skeletonRenderer.transform.TryGetComponent(out weapon))return null;
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
        unit.skeletonRenderer.Skeleton.SetSkin(unit.skinName);
        unit.idV2 = tetriBlock.PosId;
        UnitSimple unitSimple = unit as UnitSimple;
        unitSimple.durationRunning = tetriBlock.tetrisBlockSimple.occupyingTime;
        unitSimple.ResetRotation();
        unitSimple.SetFlip();
        indexPairColor = new KeyValuePair<int, UnitData.Color>(tetriUnitIndex,type);
        // 事件监听 Unit
        unit.OnDie += UnitDie;
        // 事件监听 Tetri
        tetriBlock.tetrisBlockSimple.OnRotate += unitSimple.OnRotate;
        tetriBlock.tetrisBlockSimple.OnStartMove += unitSimple.Display_StartRunning;
        tetriBlock.tetrisBlockSimple.OnTetrisMoveing += unitSimple.OnTetrisMoveing;
        tetriBlock.TetriPosIdChanged += unitSimple.OnTetriPosIdChanged;
        tetriBuoy.OnTetriTempChange += GetTetriTemp;
        // 武器
        Weapon weapon;
        if (!unit.skeletonRenderer.transform.TryGetComponent(out weapon))return null;
        weapon.SetWeapon(type);
        tetrisUnitSimple.InitProcess++;
        return unit;
    }
    void GetTetriTemp(TetriBuoySimple temp)
    {
        if(temp==null)return;
        tetriTemp = temp;
    }
    void UnitDie(Unit whoDie)
    {
        UnitSimple unitSimple = whoDie as UnitSimple;
        // 表现
        ParticleSystem dieParticle = unitSimple.Soldier.Effect.effectCollection.Display_setSkine(unitSimple.Soldier);
        dieParticle = Instantiate(dieParticle);
        dieParticle.transform.position = unitSimple.transform.position;
        dieParticle.Play();
        Destroy(dieParticle.gameObject, 1.0f);
        // 恢复砖块状态
        tetriBlock.canCreate = false;
        tetriBlock.CancleOccupied();
        // 取消后续事件检测
        tetrisUnitSimple.tetriUnits.Remove(this);
        tetrisUnitSimple.GetComponent<TetrisBlockSimple>().childTetris.Remove(this.GetComponent<TetriBlockSimple>());
        tetrisUnitSimple.GetComponent<TetrisBuoySimple>().childTetris.Remove(this.GetComponent<TetriBuoySimple>());
        tetriBlock.tetrisBlockSimple.pioneerTetris.Remove(this.GetComponent<TetriBlockSimple>());
        tetriBlock.tetrisBlockSimple.EvaluatePioneers_X();
        // 取消事件监听
        tetriBuoy.OnTetriTempChange -= GetTetriTemp;
        tetriBlock.tetrisBlockSimple.OnRotate -= unitSimple.OnRotate;
        tetriBlock.tetrisBlockSimple.OnStartMove -= unitSimple.Display_StartRunning;
        tetriBlock.tetrisBlockSimple.OnTetrisMoveing -= unitSimple.OnTetrisMoveing;
        tetriBlock.tetrisBlockSimple.childTetris.Remove(this.GetComponent<TetriBlockSimple>());
        tetriBlock.tetrisBlockSimple.sequence?.Kill();
        tetriBlock.Reset_OnDie();
        tetriBuoy.Reset();
        Destroy(gameObject,1.0f);
    }
    public void FailToCreat()
    {
        if(!haveUnit)return;
        haveUnit.ResetRotation();
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
                    
                    moveDirectionCatch = block.BlockMoveDirection.BlockPairTetri.Value.moveDirection;
                }
                block.BlockMoveDirection.Collect();
            break;
        }   
        return block.propsState;
    }
}