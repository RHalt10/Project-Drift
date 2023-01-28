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

    float cooldownTimer = 0f;
    bool taskdone = true;

    protected override void Awake()
    {
        base.Awake(); // Call the parent's awake function first
        HammerTrigger.OnDamageCaused.AddListener(HammerKnockback);
        HammerTrigger.OnDamageCaused.AddListener(Stun);
        AOETrigger.OnDamageCaused.AddListener(AOEKnockback);
    }

    private void Update()
    {
       if(taskdone) cooldownTimer += Time.unscaledDeltaTime;
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
        if (!taskdone || cooldownTimer < ChargedAttackCooldown) yield break;
        cooldownTimer = 0;
        taskdone = false;

        Debug.Log("Hammer ChargedAttack()");
        yield return new WaitForSecondsRealtime(0.5f);  

        taskdone = true;
    }

    protected override IEnumerator NormalAttackRoutine()
    {
        if (!taskdone || cooldownTimer < NormalAttackCooldown) yield break;
        cooldownTimer = 0;
        taskdone = false;

        Debug.Log("Hammer NormalAttack()");
        yield return new WaitForSecondsRealtime(0.5f);

        taskdone = true;
    }
}
