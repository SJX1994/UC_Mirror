using UnityEngine;
using System.Collections;
using UC_PlayerData;
using Mirror;
public class SoldierBehaviors : NetworkBehaviour
{
#region 数据对象
    private bool needRender;
    public bool NeedRender { 
            get {return needRender;}
            set {
                if (value != needRender)
                {
                    needRender = value;
                }
            } 
    }
    // 玩家信息
    private Player player = Player.NotReady;
    public Player Player
    {
        get
        {
            if(player == Player.NotReady)
            {
                player = UnitBase.unitTemplate.player;
                morale.player = player;
            }
            return player;
        }
    }

    //士气
    public MoraleTemplate morale;
    // 强度
    public float strength = 1f;
    public float Strength
    {
        get
        {
            return strength;
        }
        set
        {
            if(value == strength)return;
            strength = value;
            // Debug.Log(unitBase.transform.name +  "+++ StrengthChanging +++" + strength);
        }
    }
    public GameObject strengthBar;
    public GameObject StrengthBar
    {
        get
        {
            if(!strengthBar)strengthBar = transform.Find("Spine").Find("Strength").gameObject;
            return strengthBar;
        }
    }
    [HideInInspector]
    public float maxStrength = 2f;
    [HideInInspector]
    public float minStrength = 0.6f;
    float decreaseRate = 0.1f; // 恢复速率
    // 表现
    public Material positiveMaterial;
    // 关系组件
    private Effect effect;
    public Effect Effect
    {
        get
        {
            if(effect)return effect;
            effect = TryGetComponent(out Effect e) ? e : null;
            return effect;
        }
    }
    [HideInInspector]
    public WeakAssociation weakAssociation;
    public WeakAssociation WeakAssociation
    {
        get
        {
            if(!weakAssociation)weakAssociation = TryGetComponent(out WeakAssociation wa) ? wa : null;
            return weakAssociation;
        }
    }
    [HideInInspector]
    public FourDirectionsLink fourDirectionsLinks;
    public FourDirectionsLink FourDirectionsLinks{
        get
        {
            if(!fourDirectionsLinks)fourDirectionsLinks = TryGetComponent(out FourDirectionsLink fdl) ? fdl : null;
            return fourDirectionsLinks;
        }
    }
    [HideInInspector]
    public ChainTransfer chainTransfer;
    public ChainTransfer ChainTransfer
    {
        get
        {
            if(!chainTransfer)chainTransfer = TryGetComponent(out ChainTransfer ct) ? ct : null;
            return chainTransfer;
        }
    }
    [HideInInspector]
    public ParticleSystem negativeEffect = new(); 
    public ParticleSystem NegativeEffect
    {
        get
        {
            if(!negativeEffect)negativeEffect=transform.Find("FourDirLinkBadEffect").GetComponent<ParticleSystem>();
            return negativeEffect;
        }
    }
    [HideInInspector]
    public ParticleSystem positiveEffect = new();
    public ParticleSystem PositiveEffect
    {
        get
        {
            if(!positiveEffect)positiveEffect=transform.Find("FourDirLinkGoodEffect").GetComponent<ParticleSystem>();
            return positiveEffect;
        }
    }
    [HideInInspector]
    ParticleSystem levelUpEffect = new();
    public ParticleSystem LevelUpEffect
    {
        get
        {
            if(!levelUpEffect)levelUpEffect=transform.Find("LevelUpEffect").GetComponent<ParticleSystem>();
            return levelUpEffect;
        }
    }
    [HideInInspector]
    public Unit unitBase;
    public Unit UnitBase
    {
        get
        {
            if(!unitBase)unitBase = TryGetComponent(out UnitSimple unit) ? unit : null;
            return unitBase;
        }
    }
    private UnitSimple unitSimple;
    public UnitSimple UnitSimple
    {
        get
        {
            if(unitSimple)return unitSimple;
            unitSimple = TryGetComponent(out UnitSimple unit) ? unit : null;
            return unitSimple;
        }
    }
    private TetriMechanism tetriMechanism;
    public TetriMechanism TetriMechanism
    {
        get
        {
            if(tetriMechanism)return tetriMechanism;
            tetriMechanism = UnitSimple.tetriUnitSimple.TryGetComponent(out TetriMechanism tm) ? tm : null;
            return tetriMechanism;
        }
    }
    [HideInInspector]
    public string skinName = "";
    // 输入测试
    [HideInInspector]
    public MechanismInPut.ModeTest modeTest;
    [HideInInspector]
    public MechanismInPut mechanismInPut;
    Vector3 mousePosition;
    Vector3 offset;
#endregion 数据对象
#region 数据关系
    public void Start()
    {
        needRender = true;
        morale.player = Player;
        morale.baseminMorale =  morale.minMorale;
        morale.EffectByMorale(this,ref strength);
        if(!needRender)
        {
            PositiveEffect.GetComponent<Renderer>().enabled = false;
            NegativeEffect.GetComponent<Renderer>().enabled = false;
            PositiveEffect.GetComponent<Renderer>().enabled = false;
            StrengthBar.GetComponent<SpriteRenderer>().enabled = false;
        }
        // Unit
        skinName = "";
        unitBase = TryGetComponent(out UnitSimple unit) ? unit : null;
        unitBase.selectionCircle.gameObject.SetActive(false);
        if(unitBase != null)
        {
            unitBase.OnDie += Event_OnEndCalculate;
            unitBase.OnCollect += Event_OnEndCalculate;
            skinName = unitBase.SkeletonRenderer.Skeleton.Skin.Name;
            switch(skinName)
            {
                case "red":
                    morale = Instantiate(morale);
                    morale.affectedRangeColor = Color.red;
                    morale.soldierType = MoraleTemplate.SoldierType.Red;
                break;
                case "blue":
                    morale = Instantiate(morale);
                    morale.affectedRangeColor = Color.blue;
                    morale.soldierType = MoraleTemplate.SoldierType.Blue;
                break;
                case "green":
                    morale = Instantiate(morale);
                    morale.affectedRangeColor = Color.green;
                    morale.soldierType = MoraleTemplate.SoldierType.Green;
                break;
                case "purple":
                    morale = Instantiate(morale);
                    morale.affectedRangeColor = Color.magenta;
                    morale.soldierType = MoraleTemplate.SoldierType.Purple;
                break;
            }
            morale.Morale = 1.0f;
            strength = 1f;
        }
        
        // 输入测试
        // mechanismInPut = FindObjectOfType<MechanismInPut>();
        // if(!mechanismInPut)return;
        // mechanismInPut.modeChangeAction += ModeChangedAction;
        
    }
    void LateUpdate()
    {
        // 士气
        StrengthBar.transform.localScale = Vector3.one * strength;
        if(morale.Morale<=morale.minMorale)return;
        morale.ReduceMorale(this,decreaseRate*Time.deltaTime , false);
        morale.EffectByMorale(this,ref strength);
    }
    public void Behaviors_onObstacle()
    {
        morale.ReduceMorale(this, 0.5f, true);
        morale.EffectByMorale(this,ref strength);
        NegativeEffect.Play();
    }
    public void Behaviors_OnFullRows()
    {
        morale.AddMorale(this, 1.2f, false);
        morale.EffectByMorale(this,ref strength);
        PositiveEffect.Play();
    }
    public void Behaviors_WeakAssociation()
    {
        morale.AddMorale(this, 1.5f, false);
        morale.EffectByMorale(this,ref strength);
        PositiveEffect.Play();
    }
    public void Behaviors_ChainTransfer()
    {
        if(transform.GetComponent<UnitSimple>().unitTemplate.player == UC_PlayerData.Player.NotReady)return;
        if (!transform.TryGetComponent<ChainTransfer>(out ChainTransfer tempChainTransfer))return;
        FirstChainTransfer();
        tempChainTransfer.AllSoldiers();
        if(!(tempChainTransfer.bombIsMoving == false && tempChainTransfer.CanDoChain()))return;
        tempChainTransfer.FirstChain(false);
    }
    public void Behaviors_onMoveDirectionChanger()
    {
        morale.AddMorale(this, 1.0f, true);
        morale.EffectByMorale(this,ref strength);
        PositiveEffect.Play();
    }
    public void Behaviors_onReachBottomLine()
    {
        morale.AddMorale(this, 10.0f, false);
        morale.EffectByMorale(this,ref strength);
        PositiveEffect.Play();
    }
    public void Behaviors_onLevelUp(int level)
    {
        morale.AddMorale(this, level * 0.5f, false);
        morale.EffectByMorale(this,ref strength);
        LevelUpEffect.Play();
    }
    public void Behaviors_onEditingStatusAfterSelection()
    {
        morale.AddMorale(this, morale.maxMorale, false);
        morale.EffectByMorale(this,ref strength);
    }
    public void Behaviors_OnBeginDragDisplay()
    {
        StrengthBar.gameObject.SetActive(false);
    }
    public void Behaviors_OnEndDragDisplay()
    {
        StrengthBar.gameObject.SetActive(true);
    }
    public void Behaviors_UserCommandTheBattle()
    {
        float commandTheBattleAlpha_FourDirectionsLinks = 0.0f;
        float commandTheBattleAlpha_StrengthBar = 0.0f;
        Color originalStrengthBarColor = StrengthBar.GetComponent<SpriteRenderer>().color;
        StrengthBar.GetComponent<SpriteRenderer>().color = new Color(originalStrengthBarColor.r,originalStrengthBarColor.g,originalStrengthBarColor.b,commandTheBattleAlpha_StrengthBar);
        FourDirectionsLinks.Sprit_North.color = new Color(FourDirectionsLinks.Sprit_North.color.r,FourDirectionsLinks.Sprit_North.color.g,FourDirectionsLinks.Sprit_North.color.b,commandTheBattleAlpha_FourDirectionsLinks);
        FourDirectionsLinks.Sprit_East.color = new Color(FourDirectionsLinks.Sprit_East.color.r,FourDirectionsLinks.Sprit_East.color.g,FourDirectionsLinks.Sprit_East.color.b,commandTheBattleAlpha_FourDirectionsLinks);
        FourDirectionsLinks.Sprit_South.color = new Color(FourDirectionsLinks.Sprit_South.color.r,FourDirectionsLinks.Sprit_South.color.g,FourDirectionsLinks.Sprit_South.color.b,commandTheBattleAlpha_FourDirectionsLinks);
        FourDirectionsLinks.Sprit_West.color = new Color(FourDirectionsLinks.Sprit_West.color.r,FourDirectionsLinks.Sprit_West.color.g,FourDirectionsLinks.Sprit_West.color.b,commandTheBattleAlpha_FourDirectionsLinks);
    }
    public void Behaviors_UserWatchingFight()
    {
        float watchingFightAlpha_FourDirectionsLinks = 0.0f;
        float watchingFightAlpha_StrengthBar = 0.5f;
        Color originalStrengthBarColor = StrengthBar.GetComponent<SpriteRenderer>().color;
        StrengthBar.GetComponent<SpriteRenderer>().color = new Color(originalStrengthBarColor.r,originalStrengthBarColor.g,originalStrengthBarColor.b,watchingFightAlpha_StrengthBar);
        FourDirectionsLinks.Sprit_North.color = new Color(FourDirectionsLinks.Sprit_North.color.r,FourDirectionsLinks.Sprit_North.color.g,FourDirectionsLinks.Sprit_North.color.b,watchingFightAlpha_FourDirectionsLinks);
        FourDirectionsLinks.Sprit_East.color = new Color(FourDirectionsLinks.Sprit_East.color.r,FourDirectionsLinks.Sprit_East.color.g,FourDirectionsLinks.Sprit_East.color.b,watchingFightAlpha_FourDirectionsLinks);
        FourDirectionsLinks.Sprit_South.color = new Color(FourDirectionsLinks.Sprit_South.color.r,FourDirectionsLinks.Sprit_South.color.g,FourDirectionsLinks.Sprit_South.color.b,watchingFightAlpha_FourDirectionsLinks);
        FourDirectionsLinks.Sprit_West.color = new Color(FourDirectionsLinks.Sprit_West.color.r,FourDirectionsLinks.Sprit_West.color.g,FourDirectionsLinks.Sprit_West.color.b,watchingFightAlpha_FourDirectionsLinks);
    }
#endregion 数据关系
#region 数据操作
    void FirstChainTransfer()
    {   
        morale.AddMorale(this, 1.5f, true);
        morale.EffectByMorale(this,ref strength);
        PositiveEffect.Play();
    }
    public void DrawMoraleRange(LineRenderer lineRenderer)
    {
        MoraleTemplate.DrawMoraleRange(transform.position,morale.affectedRange,lineRenderer);
    }
    // 测试用的激活事件
    // void ModeChangedAction(MechanismInPut.ModeTest modeTest)
    // {
    //     this.modeTest = modeTest;
    //     switch(modeTest)
    //     {
    //         case MechanismInPut.ModeTest.Morale:
                
