using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuppetLine : MonoBehaviour
{
# region 数据对象
    GameObject puppetUnitObj;
    private bool onServer = false;
    public bool OnServer { 
          get {return onServer;}
          set {
              if (value != onServer)
              {
                  onServer = value;
              }
          } 
    }
    public PuppetUnit puppetUnit;
    [HideInInspector]
    public Unit baseUnit;
    [HideInInspector]
    public SoldierBehaviors baseSoldier;
    [HideInInspector]
    public MechanismInPut mechanismInPut;
    [HideInInspector]
    public Weapon weapon;
    [HideInInspector]
    public WeakAssociation weakAssociation;
    [HideInInspector]
    public FourDirectionsLink fourDirectionsLinker;
    [HideInInspector]
    public ChainTransfer chainTransfer;
    public int id; // 配对ID
    public UnityAction OnPuppetAttacking; // 攻击事件
    public UnityAction OnPuppetAttackFinish; // 攻击结束事件
    public UnityAction<bool,Vector3> OnPuppetShooting; // 射击事件
    public UnityAction<UnitTemplate.UnitType> OnPuppetSide; // 左右变更事件
    public UnityAction<Unit.UnitState,Vector3> OnPuppetStateChanged; // 状态变化事件
    public UnityAction<UnitTemplate.UnitType> OnPuppetTypeChanged; // 类型变化事件
    public UnityAction<string> OnPuppetSkinChanged; // 皮肤变化事件
    public UnityAction<Vector3> OnPuppetPositionChange; // 位置变化事件
    public UnityAction<Vector3> OnPuppetScaleChanged; // 缩放变化事件
    public UnityAction<float>  OnPuppetDestory; // 可能需要拆成：死亡 收集？
    public UnityAction<bool>  OnPuppetFilp; // 翻转事件
    public UnityAction<float> OnPuppetSpeedChange; // 速度变更事件
    public UnityAction<WeaponTemplate.WeaponType> OnPuppetChangeWeapon; // 武器切换事件
    public UnityAction<Vector3> OnPuppetTargetOfAttackChange; // 攻击目标变更事件
    public UnityAction<float> OnPuppetHealthChange; // 血量更新事件
    // 机制
    public UnityAction<MechanismInPut.ModeTest> OnPuppetMechModeChange; // 机制模式变更事件
    public UnityAction<PuppetEffectDataStruct> OnPuppetPlayEffect; // 链式传递特效
    
    private float puppetHealth;
    public float PuppetHealth
    {
          get { return puppetHealth; }
          set
          {
                if (value != puppetHealth)
                {
                    puppetHealth = value;
                    if(OnPuppetHealthChange != null)
                    {
                        OnPuppetHealthChange(puppetHealth);
                    }
                }
          }
    }
    private Vector3 puppetAttackPosition;
    public Vector3 PuppetAttackPosition
    {
          get { return puppetAttackPosition; }
          set
          {
                if (value != puppetAttackPosition)
                {
                    puppetAttackPosition = value;
                    OnPuppetTargetOfAttackChange(puppetAttackPosition);
                }
          }
    }
    private float puppetSpeed;
    public float PuppetSpeed
    {
          get { return puppetSpeed; }
          set
          {
                if (value != puppetSpeed)
                {
                    puppetSpeed = value;

                    if(OnPuppetSpeedChange != null)
                    {
                        OnPuppetSpeedChange(puppetSpeed);
                    }
                    
                }
          }
    }
    private Unit.UnitState puppetState;
    public Unit.UnitState PuppetState
    {
          get { return puppetState; }
          set
          {
                if (value != puppetState)
                {
                    puppetState = value;
                    OnUnitStateChanged(puppetState);
                }
          }
    }
    private Vector3 puppetPosition;
    public Vector3 PuppetPosition
    {
          get { return puppetPosition; }
          set
          {
                if (value != puppetPosition)
                {
                    puppetPosition = value;
                    if(OnPuppetPositionChange != null)
                    {
                        OnPuppetPositionChange(PuppetPosition);
                    }
                }
          }
    }
    private Vector3 puppetScale;
    public Vector3 PuppetScale
    {
          get { return puppetScale; }
          set
          {
                if (value != puppetScale)
                {
                    puppetScale = value;
                    if(OnPuppetScaleChanged != null)
                    {
                        OnPuppetScaleChanged(PuppetScale);
                    }
                }
          }
    }
    private bool puppetFilp;
    public bool PuppetFilp
    {
          get { return puppetFilp; }
          set
          {
                if (value != puppetFilp)
                {
                    puppetFilp = value;
                    OnFilp(puppetFilp);
                }
          }
    }
# endregion 数据对象
# region 数据关系
    // Start is called before the first frame update
    void Start()
    {
        baseUnit =  transform.GetComponent<Unit>();
        baseSoldier = transform.GetComponent<SoldierBehaviors>();
        weakAssociation = transform.GetComponent<WeakAssociation>();
        fourDirectionsLinker = transform.GetComponent<FourDirectionsLink>();
        chainTransfer = transform.GetComponent<ChainTransfer>();
        chainTransfer.NeedRender = true;
        weakAssociation.NeedRender = true;
        fourDirectionsLinker.NeedRender = true;
        baseSoldier.NeedRender = true;

        if(!puppetUnit)return; // 是否启用Puppet模式
        
        weapon = baseUnit.weapon;
        mechanismInPut = baseSoldier.mechanismInPut;
        id = baseUnit.id;
        
        puppetUnitObj = Instantiate(puppetUnit.gameObject);
        puppetUnit = puppetUnitObj.GetComponent<PuppetUnit>();
        puppetUnit.puppetLine = this;
        PuppetFilp = false;
        baseUnit.OnDie += OnDie;
        baseUnit.OnCollect += OnCollect;
        baseUnit.OnInitFinish += OnBaseUnitInitFinish;
        baseUnit.OnAttacking += ()=>{ OnPuppetAttacking?.Invoke(); };
        baseUnit.OnAttackFinish += ()=>{ OnPuppetAttackFinish?.Invoke(); };
        mechanismInPut.modeChangeAction += (MechanismInPut.ModeTest mode) => {
            OnPuppetMechModeChange?.Invoke(mode);
        };
        // chainTransfer.OnPlayEffect += (PuppetEffectDataStruct effectData) => {
        //     OnPuppetPlayEffect?.Invoke(effectData);
        // };
        fourDirectionsLinker.OnPlayEffect += (PuppetEffectDataStruct effectData) => {
            OnPuppetPlayEffect?.Invoke(effectData);
        };
        weakAssociation.OnPlayEffect += (PuppetEffectDataStruct effectData) => {
            OnPuppetPlayEffect?.Invoke(effectData);
        };

        Invoke(nameof(LateStart), 0.1f);
        
    }
    void LateStart()
    {
        // 鼠标事件接收和碰撞
        baseUnit.transform.GetComponent<CapsuleCollider>().enabled = onServer; // 关闭原有自身的鼠标事件监听
        
        // 关闭所有渲染相关组件--
       
        if(weapon.shooter)
        {
            weapon.shooter.enabled = false;
        }

        baseUnit.SkeletonRenderer.transform.GetComponent<MeshRenderer>().enabled = false;
        foreach(var child in baseUnit.SkeletonRenderer.transform.GetComponentsInChildren<SpriteRenderer>())
        {
            child.enabled = false;
        }
        foreach(var child in baseUnit.transform.GetComponentsInChildren<ParticleSystem>())
        {
            child.GetComponent<Renderer>().enabled = false;
        }
        chainTransfer.NeedRender = false;
        weakAssociation.NeedRender = false;
        fourDirectionsLinker.NeedRender = false;
        baseSoldier.NeedRender = false;
        // 关闭所有渲染相关组件--

        weapon.OnUnitShooting += OnUnitShooting;
        puppetUnit.puppetMouseState.OnMouseEnterPuppet += OnPuppetMouseEnter;
        puppetUnit.puppetMouseState.OnMouseExitPuppet += OnPuppetMouseExit;
        puppetUnit.puppetMouseState.OnMouseUpPuppet += OnPuppetMouseUp;
    }

      

      // Update is called once per frame
    void Update()
    {
        if(!puppetUnit)return;
        
        if(baseUnit!=null && baseSoldier!=null && baseUnit.navMeshAgent)
        {
            // 位置同步
            PuppetPosition = baseUnit.SkeletonRenderer.transform.position;
            // 缩放同步
            PuppetScale = baseUnit.SkeletonRenderer.transform.localScale;
            // 血量同步
            PuppetHealth = baseUnit.shaderHP;
            // 翻转同步
            if (baseUnit.navMeshAgent.velocity.x != 0) {
				PuppetFilp = baseUnit.navMeshAgent.velocity.x <= 0;
			}
            // 动画同步
            PuppetSpeed = baseUnit.navMeshAgent.velocity.magnitude;
            if(baseUnit.targetOfAttack!=null && baseUnit.targetOfAttack.transform!=null)
            {
                PuppetAttackPosition = baseUnit.targetOfAttack.transform.position;
            }
            
            
        }
        
    }
# endregion 数据关系
# region 数据操作
   
    private void OnUnitShooting(bool isShooting, Vector3 targetPosition)
    {
        OnPuppetShooting?.Invoke(isShooting,targetPosition);
    }
    private void OnPuppetMouseUp(int ID,GameObject whatHit)
    {
        if(baseUnit.unitTemplate.unitType == UnitTemplate.UnitType.Virus)return;
        switch(baseSoldier.modeTest)
        {
            case MechanismInPut.ModeTest.Morale:
                
            break;
            case MechanismInPut.ModeTest.FourDirectionsLinks:
                
            break;
            case MechanismInPut.ModeTest.WeakAssociation:

            break;
            case MechanismInPut.ModeTest.ChainTransfer:
                // 判断相交的物体是否是你所关心的物体
                baseSoldier.OnMouseUpChainTransfer(whatHit);
            break;
        }
        
    }
    private void OnPuppetMouseEnter(int ID)
    {
        baseSoldier.OnMouseEnterDo();
    }
    private void OnPuppetMouseExit(int ID)
    {
        baseSoldier.OnMouseExitDo();
    }
   
    
    private void OnBaseUnitInitFinish(Unit unit)
    {
        OnPuppetChangeWeapon?.Invoke(unit.weaponType);
        OnPuppetSide?.Invoke(unit.unitTemplate.unitType);
        OnPuppetSkinChanged?.Invoke(unit.skinName);
        OnPuppetTypeChanged?.Invoke(unit.unitTemplate.unitType);
    }
    private void OnUnitStateChanged(Unit.UnitState state)
    {
        OnPuppetStateChanged?.Invoke(state, baseUnit.targetPos);
    }   
    private void OnFilp(bool filp)
    {
        OnPuppetFilp?.Invoke(filp);
    }
    private void OnCollect(Unit unit)
    {
        float dieDelay = unit.unitTemplate.destoryTime;
        OnPuppetDestory?.Invoke(dieDelay);
        unit.OnCollect -= this.OnCollect;
    }
    private void OnDie(Unit unit)
    {
        float dieDelay = unit.unitTemplate.destoryTime;
        OnPuppetDestory?.Invoke(dieDelay);
        unit.OnDie -= this.OnDie;
    }
# endregion 数据操作
}
