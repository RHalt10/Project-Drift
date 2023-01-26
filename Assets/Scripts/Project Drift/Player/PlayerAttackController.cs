using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WSoft.Combat;

/// <summary>
/// A struct with data about a player's attack. Makes it easier to group items in the inspector
/// </summary>
[System.Serializable]
public class PlayerAttackData
{
    public string animationTrigger;
    public GameObject gameObject;
    public float attackDuration;
    public float startDamageTime;
    public float endDamageTime;
}

/// <summary>
/// A player subcontroller that handles the attack state of the player
/// </summary>
[System.Serializable]
public class PlayerAttackController : PlayerSubController
{
    public float groundSpeed;
    public GameObject attackRoot;
    public float destructibleRechargePercentage;

    public PlayerAttackData meleeAttack;
    public GameObject DefaultWeapon;

    GroundCharacterController characterController;
    Animator animator;
    PlayerGun playerGun;

    bool keyPressed = false;
    float timer = 0f;

    public override void Initialize()
    {
        characterController = playerController.GetComponent<GroundCharacterController>();
        animator = playerController.GetComponent<Animator>();
        playerGun = playerController.GetComponent<PlayerGun>();

        //InitializeAttack(meleeAttack);

        PlayerInventoryManager.Instance.EquiptMeleeWeapon(DefaultWeapon);
    }

    public override void OnDisable()
    {
        
    }

    public override void OnEnable()
    {
        attackRoot.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, playerController.aimInput));
        //playerController.StartCoroutine(PerformAttack(meleeAttack));

        
    }

    public override void Update()
    {
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
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            attackRoot.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(mouseWorldPosition.y - attackRoot.transform.position.y,
            mouseWorldPosition.x - attackRoot.transform.position.x) * Mathf.Rad2Deg - 90);

            //attackRoot.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, playerController.aimInput));

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
            
    }

    IEnumerator PerformAttack(PlayerAttackData attackData)
    {
        EventBus.Publish(new PlayerAttackEvent(attackData));

        animator.SetTrigger(attackData.animationTrigger);
        AkSoundEngine.PostEvent("PlayerMelee_Play", PlayerController.Instance.gameObject);
        attackData.gameObject.SetActive(true);

        yield return new WaitForSeconds(attackData.startDamageTime);

        Collider2D collider = attackData.gameObject.GetComponent<Collider2D>();
        collider.enabled = true;

        yield return new WaitForSeconds(attackData.endDamageTime - attackData.startDamageTime);

        collider.enabled = false;

        yield return new WaitForSeconds(attackData.attackDuration - attackData.endDamageTime);

        attackData.gameObject.SetActive(false);

        playerController.SetController(playerController.groundController);

        
    }

    void InitializeAttack(PlayerAttackData attackData)
    {
        //attackData.gameObject.SetActive(false);
        //attackData.gameObject.GetComponentInChildren<DamageOnTrigger2D>().OnDamageCaused.AddListener(OnDamageCaused);
        
    }

    void OnDamageCaused(GameObject target)
    {
        if (target.GetComponent<EnemyData>() != null)
            playerGun.RechargeSingleAmmo();
        else
            playerGun.RechargeAmmo(destructibleRechargePercentage);
    }
}
