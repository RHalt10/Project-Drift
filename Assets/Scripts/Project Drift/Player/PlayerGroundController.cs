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
    }

    public override void OnDisable()
    {
        
    }

    public override void OnEnable()
    {

    }

    public override void Update()
    {
        if(movementEnabled) {
            characterController.velocity = playerController.movementInput * groundSpeed;
        } else {
            characterController.velocity = Vector2.zero;
        }
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
                playerGun.Shoot(playerController.aimInput);
                playerController.mono.StartCoroutine(DisableMovement(playerGun.currentWeapon.cdTime));
                return;
        }
    }

    private IEnumerator DisableMovement(float seconds)
    {
        movementEnabled = false;
        yield return new WaitForSeconds(seconds);
        movementEnabled = true;
    }
}
