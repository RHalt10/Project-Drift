using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MeleeWeaponBase
{
    public float attackTime;

    [Header("Combo Configuration")]
    public float comboCooldown;
    public int comboAmount;

    bool taskOngoing;
    bool comboActivated;
    int currentComboAmount;
    float cooldownTimer;

    protected override void Awake()
    {
        base.Awake(); // Call the parent's awake function first
        cooldownTimer = comboCooldown;
    }

    private void Update()
    {
        if (!taskOngoing) cooldownTimer += Time.unscaledDeltaTime;
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
        if (cooldownTimer < comboCooldown) yield break;

        if (taskOngoing)
        {
            if (currentComboAmount < comboAmount)
            {
                currentComboAmount++;
                comboActivated = true;
            }

            yield return new WaitWhile(() => taskOngoing);
            yield break;
        }

        currentComboAmount = 1;
        do
        {
            taskOngoing = true;
            comboActivated = false;

            animator.SetTrigger("Normal Attack");

            yield return new WaitForSeconds(attackTime);

            taskOngoing = false;

        }
        while (comboActivated);

        currentComboAmount = 0;
    }
}
