using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

/// <summary>
/// A player subcontroller that handles the shooting state of player. 
/// Player cannot move during shooting state.
/// </summary>
public class PlayerShootController : PlayerSubController
{
    GroundCharacterController characterController;
    PlayerGun playerGun;
    float timer = 0;
    float equippedCdTime;
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
        playerGun.Shoot(playerController.aimInput);
        characterController.velocity = Vector2.zero;
        if(playerGun.currentWeapon != null) equippedCdTime = playerGun.currentWeapon.cdTime;
        timer = 0;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if(timer >= equippedCdTime) { playerController.SetController(playerController.groundController); }
    }

    public override void RecieveInput(PlayerInputType type)
    {
        if (type == PlayerInputType.Shoot)
        {
            if(timer >= equippedCdTime)
                playerGun.Shoot(playerController.aimInput);
        }
    }
}
