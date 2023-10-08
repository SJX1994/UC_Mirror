using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ApplyDifferenceAttribute : MonoBehaviour
{
# region 数据对象
    public bool testMode;
    public DifferenceAttributeTemplate differenceAttributeTemplate_Red;
    public DifferenceAttributeTemplate differenceAttributeTemplate_Green;
    public DifferenceAttributeTemplate differenceAttributeTemplate_Blue;
    public DifferenceAttributeTemplate differenceAttributeTemplate_Purple;
    WarningSystem warningSystem;
    // unit 组件
    Unit baseUnit;
    string baseUnit_skinName;
    bool filpSkeleton;
    // 果实
    FruitDataStructureTemplate fruitData = null;
    [HideInInspector]
    public List<FruitDataStructureTemplate> fruitDatas = new();
    string TextToDispaly;
# endregion 数据对象
# region 数据关系
    void Start()
    {
        if(testMode)return;
        baseUnit = GetComponent<Unit>();
        warningSystem = FindObjectOfType<WarningSystem>();
        TextToDispaly = "";
        baseUnit.OnInitFinish += OnUnitInitFinish;
        // 残躯化林
        Tree = null;
        TreeCutCount = 5;
    }
    void OnUnitInitFinish(Unit u)
    {
        baseUnit_skinName = u.skinName;
        switch(baseUnit_skinName)
        {
            case "red":
                if(!differenceAttributeTemplate_Red)break;
                differenceAttributeTemplate_Red = Instantiate(differenceAttributeTemplate_Red);
                ApplyDifference(differenceAttributeTemplate_Red);
                break;
            case "green":
                if(!differenceAttributeTemplate_Green)break;
                differenceAttributeTemplate_Green = Instantiate(differenceAttributeTemplate_Green);
                ApplyDifference(differenceAttributeTemplate_Green);
                break;
            case "blue":
                if(!differenceAttributeTemplate_Blue)break;
                differenceAttributeTemplate_Blue = Instantiate(differenceAttributeTemplate_Blue);
                ApplyDifference(differenceAttributeTemplate_Blue);
                break;
            case "purple":
                if(!differenceAttributeTemplate_Purple)break;
                differenceAttributeTemplate_Purple = Instantiate(differenceAttributeTemplate_Purple);
                ApplyDifference(differenceAttributeTemplate_Purple);
                break;
        }

        if(fruitDatas.Count==0)return;

        fruitData = null;
        fruitData = fruitDatas.Find((f) => (f.rolePuppetsOptions & WhichRole())!=0);
        if(!fruitData)return;
        if((fruitData.triggerMethodOptions & FruitDataStructureTemplate.TriggerMethod.None)!=0)return;

        bool isRule = (fruitData.triggerMethodOptions & FruitDataStructureTemplate.TriggerMethod.Rule)!=0;
        bool isSkill = (fruitData.triggerMethodOptions & FruitDataStructureTemplate.TriggerMethod.Skill)!=0;
        bool isProbability = (fruitData.triggerMethodOptions & FruitDataStructureTemplate.TriggerMethod.Probability)!=0;
        bool isLevel = (fruitData.triggerMethodOptions & FruitDataStructureTemplate.TriggerMethod.Level)!=0;
        bool isTime = (fruitData.triggerMethodOptions & FruitDataStructureTemplate.TriggerMethod.Time)!=0;
        bool isBlockPos = (fruitData.triggerMethodOptions & FruitDataStructureTemplate.TriggerMethod.BlockPos)!=0;
        bool isObject = (fruitData.triggerMethodOptions & FruitDataStructureTemplate.TriggerMethod.Object)!=0;
        bool isAttributes = (fruitData.triggerMethodOptions & FruitDataStructureTemplate.TriggerMethod.Attributes)!=0;
        bool isState = (fruitData.triggerMethodOptions & FruitDataStructureTemplate.TriggerMethod.State)!=0;
        bool isPlayerAction = (fruitData.triggerMethodOptions & FruitDataStructureTemplate.TriggerMethod.PlayerAction)!=0;

        // 机制类
        if( isRule )
        {
            if((fruitData.specificTriggerRules & FruitDataStructureTemplate.SpecificTriggerRules.None)!=0)return;

            bool isSynthesis = (fruitData.specificTriggerRules & FruitDataStructureTemplate.SpecificTriggerRules.Synthesis)!=0;
            bool isChainTransfer = (fruitData.specificTriggerRules & FruitDataStructureTemplate.SpecificTriggerRules.ChainTransfer)!=0;
            bool isWeakAssociation = (fruitData.specificTriggerRules & FruitDataStructureTemplate.SpecificTriggerRules.WeakAssociation)!=0;
            bool isFourDirectionsLink = (fruitData.specificTriggerRules & FruitDataStructureTemplate.SpecificTriggerRules.FourDirectionsLink)!=0;
            // 合成入口
            
            if(  isSynthesis && baseUnit.level > 1 ) FruitDoSynthesis();
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(testMode)return;
    }
    GameObject Tree; //"残躯化林"
    int TreeCutCount;
    public void FruitDoDying(FruitDataStructureTemplate fruitData,int lastHurt)
    {
        this.fruitData = null;
        fruitData = fruitDatas.Find((f) => (f.rolePuppetsOptions & WhichRole())!=0);
        if(!fruitData)return;
        bool isState = (fruitData.triggerMethodOptions & FruitDataStructureTemplate.TriggerMethod.State)!=0;
        if(isState)
        {
            bool isDie = (fruitData.specificTriggerState & FruitDataStructureTemplate.SpecificTriggerState.Dying)!=0;
            if(isDie)
            {
                bool isRed = (fruitData.rolePuppetsOptions & FruitDataStructureTemplate.RolePuppets.Red)!=0;
                bool isGreen = (fruitData.rolePuppetsOptions & FruitDataStructureTemplate.RolePuppets.Green)!=0;
                bool isBlue = (fruitData.rolePuppetsOptions & FruitDataStructureTemplate.RolePuppets.Blue)!=0;
                bool isPurple = (fruitData.rolePuppetsOptions & FruitDataStructureTemplate.RolePuppets.Purple)!=0;
                if(isRed && baseUnit_skinName == "red"){
                    Debug.Log("红色水果 濒死");
                }
                if(isGreen && baseUnit_skinName == "green"){
                    Debug.Log("绿色水果 濒死");
                    if(fruitData.fruitName == "残躯化林")
                    {
                        baseUnit.unitTemplate.health += lastHurt;
                        TextToDispaly = "<b><color=green>残躯化林:启动！</color></b>";
                        Invoke(nameof(WarningDisplay),0.5f);
                        // 生成树木
                        if(!Tree)
                        {
                            Tree = Instantiate(fruitData.SubObject[0],transform.position,Quaternion.identity);
                            Tree.transform.parent = transform.parent;
                        }
                        // 5次摧毁
                        TreeCutCount --;
                        if(TreeCutCount == 0)
                        {
                            Destroy(Tree,3f);
                            AICommand die = new AICommand(AICommand.CommandType.Die);
                            baseUnit.ExecuteCommand(die);
                        }
                    }
                    
                }
                if(isBlue && baseUnit_skinName == "blue"){
                    Debug.Log("蓝色水果 濒死");
                }
                if(isPurple && baseUnit_skinName == "purple"){
                    Debug.Log("紫色水果 濒死");
                }
            }
        };
    }
    public void FruitDoDie(FruitDataStructureTemplate fruitData)
    {
        this.fruitData = null;
        fruitData = fruitDatas.Find((f) => (f.rolePuppetsOptions & WhichRole())!=0);
        if(!fruitData)return;
        bool isState = (fruitData.triggerMethodOptions & FruitDataStructureTemplate.TriggerMethod.State)!=0;
        if(isState)
        {
            bool isDie = (fruitData.specificTriggerState & FruitDataStructureTemplate.SpecificTriggerState.Dead)!=0;
            if(isDie)
            {
                bool isRed = (fruitData.rolePuppetsOptions & FruitDataStructureTemplate.RolePuppets.Red)!=0;
                bool isGreen = (fruitData.rolePuppetsOptions & FruitDataStructureTemplate.RolePuppets.Green)!=0;
                bool isBlue = (fruitData.rolePuppetsOptions & FruitDataStructureTemplate.RolePuppets.Blue)!=0;
                bool isPurple = (fruitData.rolePuppetsOptions & FruitDataStructureTemplate.RolePuppets.Purple)!=0;
                if(isRed && baseUnit_skinName == "red"){
                    Debug.Log("红色水果 死亡");
                }
                if(isGreen && baseUnit_skinName == "green"){
                    Debug.Log("绿色水果 死亡");
                }
                if(isBlue && baseUnit_skinName == "blue"){
                    Debug.Log("蓝色水果 死亡");
                }
                if(isPurple && baseUnit_skinName == "purple"){
                    Debug.Log("紫色水果 死亡");
                }
            }
        };
        
    }
    void FruitDoSynthesis()
    {
        if((fruitData.rolePuppetsOptions & FruitDataStructureTemplate.RolePuppets.None)!=0)return;
        bool isRed = (fruitData.rolePuppetsOptions & FruitDataStructureTemplate.RolePuppets.Red)!=0;
        bool isGreen = (fruitData.rolePuppetsOptions & FruitDataStructureTemplate.RolePuppets.Green)!=0;
        bool isBlue = (fruitData.rolePuppetsOptions & FruitDataStructureTemplate.RolePuppets.Blue)!=0;
        bool isPurple = (fruitData.rolePuppetsOptions & FruitDataStructureTemplate.RolePuppets.Purple)!=0;

        if(isRed && baseUnit_skinName == "red"){
            Debug.Log("红色水果 合成");
            if(fruitData.fruitName == "玩具电剑")
            {
                
                TextToDispaly = "<b><color=red>玩具电剑:启动！</color></b>";
                Invoke(nameof(WarningDisplay),0.5f);
                if(fruitData.weaponTemplate!=null)
                {
                    baseUnit.weapon.SetWeapon(fruitData.weaponTemplate,fruitData.duration); // 换上电剑
                }
                
            }
            
        }
        if(isGreen && baseUnit_skinName == "green"){
            Debug.Log("绿色水果 合成");
        }
        if(isBlue && baseUnit_skinName == "blue"){
            Debug.Log("蓝色水果 合成");
        }
        if(isPurple && baseUnit_skinName == "purple"){
            Debug.Log("紫色水果 合成");
        }
        
    }
    public void FruitIn(FruitDataStructureTemplate fruitData)
    {
        if(fruitDatas.Contains(fruitData))return;
        fruitDatas.Add(fruitData);
    }
# endregion 数据关系
# region 数据操作
    FruitDataStructureTemplate.RolePuppets WhichRole()
    {
        FruitDataStructureTemplate.RolePuppets role = new();
        switch(baseUnit_skinName)
        {
            case "red":
                role = FruitDataStructureTemplate.RolePuppets.Red;
            break;
            case "green":
                role = FruitDataStructureTemplate.RolePuppets.Green;
            break;
            case "blue":
                role = FruitDataStructureTemplate.RolePuppets.Blue;
            break;
            case "purple":
                role = FruitDataStructureTemplate.RolePuppets.Purple;
            break;
        }
        return role;
    }
    void ApplyDifference(DifferenceAttributeTemplate diff)
    {
        baseUnit.flip = diff.flipSkeleton;
        diff.skeletonRenderer = baseUnit.SkeletonRenderer; 
        diff.SwitchToSkeletonData();
        baseUnit.animator.runtimeAnimatorController = diff.animatorController;
       
        int index = baseUnit.weapon.weaponTemplates.FindIndex((weapon) => weapon.weaponType == diff.weaponTemplate.weaponType);
        baseUnit.weapon.weaponTemplates[index] = diff.weaponTemplate;
        baseUnit.weapon.SetWeapon(diff.weaponTemplate.weaponType);
        
    }
    void WarningDisplay()
    {
        warningSystem.inText1 = TextToDispaly;        
        warningSystem.changeWarningTypes = WarningSystem.WarningType.Custom_Emergency;
    }
# endregion 数据操作
}
