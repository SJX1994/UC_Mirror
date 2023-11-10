using UnityEngine;
using System.Linq;
using System.Collections;
using DG.Tweening;
using Mirror;
public class UnitAttackProp : NetworkBehaviour
{
#region 数据对象
    UnitSimple unitSimple;
    public UnitSimple UnitSimple
    {
        get
        {
            if(unitSimple)return unitSimple;
            unitSimple = GetComponent<UnitSimple>();
            return unitSimple;
        }
    }
    private TetriAttackable_Attribute[] attackableTargets;
    public TetriAttackable_Attribute targetPropOfAttack;
    Vector3 endPoint;
    public float lastGuardCheckTime, guardCheckInterval = 1f;
#endregion 数据对象

#region 数据操作
    public TetriAttackable_Attribute AttackChecker()
    {
        if (transform.tag == "Untagged") return null;
        if (Time.time < lastGuardCheckTime + guardCheckInterval) return null;
        lastGuardCheckTime = Time.time;
        return GetNearestAttackPropUnit();
    }
    public void Attack(TetriAttackable_Attribute target)
    {   
        if (!target)return;       
        if (IsDeadOrNull(target))return;
        targetPropOfAttack = target;
        if (UnitSimple.tween_running != null) UnitSimple.tween_running.Kill();
        UnitSimple.animator.speed = Random.Range(0.95f, 1.15f);
        if(Local())
        {
            StartCoroutine(DealAttackSimple());
        }else
        {
            StartCoroutine(Server_DealAttackSimple());
        }
    }
    public virtual IEnumerator DealAttackSimple()
    {
        while (targetPropOfAttack != null)
        {
            if(!UnitSimple.animator)break;
            UnitSimple.RunEffect(EffectTemplate.EffectType.Attacking);
            UnitSimple.animator.SetTrigger("DoAttack");
            switch(UnitSimple.Weapon.thisWeaponType)
            {
                case WeaponTemplate.WeaponType.Bow:
                    UnitSimple.Sound_Attack_Remote();
                    break;
                case WeaponTemplate.WeaponType.Sword:
                    UnitSimple.Sound_Attack_Melee();
                    break;
                case WeaponTemplate.WeaponType.Spear:
                    UnitSimple.Sound_Attack_Mid();
                    break;
            }
            
            UnitSimple.OnAttacking?.Invoke();
            UnitSimple.WeaponDisplay();
            targetPropOfAttack.SufferAttackSimple(UnitSimple.unitTemplate.attackPower,this);
            yield return new WaitForSeconds(1f / UnitSimple.unitTemplate.attackSpeed);
            targetPropOfAttack = null;
            if (IsDeadOrNull(targetPropOfAttack))
            {
                UnitSimple.animator.SetTrigger("InterruptAttack");
                UnitSimple.OnAttackFinish?.Invoke();
                break;
            }
            if (UnitSimple.state == UnitSimple.UnitState.Dead)
            {
                yield break;
            }
        }
        if (UnitSimple.state == UnitSimple.UnitState.Attacking)
        {
            UnitSimple.GuardSimple();
        }
    }
    [Server]
    public virtual IEnumerator Server_DealAttackSimple()
    {
        while (targetPropOfAttack != null)
        {
            if(!UnitSimple.animator)break;
            UnitSimple.RunEffect(EffectTemplate.EffectType.Attacking);
            UnitSimple.animator.SetTrigger("DoAttack");
            switch(UnitSimple.Weapon.thisWeaponType)
            {
                case WeaponTemplate.WeaponType.Bow:
                    UnitSimple.Sound_Attack_Remote();
                    UnitSimple.Client_Sound_Attack_Remote();
                    break;
                case WeaponTemplate.WeaponType.Sword:
                    UnitSimple.Sound_Attack_Melee();
                    UnitSimple.Client_Sound_Attack_Melee();
                    break;
                case WeaponTemplate.WeaponType.Spear:
                    UnitSimple.Sound_Attack_Mid();
                    UnitSimple.Client_Sound_Attack_Mid();
                    break;
            }
            UnitSimple.Client_DealAttackSimple_DoAttackAnimation();
            UnitSimple.OnAttacking?.Invoke();
            UnitSimple.WeaponDisplay();
            targetPropOfAttack.SufferAttackSimple(UnitSimple.unitTemplate.attackPower,this);
            yield return new WaitForSeconds(1f / UnitSimple.unitTemplate.attackSpeed);
            targetPropOfAttack = null;
            if (IsDeadOrNull(targetPropOfAttack))
            {
                UnitSimple.animator.SetTrigger("InterruptAttack");
                UnitSimple.Client_DealAttackSimple_InterruptAttackAnimation();
                UnitSimple.OnAttackFinish?.Invoke();
                break;
            }
            if (UnitSimple.state == UnitSimple.UnitState.Dead)
            {
                yield break;
            }
        }
        if (UnitSimple.state == UnitSimple.UnitState.Attacking)
        {
            UnitSimple.GuardSimple();
        }
    }
    TetriAttackable_Attribute GetNearestAttackPropUnit()
    {
        attackableTargets = GameObject.FindObjectsOfType(typeof(TetriAttackable_Attribute)) as TetriAttackable_Attribute[];
        // Debug.Log("attackableTargets.Length"+attackableTargets.Length);
        if (attackableTargets == null || attackableTargets.Length == 0)return null;
        TetriAttackable_Attribute nearestAttackableProp = null;
        float nearestAttackableDistance = 1000f;
        for (int i = 0; i < attackableTargets.Count(); i++)
        {
            if (IsDeadOrNull(attackableTargets[i]))continue;
            float distanceFromHostile = Vector3.Distance(attackableTargets[i].transform.position, transform.position);
            if (distanceFromHostile <= UnitSimple.unitTemplate.guardDistance)
            {
                if (distanceFromHostile < nearestAttackableDistance)
                {
                    nearestAttackableProp = attackableTargets[i];
                    nearestAttackableDistance = distanceFromHostile;
                }
            }
        }
        return nearestAttackableProp;
    }
    public bool IsDeadOrNull(TetriAttackable_Attribute u)
    {
        return (u == null || u.state == TetriAttackable_Attribute.State.Dead);
    }
#endregion 数据操作
#region 联网数据操作
    bool Local()
    {
        return UnitSimple.Local();
    }
#endregion 联网数据操作
}