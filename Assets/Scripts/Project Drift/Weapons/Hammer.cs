using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;

public class Hammer : MeleeWeaponBase
{
    [Header("Hammer Basic Configuration")]
    [SerializeField] float StaminaCost;

    [Header("Hammer Knockback Configuration")]
    [SerializeField] DamageOnTrigger2D HammerTrigger;
    [SerializeField] DamageOnTrigger2D AOETrigger;
    [SerializeField] float HammerKnockbackForce;
    [SerializeField] float AOEKnockbackForce;

    [Header("Hammer Stun Configuraiton")]
    [SerializeField] float StunSeconds;
    protected override void Awake()
    {
        base.Awake(); // Call the parent's awake function first
        HammerTrigger.OnDamageCaused.AddListener(HammerKnockback);
        HammerTrigger.OnDamageCaused.AddListener(Stun);
        AOETrigger.OnDamageCaused.AddListener(AOEKnockback);
    }

    public override void ApplyCost(bool isNormalAttack)
    {
        if (isNormalAttack)
        {
            Debug.Log("Hammer Normal Attack ApplyCost()");
        }
        else
        {
            Debug.Log("Hammer Charged Attack ApplyCost()");
        }
        
    }

    public override void ApplyGain(GameObject target)
    {
        if (target.GetComponent<EnemyData>() != null)
            playerGun.RechargeAmmo(normalRechargePercentage);
        else
            playerGun.RechargeAmmo(destructibleRechargePercentage);
    }

    void HammerKnockback(GameObject target)
    {
        if (currentAttackType == AttackType.Normal)
        {
            Vector3 Force = HammerKnockbackForce * Vector3.Normalize(target.transform.position - transform.position); 
            Debug.Log("Hammer Knockback on " + target);
            EventBus.Publish(new KnockbackEvent(Force, target));
        }
    }

    void AOEKnockback(GameObject target)
    {
        if (currentAttackType == AttackType.Normal)
        {
            Vector3 Force = AOEKnockbackForce * Vector3.Normalize(target.transform.position - transform.position);
            Debug.Log("Hammer Knockback on " + target);
            EventBus.Publish(new KnockbackEvent(Force, target));
        }
    }

    void Stun(GameObject target)
    {
        if (currentAttackType == AttackType.Charged) Debug.Log("Stun on " + target);
        if (currentAttackType == AttackType.Charged) EventBus.Publish(new StunEvent(StunSeconds, target));
    }


    protected override IEnumerator ChargedAttackRoutine()
    {
        Debug.Log("Hammer ChargedAttack()");
        yield return new WaitForSecondsRealtime(0.5f);  
    }

    protected override IEnumerator NormalAttackRoutine(int ComboCount)
    {
        Debug.Log("[START] Hammer NormalAttack(), Combo: " + ComboCount);
        yield return new WaitForSecondsRealtime(3f);
        Debug.Log("[END] Hammer NormalAttack(), Combo: " + ComboCount);
    }
}
