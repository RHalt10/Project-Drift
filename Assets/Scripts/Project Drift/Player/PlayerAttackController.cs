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
    Animator animator;
    PlayerGun playerGun;

    bool movementEnabled = true;
    bool keyPressed = false;
    float timer = 0f;

    public override void Initialize()
    {
        characterController = playerController.GetComponent<GroundCharacterController>();
        animator = playerController.GetComponent<Animator>();
        playerGun = playerController.GetComponent<PlayerGun>();

        PlayerInventoryManager.Instance.EquiptMeleeWeapon(DefaultWeapon);
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
        if(movementEnabled)
            characterController.velocity = playerController.movementInput * groundSpeed;

        if (keyPressed) timer += Time.deltaTime;
    }

    public override void RecieveInput(PlayerInputType type)
    {
        if (type == PlayerInputType.MeleePressed)
        {
            keyPressed = true;
        }

        if (type == PlayerInputType.MeleeReleased)
        {
            attackRoot.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, playerController.aimInput));

            if (timer < PlayerInventoryManager.Instance.MeleeWeapon.ChargeTime)
            {
                PlayerInventoryManager.Instance.MeleeWeapon.NormalAttack();
            }
            else
            {
                PlayerInventoryManager.Instance.MeleeWeapon.ChargedAttack();
            }

            keyPressed = false;
            timer = 0f;
            playerController.SetController(playerController.groundController);
        }

        if (type == PlayerInputType.Shoot)
        {
            playerGun.Shoot(playerController.aimInput);
            playerController.mono.StartCoroutine(DisableMovement(playerGun.currentWeapon.cdTime));
        }
    }

    private IEnumerator DisableMovement(float seconds)
    {
        movementEnabled = false;
        characterController.velocity = Vector2.zero;
        yield return new WaitForSeconds(seconds);
        movementEnabled = true;
        playerController.SetController(playerController.groundController);
    }
}
