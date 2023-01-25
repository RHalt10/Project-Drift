using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Achievements;

[CreateAssetMenu(fileName = "New Enemies Killed Achievement", menuName = "Achievements/Enemies Killed")]
public class EnemiesKilledAchievement : BaseAchievement
{
    public string enemyKey;
    public int numEnemiesToKill;
    public int indicateProgressInterval = 1;

    public override void Initialize()
    {
        EventBus.Subscribe<EnemyDefeatedEvent>(OnEnemyKilled);
    }

    void OnEnemyKilled(EnemyDefeatedEvent e)
    {
        if (!ProgressionManager.instance.enemyTypesKilled.ContainsKey(enemyKey))
            return;

        int totalEnemiesKilled = ProgressionManager.instance.enemyTypesKilled[enemyKey];

        if (totalEnemiesKilled >= numEnemiesToKill)
        {
            Unlock();
        }
        else if (totalEnemiesKilled % indicateProgressInterval == 0 && totalEnemiesKilled != 0)
        {
            IndicateAchievementProgress(totalEnemiesKilled, numEnemiesToKill);
        }
    }

}
