using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A player subcontroller that controls interactions with interactable objects
/// </summary>
[System.Serializable]
public class PlayerInteractController : PlayerSubController
{

    GroundCharacterController characterController;

    public override void Initialize()
    {
        characterController = playerController.GetComponent<GroundCharacterController>();
    }

    public override void OnDisable()
    {
        
    }

    public override void OnEnable()
    {
        
    }

    public override void RecieveInput(PlayerInputType type)
    {
        if (type == PlayerInputType.Interact)
        {
            Interact(/*pass and call another function from target? pass target?*/);
        }
    }

    void Interact()
    {
        // Must be colliding with target?
        // Target must be interactable
        // Do interaction
    }
}
