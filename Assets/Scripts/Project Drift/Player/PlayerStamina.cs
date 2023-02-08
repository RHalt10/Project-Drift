using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStamina : MonoBehaviour
{
    public float currentStamina; // 0.0 to 1.0

    public UnityEvent OnStaminaAmountChanged;

    public bool recharging;

    private List<Coroutine> rechargeDelayCoroutines = new List<Coroutine>();
    Coroutine drainCoroutine = null;

    void Awake()
    {
        currentStamina = 1f;
        recharging = false;
    }

    public void UseStamina(float amount) 
    {
        currentStamina -= amount;
        if(currentStamina < 0) {
            currentStamina = 0;
        }

        OnStaminaAmountChanged.Invoke();

        recharging = false;
        rechargeDelayCoroutines.Add(StartCoroutine(RechargeDelay(10f)));
        if(rechargeDelayCoroutines.Count > 1) {
            StopCoroutine(rechargeDelayCoroutines[0]);
            rechargeDelayCoroutines.RemoveAt(0);
        }
    }

    // Duration stamina drain lasts (seconds), and rate of stamina drain (% in decimal/s)
    public void UseStaminaOverTime(float duration, float rate) 
    {
        drainCoroutine = StartCoroutine(StaminaOverTime(duration, rate));
    }

    public void StopStaminaDrain()
    {
        if (drainCoroutine != null)
        {
            StopCoroutine(drainCoroutine);
            drainCoroutine = null;
        }    
    }

    IEnumerator StaminaOverTime(float duration, float rate) 
    {
        float total = duration * rate;
        float v0 = currentStamina;
        float v1 = currentStamina - total;
        for (float t = 0f; t < duration; t += Time.deltaTime) {
            currentStamina = Mathf.Lerp(v0, v1, t / duration);
            if(currentStamina <= 0) {
                currentStamina = 0;
            }
            OnStaminaAmountChanged.Invoke();
            yield return null;
        }
        currentStamina = Mathf.Max(v1, 0);
    }

    IEnumerator RechargeDelay(float delay) 
    {
        yield return new WaitForSeconds(delay);
        recharging = true;
        // Start recharging stamina
        StartCoroutine(Recharge());
    }

    IEnumerator Recharge() 
    {
        while(recharging) 
        {
            currentStamina += 0.1f;
            if(currentStamina >= 1f) 
            {
                currentStamina = 1f;
                recharging = false;
            }

            OnStaminaAmountChanged.Invoke();

            yield return new WaitForSeconds(3f);
        }
    }
}
