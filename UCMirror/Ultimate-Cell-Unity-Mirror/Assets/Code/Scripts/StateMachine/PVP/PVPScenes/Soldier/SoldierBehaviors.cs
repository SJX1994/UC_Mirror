using UnityEngine;
using System.Collections;
using UC_PlayerData;

public class SoldierBehaviors : MonoBehaviour
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
            if(player == Player.NotReady)player = unitBase.unitTemplate.player;
            return player;
        }
    }

    //士气
    public MoraleTemplate morale;
    // 强度
    public float strength = 1f;
    public GameObject strengthBar;
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
    [HideInInspector]
    public FourDirectionsLink fourDirectionsLinks;
    [HideInInspector]
    public ChainTransfer chainTransfer;

    [HideInInspector]
    public ParticleSystem fourDirectionsLinkStartEffect = new(); 
    [HideInInspector]
    public ParticleSystem fourDirectionsLinkEndEffect = new(); 
    [HideInInspector]
    public ParticleSystem positiveEffect = new();
    [HideInInspector]
    public Unit unitBase;
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
        morale.baseminMorale =  morale.minMorale;
        morale.EffectByMorale(this,ref strength);
        // 组件
        weakAssociation = transform.GetComponent<WeakAssociation>();
        fourDirectionsLinks = transform.GetComponent<FourDirectionsLink>();
        chainTransfer = transform.GetComponent<ChainTransfer>();
        // 资产
        fourDirectionsLinkStartEffect = transform.Find("FourDirLinkGoodEffect").GetComponent<ParticleSystem>();
        fourDirectionsLinkEndEffect = transform.Find("FourDirLinkBadEffect").GetComponent<ParticleSystem>();
        positiveEffect = transform.Find("FourDirLinkGoodEffect").GetComponent<ParticleSystem>();
        strengthBar = transform.Find("Spine").Find("Strength").gameObject;
        if(!needRender)
        {
            fourDirectionsLinkStartEffect.GetComponent<Renderer>().enabled = false;
            fourDirectionsLinkEndEffect.GetComponent<Renderer>().enabled = false;
            positiveEffect.GetComponent<Renderer>().enabled = false;
            strengthBar.GetComponent<SpriteRenderer>().enabled = false;
        }
        // Unit
        skinName = "";
        unitBase = TryGetComponent(out UnitSimple unit) ? unit : null;
        unitBase.selectionCircle.gameObject.SetActive(false);
        if(unitBase != null)
        {
            unitBase.OnDie += OnEndCalculate;
            unitBase.OnCollect += OnEndCalculate;
            skinName = unitBase.skeletonRenderer.Skeleton.Skin.Name;
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
            morale.morale = 1.0f;
            strength = 1f;
        }
        
        // 输入测试
        // mechanismInPut = FindObjectOfType<MechanismInPut>();
        // if(!mechanismInPut)return;
        // mechanismInPut.modeChangeAction += ModeChangedAction;
        
    }
    void Update()
    {
        // 士气
        strengthBar.transform.localScale = Vector3.one * strength;
        if(morale.morale<=morale.minMorale)return;
        morale.ReduceMorale(this,decreaseRate*Time.deltaTime , false);
        morale.EffectByMorale(this,ref strength);
    }
    public void Behaviors_OnFullRows()
    {
        morale.AddMorale(this, 0.2f, true);
        morale.EffectByMorale(this,ref strength);
        positiveEffect.Play();
    }
    public void Behaviors_ChainTransfer()
    {
        if(transform.GetComponent<UnitSimple>().unitTemplate.player == UC_PlayerData.Player.NotReady)return;
        if (!transform.TryGetComponent<ChainTransfer>(out ChainTransfer tempChainTransfer))return;
        FirstChainTransfer();
        if(!(tempChainTransfer.bombIsMoving == false && tempChainTransfer.CanDoChain()))return;
        tempChainTransfer.FirstChain(false);
    }
#endregion 数据关系
#region 数据操作
    void FirstChainTransfer()
    {
        morale.AddMorale(this, 1.5f, true);
        morale.EffectByMorale(this,ref strength);
        positiveEffect.Play();
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
    void OnEndCalculate(Unit u)
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
        unitBase.skeletonRenderer.transform.localScale -= Vector3.one * 0.5f;
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
        unitBase.skeletonRenderer.transform.localScale += Vector3.one * 0.5f;
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
}



