using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAbilitySO : ScriptableObject
{
    protected PlayerController controller;
    protected PlayerStamina playerStamina;
    
    public float staminaCost;

    public virtual void Initialize(PlayerController _controller)
    {
        controller = _controller;
        playerStamina = controller.GetComponent<PlayerStamina>();
    }

    public virtual bool CanBeActivated()
    {
        return playerStamina.currentStamina >= staminaCost;
    }

    public abstract void Activate();
}
