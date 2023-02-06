using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WSoft.Combat;

/// <summary>
/// A player subcontroller that handles the attack state of the player
/// </summary>
[System.Serializable]
public class PlayerAttackController : PlayerSubController
{
    public float groundSpeed;
    public GameObject attackRoot;
    public float destructibleRechargePercentage;
    public GameObject DefaultWeapon;

    GroundCharacterController characterController;
    PlayerInventoryManager inventoryManager;

    bool keyPressed = false;
    float timer = 0f;

    public override void Initialize()
    {
        characterController = playerController.GetComponent<GroundCharacterController>();
        inventoryManager = playerController.GetComponent<PlayerInventoryManager>();

        inventoryManager.EquiptMeleeWeapon(DefaultWeapon);
    }

    public override void OnDisable()
    {
        
    }

    public override void OnEnable()
    {
        attackRoot.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, playerController.aimInput));
    }

    public override void Update()
    {
        characterController.velocity = playerController.movementInput * groundSpeed;

        if (keyPressed) timer += Time.unscaledDeltaTime;
    }

    public override void RecieveInput(PlayerInputType type)
    {
        if (type == PlayerInputType.MeleePressed)
        {
            keyPressed = true;
        }

        if (type == PlayerInputType.MeleeReleased)
        {
            EventBus.Publish(new AbilityInterruptEvent());

            attackRoot.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, playerController.aimInput));

            if (timer < PlayerInventoryManager.Instance.MeleeWeapon.ChargeTime)
            {
                inventoryManager.MeleeWeapon.NormalAttack();
            }
            else
            {
                inventoryManager.MeleeWeapon.ChargedAttack();
            }

            keyPressed = false;
            timer = 0f;
            playerController.SetController(playerController.groundController);
        }
    }
}
