using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Slow Debuff", menuName = "Project Drift/Buffs/SlowDebuff")]
public class SlowDebuffSO : BuffSO
{
    [SerializeField] private float slowMultiplier = 0.5f;

    // Example effects to apply.
    public Color slowedColor;
    private Color _startColor;

    private float _startMultiplier = 1;
    private GroundCharacterController character;

    public override void ActivateBuff(GameObject charToBuff)
    {
        character = charToBuff.GetComponent<GroundCharacterController>();
        character.velocityMultiplier *= slowMultiplier;
        
        // Example slowed visual effect.
        if(slowedColor != null)
        {
            _startColor = charToBuff.GetComponent<SpriteRenderer>().color;
            charToBuff.GetComponent<SpriteRenderer>().color = slowedColor;
        }
    }

    public override void DeactivateBuff(GameObject charToBuff)
    { 
        character = charToBuff.GetComponent<GroundCharacterController>();
        character.velocityMultiplier = _startMultiplier;

        // Example slowed visual effect. Deactivate on buff deactivate.
        charToBuff.GetComponent<SpriteRenderer>().color = _startColor;
    }
}
