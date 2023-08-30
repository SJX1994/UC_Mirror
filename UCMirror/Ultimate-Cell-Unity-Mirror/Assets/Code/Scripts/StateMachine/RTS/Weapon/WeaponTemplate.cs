using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Spine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Weapon_", menuName = "状态机/创建武器模板", order = 5)]

public class WeaponTemplate: ScriptableObject
{

    [Tooltip("武器形象")]
    public Sprite weaponSprite;

    [Tooltip("攻击力加成")]
	public int attackPowerBonus = 0;

    [Tooltip("攻击距离加成")]
    public float rangeBonus = 0;

    [Tooltip("攻击速度加成")]
    public float attackSpeedBonus = 0;

    [Tooltip("血量加成")]
    public int healthBonus = 0;
    
    [Tooltip("武器类型")]
    public WeaponType weaponType = WeaponType.Sword;
    
    [Tooltip("武器抓握点位置纵向调整")]
    public  float pos_weaponX = 0;

    [Tooltip("武器抓握点位置横向调整")]
    public  float pos_weaponY = 0;

    [Tooltip("武器抓握点位置旋转调整")]
    public  float rot_weapon = 0;

     [Tooltip("武器抓握点位置缩放调整")]
    public float scale_weapon = 0;

    public enum WeaponType
    {
        // 飞镖 （攻击距离加成）
        Bow,
        // 大剑 （攻击速度加成）
        Sword,
        // 长矛 （攻击力加成）
        Spear,
        // 盾牌 （血量加成）
        Shield,
        Null,
    }
    public enum WeaponLevelType
    {
        // 极品
        Legend,
        // 上等
        Good,
        // 中等
        Average,
        // 下等
        Poor,
    }
    
}
