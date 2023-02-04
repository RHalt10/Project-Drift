using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Slow Debuff", menuName = "Project Drift/Buffs/SlowDebuff")]
public class SlowDebuffSO : BuffSO
{
    private float startMultiplier = 1;
    private GroundCharacterController character;

    public override void Initialize()
    {
        
    }

    public override void ActivateBuff(GameObject buffed)
    {
        character = buffed.GetComponent<GroundCharacterController>();
        character.velocityMultiplier *= 0.5f;
    }

    public override void DeactivateBuff(GameObject buffed)
    { 
        character.velocityMultiplier = startMultiplier;
    }
}
