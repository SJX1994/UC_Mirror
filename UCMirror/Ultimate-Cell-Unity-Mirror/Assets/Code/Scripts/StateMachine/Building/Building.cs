using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Spine.Unity;
using UC_PlayerData;
public class Building : MonoBehaviour
{
#region 数据对象 
    // 通讯
    public LayerMask blockTargetMask;
    public LayerMask fuctionTargetMask;
    public Vector3 lastPos;
    MechanismInPut mechanismInPut;
    public UnityAction<Building> OnBuildingMoveing;
    public UnityAction<Building,BuildingSlot> OnTheLastStayBuilding;
    public UnityAction<SpriteRenderer,bool> OnBuildingFuctionHit;
    public UnityAction<SpriteRenderer,Building> OnBuildingFuctionExit;
    public PVP_faction.faction faction = PVP_faction.faction.None;
    public BlocksCreator_Main blocksCreator;
    // 表现
    private SpriteRenderer selectionCircle;
    private MaterialPropertyBlock spinePropertyBlock;
    private Renderer spineRenderer;
    private Vector3 rightAngle = new Vector3(0,0,0);
    private Animator animator;
    private  SkeletonRenderer skeletonRenderer;
    private Vector3 mousePosition;
    private Vector3 offset;
    private bool draging;
    public bool Draging
    {
          get { return draging; }
          set
          {
                if (value != draging)
                {
                    draging = value;

                    if(OnBuildingMoveing!=null)
                    {
                        OnBuildingMoveing(this);
                    }
                    
                }
          }
    }
    // 逻辑
    BuildingSlot lastSlot = null;
    BuildingSlot firstSlot = null;
    BuildingSlot currentSlot = null;
    List<SpriteRenderer> selectionCircles = new();
  
    float spacing = 1f;
#endregion 数据对象
#region 数据关系
    void Start()
    {
        
        if(faction!=PVP_faction.faction.None)
        {
            PVP_faction.MyFaction = faction;
        }
        faction = PVP_faction.MyFaction;
        Draging = false;
        var spine = transform.Find("Spine");
        spineRenderer = spine.GetComponent<Renderer>();
        animator = spine.GetComponent<Animator>();
        skeletonRenderer = spine.GetComponent<SkeletonRenderer>();
        selectionCircle = transform.Find("SelectionCircle").GetComponent<SpriteRenderer>();
        blocksCreator = FindObjectOfType<BlocksCreator_Main>();
        //blocksCreator.OnBlocksInitEnd += LateStart;
        rightAngle = new Vector3(80f, 0f, 0f);
        RepaintMat();
        Invoke("LateStart",0.5f);
        
    }
    void LateStart()
    {
       mechanismInPut = FindObjectOfType<MechanismInPut>();
       spacing = 0.5f;
       Ray ray = new Ray(selectionCircle.transform.position, Vector3.down);
       RaycastHit hit;
       if (Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask))
       {
           if(hit.collider.transform.TryGetComponent<BuildingSlot>(out BuildingSlot slot))
           {
               slot.Building = this;
               firstSlot = slot;
               transform.position = new Vector3 (slot.slotPos.x,transform.position.y,slot.slotPos.z);
               if(OnTheLastStayBuilding!=null)
               {
                   OnTheLastStayBuilding(this,slot);
               }
               currentSlot = slot;
           }
       }

    }
    // Update is called once per frame
    void Update()
    {
      // 面向摄像机
	  animator.transform.rotation = Quaternion.Euler(rightAngle);
	  transform.rotation = Quaternion.Euler(Vector3.zero);
      
    }
    private void OnMouseDown()
    {
        // 效果
        skeletonRenderer.transform.localScale += Vector3.one * 0.1f;
        selectionCircle.color = new Color(selectionCircle.color.r,selectionCircle.color.g,selectionCircle.color.b,1.0f);
        selectionCircle.sortingOrder = Dispaly.FlowOrder;
        lastPos = transform.position;
        animator.SetFloat("Speed", 1f);
        // blocksCreator.flowMask.color = new Color(0.0f,0.0f,0.0f,0.3f);
        foreach(BlockDisplay block in blocksCreator.blocks)
        {
            block.InFlow();
        }
        Ray ray = new Ray(selectionCircle.transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask))
        {
            
            // 计算射线上可以创建的物体数量
            float distance = 2.4f;
            int objectCount = (int)(distance / spacing);
            // 每隔一定距离创建物体
            for (int i = 1; i < objectCount; i++)
            {
                 
                // 计算当前位置
                float t = i / (float)objectCount;
                Vector3 position = ray.GetPoint(t * distance);

                // 创建物体
                SpriteRenderer obj = Instantiate(selectionCircle, position, selectionCircle.transform.localRotation,selectionCircle.transform);
                obj.transform.localScale = Vector3.one * 0.5f;
                obj.sortingOrder = Dispaly.FlowOrder;
                // obj.transform.localScale = new Vector3(selectionCircle.transform.localScale.x/i,obj.transform.localScale.y,selectionCircle.transform.localScale.z/i);
                selectionCircles.Add(obj);
            }
            if(hit.collider.transform.TryGetComponent<BuildingSlot>(out BuildingSlot slot))
            {
                slot.Building = this;
                firstSlot = slot;
            }
        }
        Draging = true;
        StartCoroutine(DragAble());
        
    }
    private void OnMouseUp()
    {
        
        // 效果
        foreach(BlockDisplay block in blocksCreator.blocks)
        {
            // block.OutFlow();
        }
        animator.SetFloat("Speed", 0f);
        // blocksCreator.flowMask.color = new Color(0.0f,0.0f,0.0f,0.0f);
        skeletonRenderer.transform.localScale -= Vector3.one * 0.1f;
        selectionCircle.color = new Color(selectionCircle.color.r,selectionCircle.color.g,selectionCircle.color.b,0.5f);
        selectionCircle.sortingOrder = Dispaly.FlowOrder;
        
        foreach (var item in selectionCircles)
        {
            Destroy(item.gameObject);
        }
        selectionCircles.Clear();
        // 在鼠标点击位置创建一条射线
        Ray ray = new Ray(selectionCircle.transform.position, Vector3.down);
        RaycastHit hit;
       
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask)) // 执行射线投射并检测相交
        {   
            
            if(hit.collider.transform.TryGetComponent<BuildingSlot>(out BuildingSlot slot))
            {
                if(OnTheLastStayBuilding!=null)
                {
                    OnTheLastStayBuilding(this,slot);
                }
                currentSlot = slot;
                slot.Building = this;
                firstSlot = slot;
                transform.position = new Vector3 (slot.slotPos.x,transform.position.y,slot.slotPos.z);
            }
        }else if(Physics.Raycast(ray, out hit, Mathf.Infinity, fuctionTargetMask))
        {
            if(hit.collider.transform.TryGetComponent<SpriteRenderer>(out SpriteRenderer button))
            {
               
                if(OnBuildingFuctionExit!=null) 
                {
                    OnBuildingFuctionExit(button,this);                    
                }
                Invoke(nameof(ResetPos),0.1f);
            }
        }else{
        
            // MechanismInPut.Instance.warningSystem.changeWarningTypes = WarningSystem.WarningType.CancelOperation;
            Invoke(nameof(ResetPos),0.1f);
        }
        Draging = false;
    }
    private void OnMouseExit()
    {
        if(!Draging){animator.SetTrigger("InterruptAttack");};
        selectionCircle.color = new Color(selectionCircle.color.r,selectionCircle.color.g,selectionCircle.color.b,0.5f);
    }
    private void OnMouseEnter()
    {
        if(!Draging){animator.SetTrigger("DoAttack");};
        selectionCircle.color = new Color(selectionCircle.color.r,selectionCircle.color.g,selectionCircle.color.b,1.0f);

    }
