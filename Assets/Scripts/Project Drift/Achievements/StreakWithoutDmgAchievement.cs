using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Achievements;

[CreateAssetMenu(fileName = "Streak without Damage Achievement", menuName = "Project Drift/Achievements/Streak without Damage Achievement")]
public class StreakWithoutDmgAchievement : BaseAchievement
{
    public int streakRequired;
    public int indicateProgressInterval = 1;
    private int currStreak = 0;
    public override void Initialize()
    {
        EventBus.Subscribe<EnemyDefeatedEvent>(OnEnemyDefeat);
        EventBus.Subscribe<PlayerDamagedEvent>(OnTakeDamage);
    }

    void OnEnemyDefeat(EnemyDefeatedEvent e)
    {
        currStreak += 1;
        if(currStreak >= streakRequired)
        {
            Unlock();
        }
        else if(currStreak%indicateProgressInterval == 0)
        {
            IndicateAchievementProgress(currStreak, streakRequired);
        }
    }

    void OnTakeDamage(PlayerDamagedEvent e)
    {
        currStreak = 0;
    }
}
