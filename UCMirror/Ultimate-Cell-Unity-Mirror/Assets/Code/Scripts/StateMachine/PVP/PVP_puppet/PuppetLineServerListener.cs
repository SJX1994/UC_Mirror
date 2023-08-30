using UnityEngine;

public class PuppetLineServerListener : MonoBehaviour 
{
    public PuppetLine puppetLine;

    private int id;

    public G2C_UnitInfoManager unitInfoManager;

    private void Start()
    {
        this.id = puppetLine.id;

        // 固定事件

        // 攻击事件开始监听
        puppetLine.OnPuppetAttacking += OnPuppetAttacking;

        //  攻击事件结束监听
        puppetLine.OnPuppetAttackFinish += OnPuppetAttackFinish;

        //  射击事件监听
        puppetLine.OnPuppetShooting += OnPuppetShooting;

        //  左右变更事件监听
        puppetLine.OnPuppetSide += OnPuppetSide;

        //  状态变化事件
        puppetLine.OnPuppetStateChanged += OnPuppetStateChanged;

        //  类型变化事件
        puppetLine.OnPuppetTypeChanged += OnPuppetTypeChanged;

        //  皮肤变化事件
        puppetLine.OnPuppetSkinChanged += OnPuppetSkinChanged;

        //  死亡
        puppetLine.OnPuppetDestory += OnPuppetDestory;

        //  武器切换事件
        puppetLine.OnPuppetChangeWeapon += OnPuppetChangeWeapon;

        // Update事件

        //  位置变化事件
        puppetLine.OnPuppetPositionChange += OnPuppetPositionChange;

        //  缩放变化事件
        puppetLine.OnPuppetScaleChanged += OnPuppetScaleChanged;

        //  血量更新事件
        puppetLine.OnPuppetHealthChange += OnPuppetHealthChange;

        //  翻转事件
        puppetLine.OnPuppetFilp += OnPuppetFilp;

        //  速度变更事件
        puppetLine.OnPuppetSpeedChange += OnPuppetSpeedChange;

        //  攻击目标变更事件
        puppetLine.OnPuppetTargetOfAttackChange += OnPuppetTargetOfAttackChange;

        // 机制事件

        //  机制模式变更事件
        puppetLine.OnPuppetMechModeChange += OnPuppetMechModeChange;

        //  链式传递特效
        puppetLine.OnPuppetPlayEffect += OnPuppetPlayEffect;

    }

    void OnPuppetAttacking() 
    {
        G2C_PuppetFixedinformationStruct msg = new G2C_PuppetFixedinformationStruct();

        msg.UnitId = this.id;

        msg.OnPuppetAttacking = true;

        msg.PuppetAttacking = true;

        unitInfoManager.G2C_SendPuppetFixedInfo(msg.UnitId, msg);
    }

    void OnPuppetAttackFinish()
    {
        G2C_PuppetFixedinformationStruct msg = new G2C_PuppetFixedinformationStruct();

        msg.UnitId = this.id;

        msg.OnPuppetAttackFinish = true;

        msg.PuppetAttackFinish = true;

        unitInfoManager.G2C_SendPuppetFixedInfo(msg.UnitId, msg);

    }

    void OnPuppetShooting(bool info1, Vector3 info2)
    {
        G2C_PuppetFixedinformationStruct msg = new G2C_PuppetFixedinformationStruct();

        msg.UnitId = this.id;

        msg.OnPuppetShooting = true;

        msg.isPuppetShooting = info1;

        msg.PuppetShootingPos = info2;

        unitInfoManager.G2C_SendPuppetFixedInfo(msg.UnitId, msg);

    }

    void OnPuppetSide(UnitTemplate.UnitType info)
    {
        G2C_PuppetFixedinformationStruct msg = new G2C_PuppetFixedinformationStruct();

        msg.UnitId = this.id;

        msg.OnPuppetSide = true;

        msg.PuppetSide = info;

        unitInfoManager.G2C_SendPuppetFixedInfo(msg.UnitId, msg);

    }

    void OnPuppetStateChanged(Unit.UnitState info1, Vector3 info2)
    {
        G2C_PuppetFixedinformationStruct msg = new G2C_PuppetFixedinformationStruct();

        msg.UnitId = this.id;

        msg.OnPuppetStateChanged = true;

        msg.PuppetState1 = info1;

        msg.PuppetState2 = info2;

        unitInfoManager.G2C_SendPuppetFixedInfo(msg.UnitId, msg);

    }

