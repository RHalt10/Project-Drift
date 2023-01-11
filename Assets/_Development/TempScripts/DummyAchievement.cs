using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Achievements;

[CreateAssetMenu(fileName = "New Dummy Achievement", menuName = "Project Drift/Dummy Achievement")]
public class DummyAchievement : BaseAchievement
{
    public override void Initialize()
    {
        AchievementsManager.instance.StartCoroutine(TempCoroutine());
    }

    void TempIndicateProgress()
    {
        IndicateAchievementProgress(1, 2);
    }

    void TempUnlock()
    {
        Unlock();
    }

    IEnumerator TempCoroutine()
    {
        yield return new WaitForSeconds(2);

        TempIndicateProgress();

        yield return new WaitForSeconds(10);

        TempUnlock();
    }
}
