using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;

public class OutLine : MonoBehaviour
{
    [Serializable]
	public struct SlotMaterialOverride : IEquatable<SlotMaterialOverride> {
		public bool overrideDisabled;
		[SpineSlot]
		public string slotName;
		public Material material;
		public bool Equals (SlotMaterialOverride other) {
			return overrideDisabled == other.overrideDisabled && slotName == other.slotName && material == other.material;
		}
	}
    [SerializeField] protected List<SlotMaterialOverride> customSlotMaterials = new List<SlotMaterialOverride>();
    public Material material_outLine;
    public Material material_base;
    SkeletonRenderer skeletonRenderer;
    SkeletonRenderer SkeletonRenderer
    {
        get
        {
            if(!skeletonRenderer)skeletonRenderer = GetComponent<SkeletonRenderer>();
            return skeletonRenderer;
        }
    }
    SkeletonMecanim skeletonMecanim;
    SkeletonMecanim SkeletonMecanim
    {
        get
        {
            if(!skeletonMecanim)skeletonMecanim = GetComponent<SkeletonMecanim>();
            return skeletonMecanim;
        }
    }
    MeshRenderer meshRenderer;
    UnityEngine.Material[] vMats;
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
         SkeletonRenderer.OnMeshAndMaterialsUpdated += OnMeshAndMaterialsUpdated;
        // SkeletonMecanim.CustomSlotMaterials.Add(SkeletonMecanim.skeleton.FindSlot(customSlotMaterials[0].slotName),material_outLine);
       
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnMeshAndMaterialsUpdated(SkeletonRenderer skeletonRenderer)
    {
        Material material = skeletonRenderer.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial;
        Debug.Log(material);
    }
    private void OnDestroy()
    {
        // 当对象被销毁时，确保在.OnMeshAndMaterialsUpdated回调上取消订阅
        if (!SkeletonRenderer)return;
        SkeletonRenderer.OnMeshAndMaterialsUpdated -= OnMeshAndMaterialsUpdated;
    }
}