    void OnPuppetTypeChanged(UnitTemplate.UnitType info)
    {
        G2C_PuppetFixedinformationStruct msg = new G2C_PuppetFixedinformationStruct();

        msg.UnitId = this.id;

        msg.OnPuppetTypeChanged = true;

        msg.PuppetTypeChanged = info;

        unitInfoManager.G2C_SendPuppetFixedInfo(msg.UnitId, msg);

    }

    void OnPuppetSkinChanged(string info)
    {
        G2C_PuppetFixedinformationStruct msg = new G2C_PuppetFixedinformationStruct();

        msg.UnitId = this.id;

        msg.OnPuppetSkinChanged = true;

        msg.PuppetSkinChanged = info;

        unitInfoManager.G2C_SendPuppetFixedInfo(msg.UnitId, msg);
    }

    void OnPuppetDestory(float info)
    {
        G2C_PuppetFixedinformationStruct msg = new G2C_PuppetFixedinformationStruct();

        msg.UnitId = this.id;

        msg.OnPuppetDestory = true;

        msg.PuppetDestory = info;

        unitInfoManager.G2C_SendPuppetFixedInfo(msg.UnitId, msg);

    }

    void OnPuppetChangeWeapon(WeaponTemplate.WeaponType info)
    {
        G2C_PuppetFixedinformationStruct msg = new G2C_PuppetFixedinformationStruct();

        msg.UnitId = this.id;

        msg.OnPuppetChangeWeapon = true;

        msg.PuppetChangeWeapon = info;

        unitInfoManager.G2C_SendPuppetFixedInfo(msg.UnitId, msg);

    }

    void OnPuppetPositionChange(Vector3 info) 
    {
        G2C_UnitInfoUpdateStruct msg = new G2C_UnitInfoUpdateStruct();

        msg.UnitId = this.id;

        msg.PosUpdate = true;

        msg.PuppetPosition = info;

        unitInfoManager.G2C_SendPuppetInfo(msg.UnitId, msg);
    }

    void OnPuppetScaleChanged(Vector3 info)
    {
        G2C_UnitInfoUpdateStruct msg = new G2C_UnitInfoUpdateStruct();

        msg.UnitId = this.id;

        msg.ScaleUpdate = true;

        msg.PuppetScale = info;

        unitInfoManager.G2C_SendPuppetInfo(msg.UnitId, msg);

    }

    void OnPuppetHealthChange(float info)
    {
        G2C_UnitInfoUpdateStruct msg = new G2C_UnitInfoUpdateStruct();

        msg.UnitId = this.id;

        msg.HealthUpdate = true;

        msg.PuppetHealth = info;

        unitInfoManager.G2C_SendPuppetInfo(msg.UnitId, msg);

    }

    void OnPuppetFilp(bool info)
    {
        G2C_UnitInfoUpdateStruct msg = new G2C_UnitInfoUpdateStruct();

        msg.UnitId = this.id;

        msg.FilpUpdate = true;

        msg.PuppetFilp = info;

        unitInfoManager.G2C_SendPuppetInfo(msg.UnitId, msg);

    }

    void OnPuppetSpeedChange(float info)
    {
        G2C_UnitInfoUpdateStruct msg = new G2C_UnitInfoUpdateStruct();

        msg.UnitId = this.id;

        msg.SpeedUpdate = true;

        msg.PuppetSpeed = info;

        unitInfoManager.G2C_SendPuppetInfo(msg.UnitId, msg);

    }

    void OnPuppetTargetOfAttackChange(Vector3 info)
    {
        G2C_UnitInfoUpdateStruct msg = new G2C_UnitInfoUpdateStruct();

        msg.UnitId = this.id;

        msg.AttackPosUpdate = true;

        msg.PuppetAttackPosition = info;

        unitInfoManager.G2C_SendPuppetInfo(msg.UnitId, msg);

    }

    void OnPuppetMechModeChange(MechanismInPut.ModeTest info) 
    {
        
    }

    void OnPuppetPlayEffect(PuppetEffectDataStruct info) 
    {
        
    }
}