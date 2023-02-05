using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A player subcontroller that processes walking around.
/// Also acts as the central state that process a lot of input
/// </summary>
[System.Serializable]
public class PlayerGroundController : PlayerSubController
{
    public float groundSpeed;

    GroundCharacterController characterController;
    PlayerGun playerGun;
    bool movementEnabled = true;

    public override void Initialize()
    {
        characterController = playerController.GetComponent<GroundCharacterController>();
        playerGun = playerController.GetComponent<PlayerGun>();

        characterController.isPlayer = true;

    }

    public override void OnDisable()
    {
        
    }

    public override void OnEnable()
    {

    }

    public override void Update()
    {
        characterController.isPlayer = true;
        characterController.velocity = playerController.movementInput * groundSpeed;
    }

    public override void RecieveInput(PlayerInputType type)
    {
        switch(type)
        {
            case PlayerInputType.Dash:
                playerController.SetController(playerController.dashController);
                return;
            case PlayerInputType.MeleePressed:
                playerController.SetController(playerController.attackController);
                return;
            case PlayerInputType.StartAim:
                playerController.SetController(playerController.aimController);
                return;
            case PlayerInputType.Shoot:
                playerController.SetController(playerController.shootController);
                return;
            case PlayerInputType.SwapRanged:
                playerGun.SwapWeapons();
                return;
            case PlayerInputType.Interact:
                if(playerController.currentController == playerController.groundController) {
                    playerController.SetController(playerController.interactController);
                }
                return;
        }
    }
}
