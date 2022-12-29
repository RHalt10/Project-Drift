using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAimController : PlayerSubController
{
    public Transform aimObject;

    PlayerGun playerGun;

    public override void Initialize()
    {
        aimObject.gameObject.SetActive(false);
        playerGun = playerController.GetComponent<PlayerGun>();
    }

    public override void OnDisable()
    {
        aimObject.gameObject.SetActive(false);
    }

    public override void OnEnable()
    {
        aimObject.gameObject.SetActive(true);
    }

    public override void RecieveInput(PlayerInputType type)
    {
        if (type == PlayerInputType.StopAim)
            playerController.SetController(playerController.groundController);
        else if (type == PlayerInputType.Shoot)
            playerGun.Shoot(playerController.aimInput);
    }

    public override void Update()
    {
        float angle = Vector2.SignedAngle(Vector2.up, playerController.aimInput);
        aimObject.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
