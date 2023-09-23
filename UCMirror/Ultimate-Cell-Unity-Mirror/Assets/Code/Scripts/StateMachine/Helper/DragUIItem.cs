using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
public class DragUIItem : 
  MonoBehaviour, 
  IBeginDragHandler, 
  IDragHandler, 
  IEndDragHandler,
  IPointerDownHandler,
  IPointerUpHandler
 
{
#region 数据对象
  [SerializeField]
  public string heroName = "查看UnitHeroTemplate.heroName";
  
  [SerializeField]
  RectTransform UIDragElement;
  [SerializeField]
  Slider soulSlider;


  [SerializeField]
  RectTransform Canvas;
  private Vector2 mOriginalLocalPointerPosition;
  private Vector3 mOriginalPanelLocalPosition;
  private Vector2 mOriginalPosition;
  UnitHeroTemplate uht;
  Image heroImage;
 
  public UnityAction<DragUIItem> OnCancel; // 创建取消事件

  // 创建失败类型：
  [HideInInspector]
  public enum CreateFailType
  {
    PositionError,// 1. 位置不合法
    PositionOccupied,// 2. 位置被占用
    SoulNotEnough,// 3. 灵魂不足
  }
  const string _PositionError = "<b><color=yellow>位置</color></b> 不合法";
  const string _PositionOccupied = "<b><color=yellow>位置</color></b> 被占用";
  const string _SoulNotEnough = "<b><color=lightblue>灵魂</color></b> 不足";

  
  
 

  // 通信类
  private BroadcastClass broadcastClass;
  // 通信管理器
  private CommunicationInteractionManager CommunicationManager;
  // 当前位置的砖块种类
  private BlocksClass BlocksInfo;
  // 表现
  RectTransform rectTransform;
  public Sprite innerActiveImage;
  Sprite InnerDisActiveImage;
  public Sprite outterActiveImage;
  Sprite outterDisActiveImage;
  Image imageInner;
  Image imageOuter;
  bool actived;

#endregion 数据对象
#region 数据关系
  private void Start()
  {
    if (GameObject.Find("LanNetWorkManager") != null)
    {
        Communication();
    }
    
    
    if(!UIDragElement)
    {
      UIDragElement = transform.GetComponent<RectTransform>();
    }
    if(!Canvas)
    {
      Canvas = GameObject.Find("CanvasManager_StayMachine").GetComponent<RectTransform>();
      
    }
    if(!soulSlider)
    {
      soulSlider = GameObject.Find("UI_SoulSlider_Main").GetComponent<Slider>();
    }
    // 灵魂费用显示
    if(Canvas.TryGetComponent<StateMachineManager>(out StateMachineManager sm))
    {
      sm.warningPanel.GetComponentInChildren<Text>().text = "";
      sm.warningPanel.GetComponent<Image>().color = new Color(0,0,0,0.0f);
        foreach(GameObject hero in sm.heroUnits)
        {
          if (hero.TryGetComponent(out IHeroUnit ihero))
          {
             if(heroName == ihero.OnCreating().heroName)
             {
                uht = ihero.OnCreating();
             }
          }
        }
    }
    rectTransform = transform.GetComponent<RectTransform>();
    imageInner = transform.GetComponent<Image>();
    imageOuter = transform.parent.Find("Outer").GetComponent<Image>();
    InnerDisActiveImage = imageInner.sprite;
    outterDisActiveImage = imageOuter.sprite;
    actived = false;
    heroImage = transform.GetComponent<Image>();
    mOriginalPosition = UIDragElement.localPosition;
    InvokeRepeating("StateUpdate",0.1f,0.2f);
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    
    rectTransform.DOScale(new Vector2(2f,2f),0.5f).SetEase(Ease.InOutSine).onComplete = ()=>{
      imageInner.sprite = innerActiveImage;
    };
    
  }

  public void OnPointerUp(PointerEventData eventData)
  {
     imageInner.sprite = InnerDisActiveImage;
     transform.GetComponent<RectTransform>().DOScale(new Vector2(1.5f,1.5f),0.5f);
  }

  public void OnBeginDrag(PointerEventData data)
  {
    // 通讯
    if(CommunicationManager)
    {
      CommunicationManager.BlocksReady(BlocksInfo.TetrisInfo, heroName);
    }
    // ---
    
    mOriginalPanelLocalPosition = UIDragElement.localPosition;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
      Canvas, 
      data.position, 
      data.pressEventCamera, 
      out mOriginalLocalPointerPosition);

        // 英雄生成信息
        if(!broadcastClass)return;
        broadcastClass.OnHeroCreateResponse += OnHeroCreateResponse;
    }

  public void OnDrag(PointerEventData data)
  {
    Vector2 localPointerPosition;
    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
      Canvas, 
      data.position, 
      data.pressEventCamera, 
      out localPointerPosition))
    {
      Vector3 offsetToOriginal =
        localPointerPosition - 
        mOriginalLocalPointerPosition;

      UIDragElement.localPosition = 
        mOriginalPanelLocalPosition + 
        offsetToOriginal;
      
      // 通讯
      if(CommunicationManager)
      {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(
        Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
          Vector3 worldPoint = hit.point;
          CommunicationManager.OnBlocksMove(worldPoint);
        }
      }
    }
    //ClampToArea();
  }
  public void OnEndDrag(PointerEventData eventData)
  {
    StartCoroutine(
      Coroutine_MoveUIElement(      
        UIDragElement,       
        mOriginalPosition,       
        0.5f));

    RaycastHit[] hits;
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    hits = Physics.RaycastAll(ray);
    Vector3 worldPoint;
   
    bool hitBlock = false;
    for (int i = 0; i < hits.Length; i++)
    {
      RaycastHit hit = hits[i];
      if(hit.transform.TryGetComponent<BlockDisplay>(out BlockDisplay block))
      {
        
        // if(!block.isOccupy)
        // {
          // worldPoint = hit.point;
          // CreateObject(worldPoint);
          hitBlock = true;
        // }else
        // {
        //   hitBlock = true;
        //   worldPoint = hit.point;
        //   CreateObjectFail(worldPoint, CreateFailType.PositionOccupied);
        // }
        
        
      }
    }
    if(hits.Length == 0)return;
    if(hits[0].transform)
    {
        worldPoint = hits[0].point;
        if (!hitBlock)
        {
            CreateObjectFail(worldPoint, CreateFailType.PositionError);
        }
        else
        {
            // 通讯
            CommunicationManager.BlocksMoveEnd(0);
        }
    }
}

