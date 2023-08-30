
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
[CreateAssetMenu(fileName = "Effect_", menuName = "状态机/创建单位特效模板", order = 5)]

public class EffectTemplate: ScriptableObject
{
    [Tooltip("收集特效")]
	public SkeletonAnimation collectEffect = null;

    [Tooltip("入场特效")]
	public SkeletonAnimation startEffect = null;

    [Tooltip("退场特效")]
    public SkeletonAnimation endEffect = null;

    [Tooltip("激活特效")]
    public SkeletonAnimation activeEffect = null;
    
    [Tooltip("攻击特效")]
    public SkeletonAnimation attackEffect = null;

    [HideInInspector]
    public EffectType effectType = EffectType.Start;
    [HideInInspector]
    public enum EffectType
    {
        Collect,
        Start,
        End,
        Active,
        Attacking,
    }
    [HideInInspector]
   public struct EffectStateTag
   {
       public const string Always = "_always";
       public const string Onece = "_once";
   }
   
}
