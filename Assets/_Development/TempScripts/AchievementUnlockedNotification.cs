using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WSoft.Achievements;

public class AchievementUnlockedNotification : MonoBehaviour
{
    [SerializeField] TMP_Text actionText;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text progressText;

    // Start is called before the first frame update
    void Start()
    {
        EventBus.Subscribe<AchievementUnlockedEvent>(OnAchievementUnlocked);
        EventBus.Subscribe<AchievementProgressIndicatedEvent>(OnAchievementProgress);
        HideAchievementNotification();
    }

    void OnAchievementUnlocked(AchievementUnlockedEvent e)
    {
        gameObject.SetActive(true);

        actionText.text = "Achievement Unlocked";
        nameText.text = e.achievement.Name;
        progressText.text = "";

        Invoke("HideAchievementNotification", 8f);
    }

    void OnAchievementProgress(AchievementProgressIndicatedEvent e)
    {
        gameObject.SetActive(true);

        actionText.text = "Achievement Progress";
        nameText.text = e.achievement.Name;
        progressText.text = "(" + e.currentProgress + "/" + e.maxProgress + ")";

        Invoke("HideAchievementNotification", 8f);
    }

    void HideAchievementNotification()
    {
        gameObject.SetActive(false);
    }
}
