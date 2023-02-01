using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerShootController : PlayerSubController
{
    GroundCharacterController characterController;
    PlayerGun playerGun;
    float timer = 0;
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
        timer = 0;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if(timer >= playerGun.currentWeapon.cdTime) { playerController.SetController(playerController.groundController); }
    }

    public override void RecieveInput(PlayerInputType type)
    {
        if (type == PlayerInputType.Shoot)
        {
            if(timer >= playerGun.currentWeapon.cdTime)
                playerGun.Shoot(playerController.aimInput);
        }
    }
}
