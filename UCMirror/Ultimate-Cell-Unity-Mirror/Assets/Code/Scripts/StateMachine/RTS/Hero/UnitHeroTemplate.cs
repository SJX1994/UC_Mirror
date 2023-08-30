
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
[CreateAssetMenu(fileName = "Hero_SloganPage_", menuName = "状态机/创建单位英雄模板", order = 1)]

public class UnitHeroTemplate: ScriptableObject
{
    [Tooltip("角色名称")]
	public string heroName = "产品名称";

    [Tooltip("口号")]
	public string slogan = "各位的梦想由我来守护！";

    [Tooltip("前景spine")]
    public SkeletonGraphic frontgroundSkeletonGraphic;

    [Tooltip("背景spine")]
    public SkeletonGraphic backgroundSkeletonGraphic;
  
    [Tooltip("角色spine")]
    public SkeletonGraphic hreoSkeletonGraphic;

    [Tooltip("消耗灵魂")]
    public int price = 5;

    [HideInInspector]
    public bool fininsh;
    public void PlayAnimation(string heroName,string backgroundName)
    {
        fininsh = false;
        if(hreoSkeletonGraphic)
        {
            hreoSkeletonGraphic.UnscaledTime = true;
            hreoSkeletonGraphic.AnimationState.SetAnimation(0, heroName, false);
            hreoSkeletonGraphic.UpdateComplete += AfterUpdateComplete;
        }
        if(backgroundSkeletonGraphic)
        {
            backgroundSkeletonGraphic.UnscaledTime = true;
            backgroundSkeletonGraphic.AnimationState.SetAnimation(0, backgroundName, true);
        }
        if(frontgroundSkeletonGraphic)
        {
            frontgroundSkeletonGraphic.UnscaledTime = true;
            frontgroundSkeletonGraphic.AnimationState.SetAnimation(0, heroName, false);
            
        }
        
    }

    void AfterUpdateComplete(ISkeletonAnimation anim)
    {
        fininsh = true;
        anim.UpdateComplete -= AfterUpdateComplete;
    }
   
}
