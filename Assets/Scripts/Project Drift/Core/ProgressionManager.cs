using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public Dictionary<string, int> enemyTypesKilled = new Dictionary<string, int>();
    public Dictionary<string, int> weaponsKilledWith = new Dictionary<string, int>();
    public int totalEnemiesKilled = 0;

    public static ProgressionManager instance { get; private set; }

    void Awake()
    {
        instance = this;

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
