using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Achievements;

[CreateAssetMenu(fileName = "Streak Without Shooting Achievement", menuName = "Project Drift/StreakWithoutShootingAchievement")]
public class StreakWithoutShootingAchievement : BaseAchievement
{
    public int EnemyKilledStreakRequired;
    public int indicateProgressInterval = 1;
    int currStreak = 0;

    Subscription<EnemyDefeatedEvent> enemy_defeated_event; 
    Subscription<PlayerShootEvent> player_shoot_event;
    public override void Initialize()
    {
        enemy_defeated_event = EventBus.Subscribe<EnemyDefeatedEvent>(OnEnemyDefeat);
        player_shoot_event = EventBus.Subscribe<PlayerShootEvent>(OnPlayerShoot);
    }

    void OnEnemyDefeat(EnemyDefeatedEvent e)
    {
        currStreak += 1;
        if(currStreak >= EnemyKilledStreakRequired)
        {
            Unlock();
        }
        else if(currStreak%indicateProgressInterval == 0)
        {
            IndicateAchievementProgress(currStreak, EnemyKilledStreakRequired);
        }
    }

    void OnPlayerShoot(PlayerShootEvent e)
    {
        currStreak = 0;
    }
}
