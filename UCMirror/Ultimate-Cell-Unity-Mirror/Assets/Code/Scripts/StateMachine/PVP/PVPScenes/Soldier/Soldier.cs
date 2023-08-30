using UnityEngine;
using System.Collections;

public class Soldier : MonoBehaviour
{
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
    [HideInInspector]
    public string skinName = "";
    // 输入测试
    [HideInInspector]
    public MechanismInPut.ModeTest modeTest;
    [HideInInspector]
    public MechanismInPut mechanismInPut;
    Vector3 mousePosition;
    Vector3 offset;
    void Start()
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
        // 输入测试
        mechanismInPut = FindObjectOfType<MechanismInPut>();
        mechanismInPut.modeChangeAction += ModeChangedAction;
        // Unit
        skinName = "";
        unitBase = TryGetComponent(out Unit unit) ? unit : null;
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
        
    }
    void Update()
    {
        
        
        // 士气测试
        strengthBar.transform.localScale = Vector3.one * strength;
        if(morale.morale>morale.minMorale)
        {
            morale.ReduceMorale(this,decreaseRate*Time.deltaTime , false);
            morale.EffectByMorale(this,ref strength);
        }
        
    }
    /// <summary>
    /// 获取士兵的兵种
    /// </summary>
    /// <returns></returns>
    public MoraleTemplate.SoldierType SoldierType()
    {
        return morale.soldierType;
    }
  
    // 暂时的激活事件
    void ModeChangedAction(MechanismInPut.ModeTest modeTest)
    {
        this.modeTest = modeTest;
        switch(modeTest)
        {
            case MechanismInPut.ModeTest.Morale:
                
            break;
            case MechanismInPut.ModeTest.FourDirectionsLinks:
                
            break;
        }
    }
    
    private void OnMouseDown()
    {
        if(!unitBase)return;
        if(unitBase.unitTemplate.unitType == UnitTemplate.UnitType.Virus)return;
        switch(modeTest)
        {
            case MechanismInPut.ModeTest.Morale:
                // morale.AddMorale(this, 0.2f, true);
                // morale.EffectByMorale(this,ref strength);
            break;
            case MechanismInPut.ModeTest.FourDirectionsLinks:
                // StartCoroutine(DragAble());
            break;
            case MechanismInPut.ModeTest.WeakAssociation:
                StartCoroutine(DragAble());
            break;
            case MechanismInPut.ModeTest.ChainTransfer:
              
               
            break;
        }
        
    }
    private void OnMouseExit()
    {
        OnMouseExitDo();
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
        OnMouseEnterDo();
    }
    public void OnMouseEnterDo()
    {
        if(!unitBase)return;
        unitBase.selectionCircle.gameObject.SetActive(true);
        unitBase.skeletonRenderer.transform.localScale += Vector3.one * 0.5f;
    }
    private void OnMouseDrag()
    {
        if(!unitBase)return;
        if(unitBase.unitTemplate.unitType == UnitTemplate.UnitType.Virus)return;
        switch(modeTest)
        {
            case MechanismInPut.ModeTest.Morale:
            break;
            case MechanismInPut.ModeTest.FourDirectionsLinks:
            break;
            case MechanismInPut.ModeTest.WeakAssociation:
            break;
            case MechanismInPut.ModeTest.ChainTransfer:

                // // 在鼠标点击位置创建一条射线
                // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                
                // RaycastHit hit;
                // ChainTransfer tempChain; // 连射

                // if (Physics.Raycast(ray, out hit)) // 执行射线投射并检测相交
                // {
                    
                //     // 连射
                //         // 判断相交的物体是否是你所关心的物体
                //         if (hit.collider.gameObject.GetComponent<ChainTransfer>() == true)
                //         {
                //             tempChain = hit.collider.gameObject.GetComponent<ChainTransfer>();
                            
                //             if(tempChain.bombIsMoving == false && tempChain.CanDoChain())
                //             {
                //                 tempChain.FirstChain(true);
                //             }
                //         }
                // }
                
            break;
        }
    }
    private void OnMouseUp()
    {
        if(!unitBase)return;
        if(unitBase.unitTemplate.unitType == UnitTemplate.UnitType.Virus)return;
        switch(modeTest)
        {
            case MechanismInPut.ModeTest.Morale:
                
            break;
            case MechanismInPut.ModeTest.FourDirectionsLinks:
                
            break;
            case MechanismInPut.ModeTest.WeakAssociation:

            break;
            case MechanismInPut.ModeTest.ChainTransfer:
            // 自身入口 检测其他
                if (Input.GetMouseButtonUp(0)) // 检测左键抬起事件
                {   
                    
                    // 在鼠标点击位置创建一条射线
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit)) // 执行射线投射并检测相交
                    {
                        // 判断相交的物体是否是你所关心的物体
                        OnMouseUpChainTransfer(hit.collider.gameObject);
                        // ChainTransfer tempChainTransfer;
                        // if (hit.collider.gameObject.TryGetComponent<ChainTransfer>(out tempChainTransfer))
                        // {
                        //     // 鼠标抬起事件在物体的碰撞检测上
                        //     if(tempChainTransfer.bombIsMoving == false && tempChainTransfer.CanDoChain())
                        //     {
                        //         tempChainTransfer.FirstChain(false);
                        //     }
                        // }

                    }
                }
            // 自身检测
                // if (Input.GetMouseButtonUp(0)) // 检测左键抬起事件
                // {   
                //     Debug.LogWarning("OnMouseUp"+ Input.mousePosition);
                //     // 在鼠标点击位置创建一条射线
                //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    
                //     RaycastHit hit;
                //     if (Physics.Raycast(ray, out hit)) // 执行射线投射并检测相交
                //     {
                //         // 判断相交的物体是否是你所关心的物体
                //         if (hit.collider.gameObject == transform.gameObject)
                //         {
                //             // 鼠标抬起事件在物体的碰撞检测上
                //             if(chainTransfer.bombIsMoving == false && chainTransfer.CanDoChain())
                //             {
                //                 chainTransfer.FirstChain(false);
                //             }
                //         }
                       
                //     }
                // }
               
            break;
        }
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
    void OnEndCalculate(Unit u)
    {
        this.enabled = false;
    }
}



