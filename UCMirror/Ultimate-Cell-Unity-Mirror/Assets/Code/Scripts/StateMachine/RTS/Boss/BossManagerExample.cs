using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
public class BossManagerExample : MonoBehaviour
{
    #region 数据对象
    public RectTransform mask,background;
    public Vector3 offset;
    private Vector3 creatPosition;

    public GameObject boss;
    bool finishBossDisplay;

    // private BroadcastClass broadcastClass;
    // private CommunicationInteractionManager CommunicationManager;
    #endregion 数据对象
    #region 数据关系
    void Start()
    {
        if(mask!=null || background!=null)
        {
            mask.gameObject.SetActive(false);
            background.gameObject.SetActive(false);
        }
        
         finishBossDisplay = true;

        // 通信获取
        // 暂时获取方式
        // CommunicationManager = GameObject.Find("LanNetWorkManager").gameObject.GetComponent<CommunicationInteractionManager>();
        // broadcastClass = CommunicationManager.GetComponent<BroadcastClass>();

        // broadcastClass.CreateBoss += CreateBoss;
    }

    public void CreateBoss(int info)
    {
        
       Vector3 position =  new Vector3(17f,0f,0f);
       GameObject bossTmp = Instantiate(boss,position,Quaternion.identity);
       boss = bossTmp;
       Weapon weapon;
       if(boss.GetComponent<Unit>().SkeletonRenderer.transform.TryGetComponent<Weapon>(out weapon))
        {
            weapon.SetWeapon(WeaponTemplate.WeaponType.Spear);
            weapon.OnChangeWeapon += BossWeaponChange;
        }
       finishBossDisplay = false; 
       if(mask==null || background==null)return;
       mask.gameObject.SetActive(true);
       background.gameObject.SetActive(true);
       mask.DOScale(new Vector3(17f,17f,17f),3f).SetEase(Ease.InBounce).onComplete = () => {
                finishBossDisplay = true;
                mask.gameObject.SetActive(false);
                background.gameObject.SetActive(false);
        };

        


        // Boss Just Create Once
        // broadcastClass.CreateBoss -= CreateBoss;
    }

    void Update()
    {
        if(finishBossDisplay)return;
        if(mask==null || background==null)return;
        
        Vector3 pos = Camera.main.WorldToScreenPoint(boss.transform.position + offset);
        if(mask.transform.position!=pos) mask.transform.position = pos;
    }
    #endregion 数据关系
    #region 数据操作
     /// <summary>
    /// Boss战士切换武器
    /// </summary>
    /// <param name="weapon"></param>
    void BossWeaponChange(Weapon weapon)
    {

    }
    #endregion 数据操作
}
