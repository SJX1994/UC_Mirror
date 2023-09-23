
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using System.Collections.Generic;
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
    [Tooltip("死亡粒子")]
    public List<ParticleSystem> deadParticle = null;

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
    public ParticleSystem Display_setSkine(SoldierBehaviors self)
    {
        if(self.skinName=="")return null;
        ParticleSystem boonEffectPrefab = null;
        switch(self.morale.soldierType)
        {
            case MoraleTemplate.SoldierType.Red:
                boonEffectPrefab = deadParticle.Find(x=>x.name == "BoomEffect_red");
                break;
            case MoraleTemplate.SoldierType.Blue:
                boonEffectPrefab = deadParticle.Find(x=>x.name == "BoomEffect_blue");
                break;
            case MoraleTemplate.SoldierType.Green:
                boonEffectPrefab = deadParticle.Find(x=>x.name == "BoomEffect_green");
                break;
            case MoraleTemplate.SoldierType.Purple:
                boonEffectPrefab = deadParticle.Find(x=>x.name == "BoomEffect_purple");
                break;
        }
        return boonEffectPrefab;
    }
   
}
