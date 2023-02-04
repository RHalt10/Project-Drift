using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DebuffType {
    Slow,
    PLACEHOLD
}

[System.Serializable]
public class DebuffInfo {
    
    public float remainingDuration;
    public DebuffType debufftype;
}

[System.Serializable]
public abstract class TimedBuff {
    public float remainingDuration;
    public DebuffType debufftype;

}

public class CharacterDebuff : MonoBehaviour
{
    GroundCharacterController character;
    public Dictionary<DebuffType, float> currentDebuffs;
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<GroundCharacterController>();

    }

    public void ApplyDebuff(DebuffType debuff, float duration)
    {
        if(debuff == DebuffType.Slow)
        {
            StartCoroutine(ApplySlowDebuffCR(duration));
        }
    }

    private IEnumerator ApplySlowDebuffCR(float duration)
    {
        float startMultiplier = character.velocityMultiplier;
        character.velocityMultiplier *= 0.5f;
        yield return new WaitForSeconds(duration);
        character.velocityMultiplier = startMultiplier;
    }
}
