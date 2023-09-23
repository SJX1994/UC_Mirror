using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
public class Effect : MonoBehaviour
{
#region 数据对象
    public bool filp = false;
    public EffectTemplate effectCollection;
    public EffectTemplate.EffectType effectType;
    List<GameObject> effects = new();
    const string CLONE = "(Clone)";
    const string EFFECT_ANI = "animation";
    // SkeletonAnimation effectChecker_sa = new();
#endregion 数据对象
#region 数据关系
    public void Start()
    {
        effectCollection = Instantiate(effectCollection);
    }
    public void FixedUpdate()
    {
        if(effects.Count == 0)
        {
            return;
        }
        foreach(GameObject effect in effects)
        {
            Vector3 targetPostition = new Vector3(effect.transform.position.x, Camera.main.transform.position.y, effect.transform.position.z - 1);
            effect.transform.LookAt(targetPostition);
        }
        
    }
    public void AddEffect()
    {
        switch(effectType)
        {
            case EffectTemplate.EffectType.Collect:
                if(!effectCollection.collectEffect)return;
                
                AddCollectEffect();
                break;
            case EffectTemplate.EffectType.Start:
                if(!effectCollection.startEffect)return;
                
                AddStartEffect();
                break;
            case EffectTemplate.EffectType.End:
                if(!effectCollection.endEffect)return;
                
                AddEndEffect();
                break;
            case EffectTemplate.EffectType.Active:
                if(!effectCollection.activeEffect)return;
                
                AddActiveEffect();
                break;
            case EffectTemplate.EffectType.Attacking:
                if(!effectCollection.attackEffect)return;
                
                AddAttackEffect();
                break;
        }
        
    }
    public void RemoveAllEffect(float delayTime)
    {
        foreach(GameObject effect in effects)
        {
            Destroy(effect,delayTime);
        }
        Destroy(this,delayTime);
    }
#endregion 数据关系
#region 数据操作
public SkeletonRenderer SetFlip (float horizontal,SkeletonRenderer skeletonRenderer) 
{
	if (horizontal != 0) {
		skeletonRenderer.Skeleton.ScaleX = horizontal > 0 ? 1f : -1f;
	}
    return skeletonRenderer;
}
/// <summary>
/// 获取/检查 特效是否存在
/// </summary>
/// <param name="effectName"></param>
/// <returns></returns>
GameObject GetEffect(string effectName)
{
   return GetChildGameObject(transform.gameObject, effectName);
}
static public GameObject GetChildGameObject(GameObject fromGameObject, string withName) {
         Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
         foreach (Transform t in ts) 
         {
           if (t.gameObject.name == withName)
           {
             return t.gameObject;
           }
         } 
         return null;
     }

/// <summary>
/// 添加特效
/// </summary>
void AddAttackEffect()
{
    GameObject effectChecker = GetEffect(effectCollection.attackEffect.name + EffectTemplate.EffectStateTag.Always);
    
    if(effectChecker)
    {
        SkeletonAnimation effectChecker_sa = effectChecker.GetComponent<SkeletonAnimation>();
        effectChecker.SetActive(true);
        Spine.AnimationState state = effectChecker_sa.AnimationState;
        state.SetAnimation(0, EFFECT_ANI, false);
        
        effectChecker_sa.AnimationState.Complete += delegate (Spine.TrackEntry trackEntry)
        {
            effectChecker.SetActive(false);
        };
    }else
    {
        
        GameObject effect = Instantiate(effectCollection.attackEffect.transform.gameObject, this.transform);
        effect.name = effect.name.Replace(CLONE, EffectTemplate.EffectStateTag.Always);
        effects.Add(effect);
        SkeletonAnimation effectChecker_sa = effect.GetComponent<SkeletonAnimation>();
        effectChecker_sa = SetFlip(filp?-1:1,effectChecker_sa) as SkeletonAnimation;
        effectChecker_sa.timeScale += Random.Range(-0.3f, 0.3f);
    }
}
void AddCollectEffect()
{
    GameObject effectChecker = GetEffect(effectCollection.collectEffect.name + EffectTemplate.EffectStateTag.Always);
    
    if(effectChecker)
    {
        SkeletonAnimation effectChecker_sa = effectChecker.GetComponent<SkeletonAnimation>();
        effectChecker.SetActive(true);
        effectChecker_sa.AnimationState.Complete += delegate (Spine.TrackEntry trackEntry)
        {
            effectChecker.SetActive(false);
        };
    }else
    {
        
        GameObject effect = Instantiate(effectCollection.collectEffect.transform.gameObject, this.transform);
        effect.name = effect.name.Replace(CLONE, EffectTemplate.EffectStateTag.Always);
        effects.Add(effect);
        SkeletonAnimation effectChecker_sa = effect.GetComponent<SkeletonAnimation>();
        effectChecker_sa = SetFlip(filp?-1:1,effectChecker_sa) as SkeletonAnimation;
        effectChecker_sa.timeScale += Random.Range(-0.3f, 0.3f);
    }
    
}
void AddStartEffect() 
{
    // 创建
    GameObject effect = Instantiate(effectCollection.startEffect.transform.gameObject, this.transform);
    SkeletonAnimation effectChecker_sa = effect.GetComponent<SkeletonAnimation>();
    effectChecker_sa = SetFlip(filp?-1:1,effectChecker_sa) as SkeletonAnimation;
    effect.name = effect.name.Replace(CLONE, EffectTemplate.EffectStateTag.Onece);
    effectChecker_sa.timeScale += Random.Range(-0.3f, 0.3f);
    // 收集
    effects.Add(effect);
    // 销毁
    effectChecker_sa.AnimationState.Complete += delegate (Spine.TrackEntry trackEntry)
    {
        effects.Remove(effect);
        DestroyImmediate(effect);
    };
}
void AddEndEffect() 
{
    // 创建
    GameObject effect = Instantiate(effectCollection.endEffect.transform.gameObject, this.transform);
    SkeletonAnimation effectChecker_sa = effect.GetComponent<SkeletonAnimation>();
    effectChecker_sa = SetFlip(filp?-1:1,effectChecker_sa) as SkeletonAnimation;
    effect.name = effect.name.Replace(CLONE, EffectTemplate.EffectStateTag.Onece);
    effectChecker_sa.timeScale += Random.Range(-0.3f, 0.3f);
    // 收集
    effects.Add(effect);
    // 销毁
    effectChecker_sa.AnimationState.Complete += delegate (Spine.TrackEntry trackEntry)
    {
        effects.Remove(effect);
        DestroyImmediate(effect);
    };
}
void AddActiveEffect() 
{
    GameObject effectChecker = GetEffect(effectCollection.activeEffect.name + EffectTemplate.EffectStateTag.Always);
    
    if(effectChecker)
    {
        SkeletonAnimation effectChecker_sa = effectChecker.GetComponent<SkeletonAnimation>();
        effectChecker.SetActive(true);
        Spine.AnimationState state = effectChecker_sa.AnimationState;
        state.SetAnimation(0, EFFECT_ANI, true);
        effectChecker_sa.AnimationState.Complete += delegate (Spine.TrackEntry trackEntry)
        {
            effectChecker.SetActive(false);
        };
    }else
    {
        
        GameObject effect = Instantiate(effectCollection.activeEffect.transform.gameObject, this.transform);
        effect.name = effect.name.Replace(CLONE, EffectTemplate.EffectStateTag.Always);
        effects.Add(effect);
        SkeletonAnimation effectChecker_sa = effect.GetComponent<SkeletonAnimation>();
        effectChecker_sa = SetFlip(filp?-1:1,effectChecker_sa) as SkeletonAnimation;
        effectChecker_sa.timeScale += Random.Range(-0.3f, 0.3f);
    }
}
#endregion 数据操作
}
