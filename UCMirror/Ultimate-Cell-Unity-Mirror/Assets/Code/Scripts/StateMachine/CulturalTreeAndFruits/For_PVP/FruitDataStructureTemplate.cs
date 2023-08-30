using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "FruitData_", menuName = "果实创建", order = 1)]
public class FruitDataStructureTemplate : ScriptableObject
{
    
    
    [Tooltip("作用兵种")]
    public RolePuppets rolePuppetsOptions;
    [System.Flags]
    public enum RolePuppets
    {
        None = 0,
        Red = 1 << 0,
        Green = 1 << 1,
        Blue = 1 << 2,
        Purple = 1 << 3,
    }
    public List<Color> FruitColor()
    {
        List<Color> colors = new();
        bool isRed = (rolePuppetsOptions & RolePuppets.Red)!=0;
        bool isGreen = (rolePuppetsOptions & RolePuppets.Green)!=0;
        bool isBlue = (rolePuppetsOptions & RolePuppets.Blue)!=0;
        bool isPurple = (rolePuppetsOptions & RolePuppets.Purple)!=0;
        if(isRed)
        {
            colors.Add(Color.red);
        }
        if(isGreen)
        {
            colors.Add(Color.green);
        }
        if(isBlue)
        {
            colors.Add(Color.blue);
        }
        if(isPurple)
        {
            colors.Add(Color.magenta);
        }
        return colors;
    }
    [Tooltip("果实名称")]
    public string fruitName;
    [Tooltip("果实描述")]
    public string fruitDescription;
    [Tooltip("果实解释")]
    public string fruitExplanation;
    [Tooltip("果实图标")]
    public Sprite fruitIcon;
    [Tooltip("触发条件")]
    public TriggerMethod triggerMethodOptions;
    [System.Flags]
    public enum TriggerMethod
    {
        None = 0,
        Time = 1 << 0, // 时间触发
        BlockPos = 1 << 1, // 位置触发
        Object = 1 << 2, // 对象
        Attributes = 1 << 3,// 属性
        State = 1 << 4,// 状态
        PlayerAction = 1 << 5, // 玩家行动
        Rule = 1 << 6,// 规则/机制
        Skill = 1 << 7,// 技巧
        Probability = 1 << 8,// 概率
        Level = 1 << 9,// 等级
    }
    [Tooltip("具体时间触发")]
    public float specificTriggerTime;
    [Tooltip("具体触发位置")]
    public SpecificTriggerBlockPos specificTriggerBlockPos;
    [System.Flags]
    public enum SpecificTriggerBlockPos
    {
        None = 0,
        EvenNumber = 1 << 0, // 所有偶数砖块
        CardinalityNumber = 1 << 1, // 所有基数砖块
        SpecificNumber = 1 << 2, // 特定砖块
    }

    [Tooltip("具体触发规则/机制")]
    public SpecificTriggerRules specificTriggerRules;
    [System.Flags]
    public enum SpecificTriggerRules
    {
        None = 0,
        Synthesis = 1 << 0, // 合成
        WeakAssociation = 1 << 1, // 弱势关联
        ChainTransfer = 1 << 2, // 链式传递
        FourDirectionsLink = 1 << 3,// 四方联结
    }
    [Tooltip("具体触发等级")]
    public SpecificTriggerLevel specificTriggerLevel;
    [System.Flags]
    public enum SpecificTriggerLevel
    {
        None = 0,
        Level_1 = 1 << 0, 
        Level_2 = 1 << 1, 
        Level_3 = 1 << 2, 
        Level_4 = 1 << 3,
    }
    [Tooltip("具体触发等级")]
    public SpecificTriggerState specificTriggerState;
    [System.Flags]
    public enum SpecificTriggerState
    {
        None = 0,
        Idle = 1 << 0, 
        Guarding = 1 << 1, 
        Attacking = 1 << 2, 
        Dead = 1 << 3,
        Dying = 1 << 4,
    }
    [Tooltip("持续时间")]
    public float duration;
    [Tooltip("正面效果")]
    public PositiveEffect positiveEffect;
    [System.Flags]
    public enum PositiveEffect
    {
        None = 0,
        CD_Reduction = 1 << 0, // 减少CD
        AttributeIncrease = 1 << 1, // 属性增加
        NewBuffMechanism = 1 << 2, // 新增增益机制
        BaseBuffMechanism = 1 << 3,// 基础机制增益
    }
    // 改变武器形象
    [Tooltip("改变武器形象")]
    public WeaponTemplate weaponTemplate;
    // 子生成物
    [Tooltip("子生成物")]
    public List<GameObject> SubObject;

    
}
