using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A player subcontroller that deals with aiming the player's gun.
/// Written by Nikhil Ghosh '24
/// </summary>
[System.Serializable]
public class PlayerAimController : PlayerSubController
{
    public Transform aimObject;

    public AK.Wwise.Event startAimSfx;
    public AK.Wwise.Event stopAimSfx;

    PlayerGun playerGun;

    public override void Initialize()
    {
        aimObject.gameObject.SetActive(false);
        playerGun = playerController.GetComponent<PlayerGun>();
    }

    public override void OnDisable()
    {
        aimObject.gameObject.SetActive(false);
        stopAimSfx.Post(playerController.gameObject);
    }

    public override void OnEnable()
    {
        aimObject.gameObject.SetActive(true);
        startAimSfx.Post(playerController.gameObject);
    }

    public override void RecieveInput(PlayerInputType type)
    {
        if (type == PlayerInputType.StopAim)
            playerController.SetController(playerController.groundController);
        else if (type == PlayerInputType.Shoot)
            playerGun.Shoot(playerController.aimInput);
        else if (type == PlayerInputType.SwapRanged)
            playerGun.SwapWeapons();
    }

    public override void Update()
    {
        float angle = Vector2.SignedAngle(Vector2.up, playerController.aimInput);
        aimObject.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
