using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Spine.Unity.AttachmentTools;
using UnityEngine.Events;
using DG.Tweening;
public class Weapon : MonoBehaviour
{
#region 数据对象
	public bool IsPuppet = false;
    	public const string DefaultPMAShader = "Spine/Skeleton";
	public const string DefaultStraightAlphaShader = "Sprites/Default";
      public UnityAction<Weapon> OnChangeWeapon; // 武器切换事件
	public UnityAction<bool,Vector3> OnUnitShooting; // 射击事件
      [HideInInspector]
      public float pos_weaponX , pos_weaponY , rot_weapon, scale_weapon = 0f;
	RegionAttachment attachment;
	Slot spineSlot;
	bool applyPMA;
	WeaponTemplate.WeaponType thisWeaponType;
	// 通讯
	[HideInInspector]
      public WeaponTemplate weaponTemplate;
	static Dictionary<Texture, AtlasPage> atlasPageCache;
	Unit unit;
	PuppetUnit puppetUnit;
	ApplyDifferenceAttribute applyDifferenceAttribute;
	bool isBonus = false;
	public SkeletonMecanim skeletonAnimation;
	// 武器指向目标
	[SpineBone(dataField: "skeletonAnimation")]
	public string boneName;
	[HideInInspector]
	public Bone bone;
	[HideInInspector]
	public SpriteRenderer shooter;
	Vector3 shooterPos;
	#region Inspector
	public bool updateAim = true;
	public bool attachOnStart = true;
	public bool overrideAnimation = true;
	public Sprite weaponSprite;
	[SpineSlot] public string slot;
      public List<WeaponTemplate> weaponTemplates;
	#endregion
	
#endregion 数据对象
#region 数据关系
void Start () 
{     
	
	bone = skeletonAnimation.Skeleton.FindBone(boneName);
	// Initialize slot and attachment references.
	Initialize(false);
	if (attachOnStart)
		Attach();
	if(!puppetUnit)
	{
		transform.parent.TryGetComponent(out puppetUnit);
	}
	if(!unit)
	{
		transform.parent.TryGetComponent(out unit);
		return;
	}
	shooter = transform.Find("Shooter").GetComponent<SpriteRenderer>();
	shooterPos = shooter.transform.position;
	shooter.gameObject.SetActive(false);
	
	
}
void Update()
{
	if(!IsPuppet)
	{
		if(unit.targetOfAttack == null || skeletonAnimation == null || !updateAim) return;
		if(unit.state == Unit.UnitState.Dead)
		{
			return;
		}
		Vector3 skeletonSpacePoint = skeletonAnimation.transform.InverseTransformPoint(unit.targetOfAttack.transform.position);
		skeletonSpacePoint.x *= skeletonAnimation.Skeleton.ScaleX;
		skeletonSpacePoint.y *= skeletonAnimation.Skeleton.ScaleY;
		bone.SetLocalPosition(skeletonSpacePoint);
	}else
	{
		if(!puppetUnit)return;
		// Vector3 skeletonSpacePoint = skeletonAnimation.transform.InverseTransformPoint(puppetUnit.puppetTargetPosition);
		// skeletonSpacePoint.x *= skeletonAnimation.Skeleton.ScaleX;
		// skeletonSpacePoint.y *= skeletonAnimation.Skeleton.ScaleY;
		// bone.SetLocalPosition(skeletonSpacePoint);
	}
	
	
	if(shooter==null)return;
	shooter.transform.Rotate(shooter.transform.localRotation.x,shooter.transform.localRotation.y,shooter.transform.localRotation.z+720*Time.deltaTime);
	
}
void OnDestroy () 
{
	ISkeletonAnimation animatedSkeleton = GetComponent<ISkeletonAnimation>();
	if (animatedSkeleton != null)
		animatedSkeleton.UpdateComplete -= AnimationOverrideSpriteAttach;
}

public void SetWeapon(EventType.UnitColor color)
{
	transform.parent.TryGetComponent(out applyDifferenceAttribute);   
	if(!applyDifferenceAttribute)return;
            switch(color)
            {
                // TODO 更多武器
                case EventType.UnitColor.red:
		    	if(applyDifferenceAttribute.differenceAttributeTemplate_Red!=null)break;
                    SetWeapon(WeaponTemplate.WeaponType.Sword);
                    thisWeaponType = WeaponTemplate.WeaponType.Sword;
                    break;
                case EventType.UnitColor.blue:
		    	if(applyDifferenceAttribute.differenceAttributeTemplate_Blue!=null)break;
                    SetWeapon(WeaponTemplate.WeaponType.Spear);
                    thisWeaponType = WeaponTemplate.WeaponType.Spear;
                    break;
                case EventType.UnitColor.green:
		    	if(applyDifferenceAttribute.differenceAttributeTemplate_Green!=null)break;  
                    SetWeapon(WeaponTemplate.WeaponType.Shield);
                    thisWeaponType = WeaponTemplate.WeaponType.Shield;
                    break;
                case EventType.UnitColor.purple:
		    	if(applyDifferenceAttribute.differenceAttributeTemplate_Purple!=null)break;
		    	  SetWeapon(WeaponTemplate.WeaponType.Bow);
                    thisWeaponType = WeaponTemplate.WeaponType.Bow;
                    
                    break;
                
            }
            // 修改武器后对应产生属性变化事件
            if(OnChangeWeapon!=null)
            {
                OnChangeWeapon(this);
            }
}
public bool Shoot(Vector3 target,bool rotation = true)
{
	
	shooterPos = transform.position;
	shooter.gameObject.SetActive(true);
      OnUnitShooting?.Invoke(true,target);
	float duration = 0.5f;
	if(IsPuppet)
	{
	   // TODO Puppet读取速度
	}else
	{
	   duration = 1/unit.unitTemplate.attackSpeed;
	}
        
	
	shooter.transform.DOMove(target,duration).OnComplete(() => {
		shooter.gameObject.SetActive(false);
		OnUnitShooting?.Invoke(false,target);
		shooter.transform.position = shooterPos;
	});
	
	
	return true;
}
#endregion 数据关系
#region 数据操作
#if UNITY_EDITOR
    void OnValidate () {
			if (skeletonAnimation == null) skeletonAnimation = GetComponent<ISkeletonComponent>() as SkeletonMecanim;
			ISkeletonComponent skeletonComponent = GetComponent<ISkeletonComponent>();
			SkeletonRenderer skeletonRenderer = skeletonComponent as SkeletonRenderer;
			bool applyPMA;

			if (skeletonRenderer != null) {
				applyPMA = skeletonRenderer.pmaVertexColors;
			} else {
				SkeletonGraphic skeletonGraphic = skeletonComponent as SkeletonGraphic;
				applyPMA = skeletonGraphic != null && skeletonGraphic.MeshGenerator.settings.pmaVertexColors;
			}

			if (applyPMA) {
				try {
					if (weaponSprite == null)
						return;
					weaponSprite.texture.GetPixel(0, 0);
				} catch (UnityException e) {
					Debug.LogFormat("Texture of {0} ({1}) is not read/write enabled. SpriteAttacher requires this in order to work with a SkeletonRenderer that renders premultiplied alpha. Please check the texture settings.", weaponSprite.name, weaponSprite.texture.name);
					UnityEditor.EditorGUIUtility.PingObject(weaponSprite.texture);
					throw e;
				}
			}
		}
#endif
       
