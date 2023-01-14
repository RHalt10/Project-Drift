using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;

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
