using UnityEngine;

public class UnitInfoUpdateClass : MonoBehaviour
{
    public int UnitId;

    // 是否有相应的状态更新
    public bool PosUpdate;

    public bool ScaleUpdate;

    public bool HealthUpdate;

    public bool FilpUpdate;

    public bool SpeedUpdate;

    public bool AttackPosUpdate;

    // 更新的具体数值
    public Vector3 PuppetPosition;

    public Vector3 PuppetScale;

    public float PuppetHealth;

    public bool PuppetFilp;

    public float PuppetSpeed;

    public Vector3 PuppetAttackPosition;
}