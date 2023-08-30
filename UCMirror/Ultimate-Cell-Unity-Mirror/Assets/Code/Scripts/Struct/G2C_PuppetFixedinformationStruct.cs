using Mirror;
using UnityEngine;

public struct G2C_PuppetFixedinformationStruct : NetworkMessage 
{
    public int UnitId;

    // 是否包含信息
    public bool OnPuppetAttacking;

    public bool OnPuppetAttackFinish;

    public bool OnPuppetShooting;

    public bool OnPuppetSide;

    public bool OnPuppetStateChanged;

    public bool OnPuppetTypeChanged;

    public bool OnPuppetSkinChanged;

    public bool OnPuppetDestory;

    public bool OnPuppetChangeWeapon;

    // 信息更新内容
    public bool PuppetAttacking;

    public bool PuppetAttackFinish;

    public bool isPuppetShooting;

    public Vector3 PuppetShootingPos;

    public UnitTemplate.UnitType PuppetSide;

    public Unit.UnitState PuppetState1;

    public Vector3 PuppetState2;

    public UnitTemplate.UnitType PuppetTypeChanged;

    public string PuppetSkinChanged;

    public float PuppetDestory;

    public WeaponTemplate.WeaponType PuppetChangeWeapon;
}