	static AtlasPage GetPageFor (Texture texture, Shader shader) {
			if (atlasPageCache == null) atlasPageCache = new Dictionary<Texture, AtlasPage>();
			AtlasPage atlasPage;
			atlasPageCache.TryGetValue(texture, out atlasPage);
			if (atlasPage == null) {
				Material newMaterial = new Material(shader);
				atlasPage = newMaterial.ToSpineAtlasPage();
				atlasPageCache[texture] = atlasPage;
			}
			return atlasPage;
	}
	
	public void SetWeapon(WeaponTemplate.WeaponType weaponType)
      {
            weaponTemplate = weaponTemplates.Find((weapon) => weapon.weaponType == weaponType);
            weaponSprite = weaponTemplate.weaponSprite;
            pos_weaponX = weaponTemplate.pos_weaponX;
            pos_weaponY = weaponTemplate.pos_weaponY; 
            rot_weapon = weaponTemplate.rot_weapon;
            scale_weapon =  weaponTemplate.scale_weapon;
            Initialize(true);
		weaponTemplate = Instantiate(weaponTemplate); // 复制武器模板 防止重写
		// 武器加成
		if(transform.parent.TryGetComponent(out unit) && !isBonus)
		{
			isBonus = true;
			unit.unitTemplate.engageDistance += this.weaponTemplate.rangeBonus;
			unit.unitTemplate.guardDistance += this.weaponTemplate.rangeBonus;
			unit.unitTemplate.attackPower += this.weaponTemplate.attackPowerBonus;
			unit.unitTemplate.attackSpeed += this.weaponTemplate.attackSpeedBonus;
			unit.unitTemplate.health += this.weaponTemplate.healthBonus;
			if(unit.unitTemplate.engageDistance<=0)unit.unitTemplate.engageDistance = 1;
			if(unit.unitTemplate.guardDistance<=0)unit.unitTemplate.guardDistance = 1;
			if(unit.unitTemplate.attackPower<=0)unit.unitTemplate.attackPower = 1;
			if(unit.unitTemplate.attackSpeed<=0)unit.unitTemplate.attackSpeed = 1;
			if(unit.unitTemplate.health<=0)unit.unitTemplate.health = 1;
			
			
		}else
		{
			return;
		}
		
      }
	public void ResetWeaponDispaly(WeaponTemplate.WeaponType weaponType)
      {
		
            weaponTemplate = weaponTemplates.Find((weapon) => weapon.weaponType == weaponType);
            weaponSprite = weaponTemplate.weaponSprite;
            pos_weaponX = weaponTemplate.pos_weaponX;
            pos_weaponY = weaponTemplate.pos_weaponY; 
            rot_weapon = weaponTemplate.rot_weapon;
            scale_weapon =  weaponTemplate.scale_weapon;
            Initialize(true);
		
		
      }
	WeaponTemplate weaponDisplay;
	public void SetWeapon(WeaponTemplate weaponDisplay,float duration = 3f)
	{
		this.weaponDisplay = Instantiate(weaponDisplay); // 复制武器模板 防止重写
		weaponSprite = weaponDisplay.weaponSprite;
            pos_weaponX = weaponDisplay.pos_weaponX;
            pos_weaponY = weaponDisplay.pos_weaponY; 
            rot_weapon = weaponDisplay.rot_weapon;
            scale_weapon =  weaponDisplay.scale_weapon;
		Initialize(true);
		if(transform.parent.TryGetComponent(out unit) && !isBonus)
		{
			isBonus = true;
			unit.unitTemplate.engageDistance += weaponDisplay.rangeBonus;
			unit.unitTemplate.guardDistance += weaponDisplay.rangeBonus;
			unit.unitTemplate.attackPower += weaponDisplay.attackPowerBonus;
			unit.unitTemplate.attackSpeed += weaponDisplay.attackSpeedBonus;
			unit.unitTemplate.health += weaponDisplay.healthBonus;
			if(unit.unitTemplate.engageDistance<=0)unit.unitTemplate.engageDistance = 1;
			if(unit.unitTemplate.guardDistance<=0)unit.unitTemplate.guardDistance = 1;
			if(unit.unitTemplate.attackPower<=0)unit.unitTemplate.attackPower = 1;
			if(unit.unitTemplate.attackSpeed<=0)unit.unitTemplate.attackSpeed = 1;
			if(unit.unitTemplate.health<=0)unit.unitTemplate.health = 1;
			
			
		}
		OnChangeWeapon?.Invoke(this);
		Invoke(nameof(ResetWeapon),duration);
	}
	void ResetWeapon()
	{
		if(transform.parent.TryGetComponent(out unit) && !isBonus)
		{
			isBonus = true;
			unit.unitTemplate.engageDistance -= weaponDisplay.rangeBonus;
			unit.unitTemplate.guardDistance -= weaponDisplay.rangeBonus;
			unit.unitTemplate.attackPower -= weaponDisplay.attackPowerBonus;
			unit.unitTemplate.attackSpeed -= weaponDisplay.attackSpeedBonus;
			unit.unitTemplate.health -= weaponDisplay.healthBonus;
			if(unit.unitTemplate.engageDistance<=0)unit.unitTemplate.engageDistance = 1;
			if(unit.unitTemplate.guardDistance<=0)unit.unitTemplate.guardDistance = 1;
			if(unit.unitTemplate.attackPower<=0)unit.unitTemplate.attackPower = 1;
			if(unit.unitTemplate.attackSpeed<=0)unit.unitTemplate.attackSpeed = 1;
			if(unit.unitTemplate.health<=0)unit.unitTemplate.health = 1;
			
			
		}
		ResetWeaponDispaly(thisWeaponType);
		OnChangeWeapon?.Invoke(this);
		this.weaponDisplay = null; 
	}
	
