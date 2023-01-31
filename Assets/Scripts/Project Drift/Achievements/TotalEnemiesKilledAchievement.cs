using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Achievements;

[CreateAssetMenu(fileName = "New Total Enemies Killed", menuName = "Project Drift/Achievements/Total Enemies Achievement")]
public class TotalEnemiesKilledAchievement : BaseAchievement
{
    public int numEnemies;

    public override void Initialize()
    {
        EventBus.Subscribe<EnemyDefeatedEvent>(OnEnemyKilled);
    }

    void OnEnemyKilled(EnemyDefeatedEvent e)
    {
        int totalEnemies = ProgressionManager.instance.totalEnemiesKilled;
        if (totalEnemies >= numEnemies)
            Unlock();
    }

}
