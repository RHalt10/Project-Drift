using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MeleeWeaponBase
{
    [Header("Sword Configuration")]
    public float attackTime;

    protected override void Awake()
    {
        base.Awake(); // Call the parent's awake function first
    }

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
        /*
        switch (ComboCount)
        {
            case 1:
                // Combo 1 Stuff
                break;
            case 2:
                // Combo 1 Stuff
                break;

            // ... //

            default:
                break;
        }
        */

        // Outside if all combos do the same thing
        animator.SetTrigger("Normal Attack");

        yield return new WaitForSeconds(attackTime);

    }
}