	void AnimationOverrideSpriteAttach (ISkeletonAnimation animated) {
		if (overrideAnimation && isActiveAndEnabled)
			Attach();
	}
	public void Initialize (bool overwrite = true) 
	{
		if (overwrite || attachment == null) {
			// Get the applyPMA value.
			ISkeletonComponent skeletonComponent = GetComponent<ISkeletonComponent>();
			SkeletonRenderer skeletonRenderer = skeletonComponent as SkeletonRenderer;
			if (skeletonRenderer != null)
				this.applyPMA = skeletonRenderer.pmaVertexColors;
			else {
				SkeletonGraphic skeletonGraphic = skeletonComponent as SkeletonGraphic;
				if (skeletonGraphic != null)
					this.applyPMA = skeletonGraphic.MeshGenerator.settings.pmaVertexColors;
			}
			// Subscribe to UpdateComplete to override animation keys.
			if (overrideAnimation) {
				ISkeletonAnimation animatedSkeleton = skeletonComponent as ISkeletonAnimation;
				if (animatedSkeleton != null) {
					animatedSkeleton.UpdateComplete -= AnimationOverrideSpriteAttach;
					animatedSkeleton.UpdateComplete += AnimationOverrideSpriteAttach;
				}
			}
			spineSlot = spineSlot ?? skeletonComponent.Skeleton.FindSlot(slot);
			
			
			Shader attachmentShader = applyPMA ? Shader.Find(DefaultPMAShader) : Shader.Find(DefaultStraightAlphaShader);
			if (weaponSprite == null)
				attachment = null;
			else
				attachment = applyPMA ? weaponSprite.ToRegionAttachmentPMAClone(attachmentShader) : weaponSprite.ToRegionAttachment(Weapon.GetPageFor(weaponSprite.texture, attachmentShader));
				
              if(pos_weaponX != 0 || pos_weaponY != 0 || rot_weapon != 0 || scale_weapon != 0)
              {
                  // 变换抓握位置
                  attachment.X += pos_weaponX;
                  attachment.Y +=  pos_weaponY;
                  attachment.Rotation += rot_weapon;
                  attachment.ScaleX += scale_weapon;
                  attachment.ScaleY += scale_weapon;
                  attachment.UpdateRegion();
			
                  
              }
		}
	}

