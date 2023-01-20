using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;

/// <summary>
/// A generic script that exists on every enemy.
/// Ensures events get called as expected.
/// </summary>
public class EnemyData : MonoBehaviour
{
    public string enemyKey;

    void Awake()
    {
        HealthSystem health = GetComponent<HealthSystem>();
        health.events.OnDeath.AddListener(OnDeath);
    }

    void OnDeath()
    {
        EventBus.Publish(new EnemyDefeatedEvent(enemyKey));
    }
}
