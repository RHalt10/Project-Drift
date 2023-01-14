using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    Dictionary<string, int> enemyTypesKilled = new Dictionary<string, int>();
    Dictionary<string, int> weaponsKilledWith = new Dictionary<string, int>();
    int totalEnemiesKilled = 0;

    void Awake()
    {
        EventBus.Subscribe<EnemyDefeatedEvent>(OnEnemyDefeated);
    }

    void OnEnemyDefeated(EnemyDefeatedEvent e)
    {
        if (!enemyTypesKilled.ContainsKey(e.enemyKey))
            enemyTypesKilled.Add(e.enemyKey, 1);
        else
            enemyTypesKilled[e.enemyKey] += 1;

        totalEnemiesKilled++;

        PlayerWeaponSO weapon = PlayerController.Instance.GetComponent<PlayerGun>().currentWeapon;

        if (!weaponsKilledWith.ContainsKey(weapon.weaponName))
            weaponsKilledWith.Add(weapon.weaponName, 1);
        else
            weaponsKilledWith[weapon.weaponName] += 1;
    }
}