#endregion 数据关系
#region 数据操作  
  void Communication()
  {
      // 通信获取
        // 暂时获取方式
        CommunicationManager = GameObject.Find("LanNetWorkManager").gameObject.GetComponent<CommunicationInteractionManager>();
        broadcastClass = CommunicationManager.GetComponent<BroadcastClass>();

        // 通讯协议绑定触发方法
        broadcastClass.CreateNewIdea += OnGameObjCreate;

        // 触发生成方块方法
        CommunicationManager.OnCreateNewIdea(0);
  }

  void OnHeroCreateResponse(HeroInfoClass info)
  {
      CreateObject(info.HeroPos, info.IndexId);

    //英雄生成信息
    broadcastClass.OnHeroCreateResponse -= OnHeroCreateResponse;
}
  private void OnGameObjCreate(List<IdeaClass> IdeaClass, EventType.BlocksGrade info)
  {
    BlocksInfo = IdeaClass[0].BlocksInfo;

    broadcastClass.CreateNewIdea -= OnGameObjCreate;
  }
  public void StateUpdate()
  {
     if(soulSlider.value >= uht.price)
     {
        // heroImage.color = Color.white;
        if(!actived)
        {
          heroImage.transform.DOScale(new Vector3(1.5f,1.5f,1.5f),0.5f).SetEase(Ease.OutBounce).onComplete=()=>{
            imageOuter.sprite = outterActiveImage;
            actived = true;
          };
        }
     }else
     {
        // heroImage.color = Color.gray;
        actived = false;
        heroImage.transform.localScale = Vector3.zero;
        imageOuter.sprite = outterDisActiveImage;
     }
  }
  public IEnumerator Coroutine_MoveUIElement(
    RectTransform r, 
    Vector2 targetPosition, 
    float duration = 0.1f)
  {
    float elapsedTime = 0;
    Vector2 startingPos = r.localPosition;
    while (elapsedTime < duration)
    {
      r.localPosition =
        Vector2.Lerp(
          startingPos,
          targetPosition, 
          (elapsedTime / duration));
      elapsedTime += Time.deltaTime;

      yield return new WaitForEndOfFrame();
      
    }
    r.localPosition = targetPosition;
    
    
  }

  // Clamp panel to dragArea
  private void ClampToArea()
  {
    Vector3 pos = UIDragElement.localPosition;

    Vector3 minPosition = 
      Canvas.rect.min - 
      UIDragElement.rect.min;

    Vector3 maxPosition = 
      Canvas.rect.max - 
      UIDragElement.rect.max;

    pos.x = Mathf.Clamp(
      UIDragElement.localPosition.x, 
      minPosition.x, 
      maxPosition.x);

    pos.y = Mathf.Clamp(
      UIDragElement.localPosition.y, 
      minPosition.y, 
      maxPosition.y);

    UIDragElement.localPosition = pos;
  }
  /// <summary>
  /// 在不可创建的范围内，创建英雄失败表现
  /// </summary>
  /// <param name="position"></param>
  public void CreateObjectFail(Vector3 position,CreateFailType type)
  {
    if(Canvas.TryGetComponent<StateMachineManager>(out StateMachineManager sm))
    {
        GameObject obj = Instantiate(
        sm.CreatFail, 
        position, 
        sm.CreatFail.transform.rotation);
        sm.warningPanel.GetComponent<Image>().color = new Color(0,0,0,0.3f);
        switch(type)
        {
          
          case CreateFailType.PositionError:
            sm.warningPanel.GetComponentInChildren<Text>().text = _PositionError;
            break;
          case CreateFailType.PositionOccupied:
            sm.warningPanel.GetComponentInChildren<Text>().text = _PositionOccupied;
            break;
          case CreateFailType.SoulNotEnough:
            sm.warningPanel.GetComponentInChildren<Text>().text = _SoulNotEnough;
            break;
        }
        StartCoroutine(WarningTextOff(sm.warningPanel));
        Destroy(obj,1.0f);
    }
    // 取消事件发送订阅者
    if(OnCancel != null)
		{
			OnCancel(this);
		}
  }
  IEnumerator WarningTextOff(RectTransform warningPanel)
  {
    yield return new WaitForSeconds(1.5f);
    warningPanel.GetComponentInChildren<Text>().text = "";
    warningPanel.GetComponent<Image>().color = new Color(0,0,0,0.0f);
  }
  
  /// <summary>
  /// 在可创建的范围内，创建英雄
  /// </summary>
  /// <param name="position"></param>
  public void CreateObject(Vector3 position, int indexId)
  {
    if (heroName == null)
    {
      Debug.Log("No prefab to instantiate");
      return;
    }

    // GameObject obj = Instantiate(
    //   PrefabToInstantiate, 
    //   position, 
    //   Quaternion.identity);
    // 生成英雄
    if(Canvas.TryGetComponent<StateMachineManager>(out StateMachineManager sm))
    {
      if(soulSlider.value>=uht.price)
      {
        HeroCreateClass heroCreateClass = new HeroCreateClass();
        heroCreateClass.HeroInfoClass = new HeroInfoClass();
        heroCreateClass.HeroInfoClass.HeroName = heroName;
        heroCreateClass.CreatePos = position;
        soulSlider.value -= uht.price;
        sm.OnHeroCreate(heroCreateClass, indexId);
      }else
      {
        CreateObjectFail(position, CreateFailType.SoulNotEnough);
        return;
      }
      
    }
  }

      
      #endregion 数据操作
}