	/// <summary>Update the slot's attachment to the Attachment generated from the sprite.</summary>
	public void Attach () 
	{
		if (spineSlot != null)spineSlot.Attachment = attachment;
	}
#endregion 数据操作

}
public static class SpriteAttachmentExtensions {
		[System.Obsolete]
		public static RegionAttachment AttachUnitySprite (this Skeleton skeleton, string slotName, Sprite sprite, string shaderName = Weapon.DefaultPMAShader, bool applyPMA = true, float rotation = 0f) {
			return skeleton.AttachUnitySprite(slotName, sprite, Shader.Find(shaderName), applyPMA, rotation: rotation);
		}

		[System.Obsolete]
		public static RegionAttachment AddUnitySprite (this SkeletonData skeletonData, string slotName, Sprite sprite, string skinName = "", string shaderName = Weapon.DefaultPMAShader, bool applyPMA = true, float rotation = 0f) {
			return skeletonData.AddUnitySprite(slotName, sprite, skinName, Shader.Find(shaderName), applyPMA, rotation: rotation);
		}

		[System.Obsolete]
		public static RegionAttachment AttachUnitySprite (this Skeleton skeleton, string slotName, Sprite sprite, Shader shader, bool applyPMA, float rotation = 0f) {
			RegionAttachment att = applyPMA ? sprite.ToRegionAttachmentPMAClone(shader, rotation: rotation) : sprite.ToRegionAttachment(new Material(shader), rotation: rotation);
			skeleton.FindSlot(slotName).Attachment = att;
			return att;
		}

		[System.Obsolete]
		public static RegionAttachment AddUnitySprite (this SkeletonData skeletonData, string slotName, Sprite sprite, string skinName, Shader shader, bool applyPMA, float rotation = 0f) {
			RegionAttachment att = applyPMA ? sprite.ToRegionAttachmentPMAClone(shader, rotation: rotation) : sprite.ToRegionAttachment(new Material(shader), rotation);

			int slotIndex = skeletonData.FindSlot(slotName).Index;
			Skin skin = skeletonData.DefaultSkin;
			if (skinName != "")
				skin = skeletonData.FindSkin(skinName);

			if (skin != null)
				skin.SetAttachment(slotIndex, att.Name, att);

			return att;
		}
	}
