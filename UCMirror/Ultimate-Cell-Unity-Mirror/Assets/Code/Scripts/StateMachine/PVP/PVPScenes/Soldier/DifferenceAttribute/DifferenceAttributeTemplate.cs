using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

[CreateAssetMenu(fileName = "DifferenceAttribute_", menuName = "自定义士兵", order = 1)]

public class DifferenceAttributeTemplate : ScriptableObject
{
   // 通讯
   [HideInInspector]
   public SkeletonRenderer skeletonRenderer;
   // 定制
   [SerializeField]
   public string soldierName;
   public SkeletonDataAsset skeletonDataAsset;
   public bool flipSkeleton;
   public RuntimeAnimatorController animatorController;
   public WeaponTemplate weaponTemplate;
   // 数据
   List<string> skinNames = new List<string>();
   public void SwitchToSkeletonData()
   {
      skeletonRenderer.skeletonDataAsset = skeletonDataAsset;
      GetSkinNames();
      skeletonRenderer.initialSkinName = skinNames[0];
      skeletonRenderer.Initialize(true);
   }
   void GetSkinNames()
   {
      skinNames.Clear();
      foreach(var s in skeletonDataAsset.GetSkeletonData(false).Skins)
      {
         skinNames.Add(s.Name);
      }
   }
}