    //         break;
    //         case MechanismInPut.ModeTest.FourDirectionsLinks:
                
    //         break;
    //     }
    // }
    void Event_OnEndCalculate(Unit u)
    {
        this.enabled = false;
    }
   
# region 测试用 需要测试打开 碰撞检测
    private void OnMouseDown()
    {
        // if(!unitBase)return;
        // if(unitBase.unitTemplate.unitType == UnitTemplate.UnitType.Virus)return;
        // switch(modeTest)
        // {
        //     case MechanismInPut.ModeTest.Morale:
        //         // morale.AddMorale(this, 0.2f, true);
        //         // morale.EffectByMorale(this,ref strength);
        //     break;
        //     case MechanismInPut.ModeTest.FourDirectionsLinks:
        //         // StartCoroutine(DragAble());
        //     break;
        //     case MechanismInPut.ModeTest.WeakAssociation:
        //         StartCoroutine(DragAble());
        //     break;
        //     case MechanismInPut.ModeTest.ChainTransfer:
              
               
        //     break;
        // }
        
    }
    private void OnMouseExit()
    {
        // OnMouseExitDo();
    }
    public void OnMouseExitDo()
    {
        if(!unitBase)return;
        if(!unitBase.selectionCircle)return;
        unitBase.selectionCircle.gameObject.SetActive(false);
        unitBase.SkeletonRenderer.transform.localScale -= Vector3.one * 0.5f;
    }
    private void OnMouseEnter()
    {
        // weakAssociation.Active();
        //OnMouseEnterDo();
    }
    public void OnMouseEnterDo()
    {
        if(!unitBase)return;
        // unitBase.selectionCircle.gameObject.SetActive(true);
        unitBase.SkeletonRenderer.transform.localScale += Vector3.one * 0.5f;
    }
    private void OnMouseDrag()
    {
        // if(!unitBase)return;
        // if(unitBase.unitTemplate.unitType == UnitTemplate.UnitType.Virus)return;
        // switch(modeTest)
        // {
        //     case MechanismInPut.ModeTest.Morale:
        //     break;
        //     case MechanismInPut.ModeTest.FourDirectionsLinks:
        //     break;
        //     case MechanismInPut.ModeTest.WeakAssociation:
        //     break;
        //     case MechanismInPut.ModeTest.ChainTransfer:

        //         // // 在鼠标点击位置创建一条射线
        //         // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                
        //         // RaycastHit hit;
        //         // ChainTransfer tempChain; // 连射

        //         // if (Physics.Raycast(ray, out hit)) // 执行射线投射并检测相交
        //         // {
                    
        //         //     // 连射
        //         //         // 判断相交的物体是否是你所关心的物体
        //         //         if (hit.collider.gameObject.GetComponent<ChainTransfer>() == true)
        //         //         {
        //         //             tempChain = hit.collider.gameObject.GetComponent<ChainTransfer>();
                            
        //         //             if(tempChain.bombIsMoving == false && tempChain.CanDoChain())
        //         //             {
        //         //                 tempChain.FirstChain(true);
        //         //             }
        //         //         }
        //         // }
                
        //     break;
        // }
    }
    private void OnMouseUp()
    {
        // if(!unitBase)return;
        // if(unitBase.unitTemplate.unitType == UnitTemplate.UnitType.Virus)return;
        // switch(modeTest)
        // {
        //     case MechanismInPut.ModeTest.Morale:
                
        //     break;
        //     case MechanismInPut.ModeTest.FourDirectionsLinks:
                
        //     break;
        //     case MechanismInPut.ModeTest.WeakAssociation:

        //     break;
        //     case MechanismInPut.ModeTest.ChainTransfer:
        //     // 自身入口 检测其他
        //         if (Input.GetMouseButtonUp(0)) // 检测左键抬起事件
        //         {   
                    
        //             // 在鼠标点击位置创建一条射线
        //             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //             if (Physics.Raycast(ray, out RaycastHit hit)) // 执行射线投射并检测相交
        //             {
        //                 // 判断相交的物体是否是你所关心的物体
        //                 OnMouseUpChainTransfer(hit.collider.gameObject);
        //                 // ChainTransfer tempChainTransfer;
        //                 // if (hit.collider.gameObject.TryGetComponent<ChainTransfer>(out tempChainTransfer))
        //                 // {
        //                 //     // 鼠标抬起事件在物体的碰撞检测上
        //                 //     if(tempChainTransfer.bombIsMoving == false && tempChainTransfer.CanDoChain())
        //                 //     {
        //                 //         tempChainTransfer.FirstChain(false);
        //                 //     }
        //                 // }

        //             }
        //         }
        //     // 自身检测
        //         // if (Input.GetMouseButtonUp(0)) // 检测左键抬起事件
        //         // {   
        //         //     Debug.LogWarning("OnMouseUp"+ Input.mousePosition);
        //         //     // 在鼠标点击位置创建一条射线
        //         //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    
        //         //     RaycastHit hit;
        //         //     if (Physics.Raycast(ray, out hit)) // 执行射线投射并检测相交
        //         //     {
        //         //         // 判断相交的物体是否是你所关心的物体
        //         //         if (hit.collider.gameObject == transform.gameObject)
        //         //         {
        //         //             // 鼠标抬起事件在物体的碰撞检测上
        //         //             if(chainTransfer.bombIsMoving == false && chainTransfer.CanDoChain())
        //         //             {
        //         //                 chainTransfer.FirstChain(false);
        //         //             }
        //         //         }
                       
        //         //     }
        //         // }
               
        //     break;
        // }
    }
    public void OnMouseUpChainTransfer(GameObject whatHit)
    {
        if (whatHit.TryGetComponent<ChainTransfer>(out ChainTransfer tempChainTransfer))
        {
            // 鼠标抬起事件在物体的碰撞检测上
            if (tempChainTransfer.bombIsMoving == false && tempChainTransfer.CanDoChain())
            {
                tempChainTransfer.FirstChain(false);
            }
        }
    }
    IEnumerator DragAble()
    {
        //当前物体对应的屏幕坐标
        mousePosition = Camera.main.WorldToScreenPoint(transform.position);
        //偏移值=物体的世界坐标，减去转化之后的鼠标世界坐标（z轴的值为物体屏幕坐标的z值）
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3
        (Input.mousePosition.x, Input.mousePosition.y, mousePosition.z));
        //当鼠标左键点击
        while (Input.GetMouseButton(0))
        {
            //当前坐标等于转化鼠标为世界坐标（z轴的值为物体屏幕坐标的z值）+ 偏移量
            Vector3 FinalPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, mousePosition.z)) + offset;
            transform.position = new Vector3(FinalPos.x, transform.position.y, FinalPos.z);
            //等待固定更新
            yield return new WaitForFixedUpdate();
        }
    }
# endregion
# endregion 数据操作
# region 联网数据操作
    public bool Local()
    {
        if(RunModeData.CurrentRunMode == RunMode.Local)return true;
        return false;
    }
# endregion 联网数据操作
}



