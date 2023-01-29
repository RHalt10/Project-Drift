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

    void Awake()
    {
        currentStamina = 1f;
        recharging = false;
    }

    public void UseStamina(float amount) {
        currentStamina -= amount;
        if(currentStamina < 0) {
            currentStamina = 0;
        }

        recharging = false;
        rechargeDelayCoroutines.Add(StartCoroutine(RechargeDelay(10f)));
        if(rechargeDelayCoroutines.Count > 1) {
            StopCoroutine(rechargeDelayCoroutines[0]);
            rechargeDelayCoroutines.RemoveAt(0);
        }
    }

    IEnumerator RechargeDelay(float delay) {
        yield return new WaitForSeconds(delay);
        recharging = true;
        // Start recharging stamina
        StartCoroutine(Recharge());
    }

    IEnumerator Recharge() {
        while(recharging) {
            currentStamina += 0.1f;
            if(currentStamina >= 1f) {
                currentStamina = 1f;
                recharging = false;
            }
            yield return new WaitForSeconds(3f);
        }
    }


    
}
