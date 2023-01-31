using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MeleeWeaponBase
{
    [Header("Spear Configuration")]
    [SerializeField] float attackTime;
    public override void ApplyCost(bool isNormalAttack)
    {
        
    }

    public override void ApplyGain(GameObject target)
    {
        if (target.GetComponent<EnemyData>() != null)
            playerGun.RechargeAmmo(normalRechargePercentage);
        else
            playerGun.RechargeAmmo(destructibleRechargePercentage);
    }

    protected override IEnumerator ChargedAttackRoutine()
    {
        yield break; 
    }

    protected override IEnumerator NormalAttackRoutine(int ComboCount)
    {
        animator.SetTrigger("Normal Attack");

        yield return new WaitForSeconds(attackTime);
    }

}
