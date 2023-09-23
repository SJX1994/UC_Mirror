using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Fruit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityAction<Fruit,bool> OnFruitHover;
    public FruitDataStructureTemplate fruitDataStructureTemplate;
    [HideInInspector]
    public float delayTime;
    FruitsManager fruitsManager;
    Image fruitImage;
    private bool isMouseHovering;
    StateMachineManager stateMachineManager;
    ApplyDifferenceAttribute attr;

    // Start is called before the first frame update
    void Start()
    {
        fruitsManager = FindObjectOfType<FruitsManager>();
        stateMachineManager = FindObjectOfType<StateMachineManager>();
        fruitImage = GetComponent<Image>();
        fruitsManager.OnFruitsLoaded += UseSkill;
        stateMachineManager.OnSyntheticEvent += OnSyntheticFired;
        stateMachineManager.OnUnitCreat += OnUnitCreat;
        stateMachineManager.OnUnitDied += OnUnitDied;
        stateMachineManager.OnUnitDying += OnUnitDying;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseHovering = true;
        OnFruitHover?.Invoke(this,isMouseHovering);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseHovering = false;
        OnFruitHover?.Invoke(this,isMouseHovering);
    }

    void UseSkill()
    {
        if(fruitDataStructureTemplate)
        {
            fruitImage.sprite = fruitDataStructureTemplate.fruitIcon;
        }else
        {
            fruitImage.enabled = false;
            return;
        }
        

    }
    /// <summary>
    /// 单位濒死触发
    /// </summary>
    void OnUnitDying(SoldierBehaviors soldierDying,int lastPower)
    {
        attr = null;
        if(!fruitDataStructureTemplate)return;
        if(!soldierDying)return;
        if(soldierDying.transform.TryGetComponent(out attr))
        {
            if(!attr.fruitDatas.Contains(fruitDataStructureTemplate))
            {
                attr.FruitIn(fruitDataStructureTemplate);
            }
            attr.FruitDoDying(fruitDataStructureTemplate,lastPower);
        };
    }
    /// <summary>
    /// 单位死亡触发
    /// </summary>
    void OnUnitDied(SoldierBehaviors soldierDied)
    {
        attr = null;
        if(!fruitDataStructureTemplate)return;
        if(!soldierDied)return;
        if(soldierDied.transform.TryGetComponent(out attr))
        {
            if(!attr.fruitDatas.Contains(fruitDataStructureTemplate))
            {
                attr.FruitIn(fruitDataStructureTemplate);
            }
            attr.FruitDoDie(fruitDataStructureTemplate);
        };
    }
    /// <summary>
    /// 单位创建触发
    /// </summary>
    /// <param name="soldier">创建的单位</param> 
    void OnUnitCreat(SoldierBehaviors soldier)
    {
        attr = null;
        if(!fruitDataStructureTemplate)return;
        if(!soldier)return;
        if(soldier.transform.TryGetComponent(out attr))
        {
            Invoke(nameof(OnUnitCreatDelay), delayTime);
        };
    }
    void OnUnitCreatDelay()
    {
        if(!attr)return;
        attr.FruitIn(fruitDataStructureTemplate);
    }
    /// <summary>
    /// 合成触发
    /// </summary>
    /// <param name="soldier">合成出的单位</param>
    void OnSyntheticFired(SoldierBehaviors soldier)
    {
        attr = null;
        if(!fruitDataStructureTemplate)return;
        if(!soldier)return;
        if((fruitDataStructureTemplate.specificTriggerRules & FruitDataStructureTemplate.SpecificTriggerRules.Synthesis)==0)return;
       
        if(soldier.transform.TryGetComponent(out attr))
        {
            Invoke(nameof(OnSyntheticFiredDelay), delayTime);
        };
    }
    void OnSyntheticFiredDelay()
    {
        attr.FruitIn(fruitDataStructureTemplate);
    }
    
}
