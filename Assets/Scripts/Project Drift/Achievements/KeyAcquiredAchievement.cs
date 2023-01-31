using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using WSoft.Achievements;


[CreateAssetMenu(fileName = "New Key Acquired Achievement", menuName = "Project Drift/Achievements/Key Acquired Achievement")]
public class KeyAcquiredAchievement : BaseAchievement
{
    public int numKeys;

    public override void Initialize()
    {
        EventBus.Subscribe<KeyAcquiredEvent>(OnKeyAcquired);
    }

    void OnKeyAcquired(KeyAcquiredEvent e)
    {
        int totalAcquiredKeys = PlayerInventoryManager.Instance.keys;
        if (totalAcquiredKeys >= numKeys) { Unlock(); }
    }

}
