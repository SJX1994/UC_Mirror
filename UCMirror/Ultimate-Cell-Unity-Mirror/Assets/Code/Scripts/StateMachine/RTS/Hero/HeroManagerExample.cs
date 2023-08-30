using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using DG.Tweening;
public class HeroManagerExample : MonoBehaviour
{
#region 数据对象
public GameObject[] heroUnits;
public RectTransform heroPanel,backgroundPanel,heroName,sloganPanel;
public AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
float m_displayTime = 3f;
GameObject bgTemp;
GameObject heroTemp;
Tween tweenAnimation;
GameObject heroUnit;
#endregion 数据对象
#region 数据关系
public void SelectHero(int index)
{
    switch (index)
    {
        case 0:
            ShowPanel();
            HeroToCreate(heroUnits[index]);
            break;
        case 1:
            ShowPanel();
            HeroToCreate(heroUnits[index]);
            break;
        case 2:
            ShowPanel();
            HeroToCreate(heroUnits[index]);
            break;
        case 3:
            HidePanel();
            break;
        default:
            break;
    }
}
#endregion 数据关系
#region 数据操作

void ShowPanel()
{
    ClearPanel();
    heroPanel.gameObject.SetActive(true);
    backgroundPanel.gameObject.SetActive(true);
    sloganPanel.gameObject.SetActive(true);
    heroName.gameObject.SetActive(true);
}
void HidePanel()
{
    heroPanel.gameObject.SetActive(false);
    backgroundPanel.gameObject.SetActive(false);
    sloganPanel.gameObject.SetActive(false);
    heroName.gameObject.SetActive(false);
    ClearPanel();
}
void ClearPanel()
{
    bgTemp = null;
    heroTemp = null;
    tweenAnimation.Kill();
    foreach (Transform item in heroPanel)
    {
        Destroy(item.gameObject);
    }
    foreach (Transform item in backgroundPanel)
    {
        Destroy(item.gameObject);
    }
}

void HeroToCreate(GameObject heroToCreate)
{
    if (heroToCreate.TryGetComponent(out IHeroUnit ihero))
    {
        UnitHeroTemplate uht = ihero.OnCreating();
        heroTemp = Instantiate(uht.hreoSkeletonGraphic.gameObject,heroPanel,false);
        bgTemp = Instantiate(uht.backgroundSkeletonGraphic.gameObject,backgroundPanel,false);
        sloganPanel.GetComponent<Text>().text = uht.slogan;
        heroName.GetComponent<Text>().text = uht.heroName;
        heroUnit = heroToCreate;
        DoAnimation(heroTemp.GetComponent<RectTransform>());
    }
}
void HeroToDefeated(GameObject heroToDefeated)
{
    if (heroToDefeated.TryGetComponent(out IHeroUnit ihero))
    {
        if(!ihero.OnDefeated().fininsh)
        {
            Time.timeScale = 0;
        }else
        {
            Time.timeScale = 1;
        }
    }
}
void DoAnimation(RectTransform ihero)
{
    if(ihero)
    {
        Time.timeScale = 0;
         tweenAnimation = ihero.DOAnchorPos(new Vector2(1500f, 0f), m_displayTime)
            .ChangeStartValue(new Vector2(-1500f,0f))
            .SetEase(curve);
        tweenAnimation.SetUpdate(true);
        tweenAnimation.onComplete = () => {
            Time.timeScale = 1;
            HidePanel();
            // 假数据
            Vector3 posCreate = new Vector3(0,0,0); 
            Vector3 posCreateMoveTo = new Vector3(-17,0,0); 
            CreateHeroInWorldSpace(posCreate,posCreateMoveTo);
        };
    }
    
}
void CreateHeroInWorldSpace(Vector3 posCreate,Vector3 posCreateMoveTo)
{
    GameObject hero = Instantiate(heroUnit,posCreate,Quaternion.identity);
    StartCoroutine(hero.GetComponent<IHeroUnit>().WhenCreatMoveTo(posCreateMoveTo));
}
#endregion 数据操作
}