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

    public override void Initialize()
    {
        characterController = playerController.GetComponent<GroundCharacterController>();
        playerGun = playerController.GetComponent<PlayerGun>();
    }

    public override void OnDisable()
    {
        
    }

    public override void OnEnable()
    {

    }

    public override void Update()
    {
        characterController.velocity = playerController.movementInput * groundSpeed;
    }

    public override void RecieveInput(PlayerInputType type)
    {
        switch(type)
        {
            case PlayerInputType.Dash:
                playerController.SetController(playerController.dashController);
                return;
            case PlayerInputType.Melee:
                playerController.SetController(playerController.attackController);
                return;
            case PlayerInputType.StartAim:
                playerController.SetController(playerController.aimController);
                return;
            case PlayerInputType.Shoot:
                playerGun.Shoot(playerController.aimInput);
                return;
        }
    }
}
