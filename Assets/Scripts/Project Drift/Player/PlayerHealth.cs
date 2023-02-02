using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WSoft.Combat;

/// <summary>
/// An overridden version of a health system designed for the player's abilities
/// Written by Nikhil Ghosh
/// </summary>
public class PlayerHealth : HealthSystem
{
    [System.Serializable]
    public class IframeDurationEvent : UnityEvent<float> { }

    [Tooltip("The maximum amount of health. Also the starting amount")]
    public int maxHealth;

    [Tooltip("The current amount of health.")]
    public int currentHealth;

    [Tooltip("The maximum amount of shield. Also the starting amount")]
    public int maxShield;

    [Tooltip("The current amount of shield.")]
    public int currentShield;

    [Tooltip("Are invincibility frames enabled when the health takes damage")]
    public bool iframesEnabled = false;

    [Tooltip("The duration of the iFrames, if iFrames are enabled")]
    public float iframesDuration = 1f;

    // The time at which the iFrames end, in Unity time
    private float iframesEnd = -1f;

    [Tooltip("Invoked when iframes block damage. Passed the duration of iframes left.")]
    public IframeDurationEvent OnIframes;

    [Tooltip("Invoked when the player's health is upgraded")]
    public UnityEvent OnHealthUpgrade;

    [Tooltip("Invoked when the player's shield is regenerated")]
    public UnityEvent OnShieldRegen;

    private List<Coroutine> shieldDelayCoroutines = new List<Coroutine>();
    private bool shieldBroken;

    protected override void Initialize()
    {
        base.Initialize();

        current = maxHealth + maxShield;
        currentHealth = maxHealth;
        currentShield = maxShield;
    }

    protected override bool ApplyDamage(int amount, object obj = null)
    {
        if (iframesEnabled)
        {
            // Black damage if the iframes are stil active
            if (Time.time < iframesEnd)
            {
                OnIframes.Invoke(iframesEnd - Time.time);
                return false;
            }

            iframesEnd = Time.time + iframesDuration;
        }

        if(currentShield > 0) { // Shield remaining -> apply damage to shield
            currentShield -= Mathf.Min(currentShield, amount);
        } else {
            currentHealth -= Mathf.Min(currentHealth, amount);
        }

        current = currentHealth + currentShield;

        // Shield break
        if(currentShield <= 0) {
            shieldBroken = true;
        }

        // Delay shield regeneration
        if(shieldBroken) {
            shieldDelayCoroutines.Add(StartCoroutine(ShieldDelay(5f)));
            if(shieldDelayCoroutines.Count > 1) {
                StopCoroutine(shieldDelayCoroutines[0]);
                shieldDelayCoroutines.RemoveAt(0);
            }
        }

        if (current <= 0)
        {
            current = 0;
            Die();
        }

        EventBus.Publish(new PlayerDamagedEvent(amount));

        return true;
    }

    protected override bool ApplyHeal(int amount, object obj = null)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }

        current = currentHealth + currentShield;

        return true;
    }

    public void UpgradeHealth(int healthAmount)
    {
        maxHealth += healthAmount;
        currentHealth = maxHealth;
        current = currentHealth + currentShield;

        OnHealthUpgrade.Invoke();
    }

    public override string GetDebugData()
    {
        return "Current: " + current + "/" + (maxHealth + maxShield) +
            "\n" + (Time.time < iframesEnd ? "iFrames active" : "iFrames inactive");
    }

    IEnumerator ShieldDelay(float delay) {
        yield return new WaitForSeconds(delay);
        shieldBroken = false;
        // Start regening shield
        StartCoroutine(ShieldRegen(1f));
    }

    IEnumerator ShieldRegen(float regenTime) {
        while(currentShield < maxShield) {
            currentShield += 1;
            OnShieldRegen.Invoke();
            if(currentShield > maxShield) {
                currentShield = maxShield;
            }
            yield return new WaitForSeconds(regenTime);
        }
    }

}
