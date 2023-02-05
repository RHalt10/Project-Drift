using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;

public class HealthDrop : MonoBehaviour
{
    public int healAmount;

    public void HealPlayer() {
        HealthSystem health = GameObject.FindWithTag("Player").GetComponent<HealthSystem>();
        health.Heal(healAmount);
        Destroy(gameObject);
    }
}
