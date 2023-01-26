using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MeleeWeaponBase
{
    [Header("Hammer Configuration")]
    [SerializeField] float HammerDamage;
    [SerializeField] float HandleDamage;
    [SerializeField] float AOEDamage;
    [SerializeField] float StaminaCost;
    [SerializeField] float AmmoGainOnEnemy;
    [SerializeField] float AmmoGainOnObject;

    public float cooldownTimer = 0f;
    bool taskdone = true;

    private void Update()
    {
       if(taskdone) cooldownTimer += Time.unscaledDeltaTime;
    }

    public override void ApplyCost(GameObject target)
    {
        Debug.Log("Hammer ApplyCost()");
    }

    public override void ApplyGain(GameObject target)
    {
        Debug.Log("Hammer ApplyGain()");
    }

    protected override IEnumerator ChargedAttackRoutine()
    {
        if (cooldownTimer < ChargedAttackCooldown) yield break;
        cooldownTimer = 0;
        taskdone = false;

        Debug.Log("Hammer ChargedAttack()");
        yield return new WaitForSecondsRealtime(0.5f);

        taskdone = true;
    }

    protected override IEnumerator NormalAttackRoutine()
    {
        if (cooldownTimer < NormalAttackCooldown) yield break;
        cooldownTimer = 0;
        taskdone = false;

        Debug.Log("Hammer NormalAttack()");
        yield return new WaitForSecondsRealtime(0.5f);

        taskdone = true;
    }
}