#endregion 数据关系
#region 数据操作
    void ResetPos()
    {
        transform.position = lastPos;
        if(firstSlot!=null)
        {
            firstSlot.Building = this;
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

            // 创建一条射线
            Ray ray = new Ray(selectionCircle.transform.position, Vector3.down);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, blockTargetMask)) // 执行射线投射并检测相交
            {   
                if(hit.collider.transform.TryGetComponent<BuildingSlot>(out BuildingSlot slot))
                {
                    slot.Building = this;

                    // Debug.Log("Building pos Changeing: " + slot.blockDisplay.posId);

                    BuildingSlot tempslot = slot;

                    if(tempslot != lastSlot)
                    {
                        // 值发生变化
                        if(lastSlot!=null)
                        {
                            lastSlot.Building = null;
                        }
                        
                    }
                    lastSlot = tempslot;
                }
            }else
            {
                lastSlot.Building = null;
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, fuctionTargetMask))
                {
                    if(hit.collider.transform.TryGetComponent<SpriteRenderer>(out SpriteRenderer button))
                    {
                        if(OnBuildingFuctionHit!=null) 
                        {
                            OnBuildingFuctionHit(button,true);
                        }
                    }
                }
                else
                {
                    if(OnBuildingFuctionHit!=null) 
                    {
                        SpriteRenderer tempButton = null;
                        OnBuildingFuctionHit(tempButton,false);
                    }
                }
            }
           
            
            
            
            
            //等待固定更新
            yield return new WaitForFixedUpdate();
        }
    }
    void RepaintMat()
    {
        if (spinePropertyBlock == null)
        {
            spinePropertyBlock = new MaterialPropertyBlock();
        }
        spineRenderer .GetPropertyBlock(spinePropertyBlock);
        spinePropertyBlock.SetFloat("_Porcess", 1f);
        spineRenderer.SetPropertyBlock(spinePropertyBlock);
    }
#endregion 数据操作
}
