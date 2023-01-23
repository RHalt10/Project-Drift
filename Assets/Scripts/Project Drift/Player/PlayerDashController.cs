using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A player subcontroller that control the dash ability
/// </summary>
[System.Serializable]
public class PlayerDashController : PlayerSubController
{
    public float dashSpeed;
    public float dashTime;

    GroundCharacterController characterController;
    Vector2 dashDirection;
    float currentDashTime = 0;

    public override void Initialize()
    {
        characterController = playerController.GetComponent<GroundCharacterController>();
    }

    public override void OnDisable()
    {
        characterController.canMoveOnAir = false;
        characterController.willFallOnAir = true;
        characterController.velocity = Vector2.zero;
    }

    public override void OnEnable()
    {
        dashDirection = playerController.movementInput;
        currentDashTime = 0;
        characterController.canMoveOnAir = true;
        characterController.willFallOnAir = false;

        if (dashDirection != Vector2.zero)
            AkSoundEngine.PostEvent("PlayerDash_Play", PlayerController.Instance.gameObject);
    }

    public override void Update()
    {
        characterController.velocity = dashSpeed * dashDirection;

        currentDashTime += Time.deltaTime;
        if (currentDashTime >= dashTime)
            playerController.SetController(playerController.groundController);
    }

    public override void RecieveInput(PlayerInputType type)
    {
        
    }
}
