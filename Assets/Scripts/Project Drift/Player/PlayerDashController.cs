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
    public float dashCooldown;
    public float dashStaminaCost;
    public bool chainDashAllowed;
    public float chainDashTimeWindow;
    public float chainDashStaminaCost;
    public bool useMovementInput = false;

    GroundCharacterController characterController;
    PlayerStamina playerStamina;
    Vector2 dashDirection;
    float currentDashTime = 0;
    bool chainDashActivated;

    bool CanChainDash => characterController.isOnGround;

    public override void Initialize()
    {
        characterController = playerController.GetComponent<GroundCharacterController>();
        playerStamina = playerController.GetComponent<PlayerStamina>();
    }

    public override void OnDisable()
    {
        characterController.canMoveOnAir = false;
        characterController.willFallOnAir = true;
        characterController.velocity = Vector2.zero;
    }

    public override void OnEnable()
    {
        EventBus.Publish(new AbilityInterruptEvent());

        dashDirection = useMovementInput ? playerController.movementInput : playerController.aimInput;
        currentDashTime = 0;
        characterController.canMoveOnAir = true;
        characterController.willFallOnAir = false;
        chainDashActivated = false;
        playerStamina.UseStamina(dashStaminaCost);

        if (dashDirection != Vector2.zero)
            AkSoundEngine.PostEvent("PlayerDash_Play", playerController.gameObject);
    }

    public override void Update()
    {
        currentDashTime += Time.deltaTime;

        // If we are still dashing, keep setting the velocity;
        if (currentDashTime < dashTime)
            characterController.velocity = dashSpeed * dashDirection;
        else
        {
            // If we are over the dash time, we need to switch to walking
            characterController.velocity = playerController.groundController.groundSpeed * playerController.movementInput;
            // If we can chain dash and have pressed the buttons to do so, chain dash
            if (chainDashActivated && chainDashAllowed && CanChainDash)
                ChainDash();

            // If we've exceeded the cooldown, do the dash window
            if (currentDashTime >= dashTime + dashCooldown)
                playerController.SetController(playerController.groundController);
        }
    }

    public override void RecieveInput(PlayerInputType type)
    {
        if (type == PlayerInputType.Dash)
        {
            if (currentDashTime > dashTime)
            {
                float currentTimeInCooldown = currentDashTime - dashTime;
                if (currentTimeInCooldown < chainDashTimeWindow && chainDashAllowed)
                    chainDashActivated = true;
            }
        }
    }

    void ChainDash()
    {
        EventBus.Publish(new AbilityInterruptEvent());

        dashDirection = useMovementInput ? playerController.movementInput : playerController.aimInput;
        currentDashTime = 0;
        chainDashActivated = false;
        playerStamina.UseStamina(chainDashStaminaCost);
        AkSoundEngine.PostEvent("PlayerDash_Play", playerController.gameObject);
    }
}
