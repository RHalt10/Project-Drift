using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    GroundCharacterController characterController;
    Animator animator;
    PlayerGun playerGun;

    public override void Initialize()
    {
        characterController = playerController.GetComponent<GroundCharacterController>();
        animator = playerController.GetComponent<Animator>();
        playerGun = playerController.GetComponent<PlayerGun>();

        InitializeAttack(meleeAttack);
    }

    public override void OnDisable()
    {
        
    }

    public override void OnEnable()
    {
        attackRoot.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, playerController.aimInput));
        playerController.StartCoroutine(PerformAttack(meleeAttack));

        
    }

    public override void Update()
    {
        characterController.velocity = playerController.movementInput * groundSpeed;
    }

    public override void RecieveInput(PlayerInputType type)
    {
        
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
        attackData.gameObject.SetActive(false);
        attackData.gameObject.GetComponentInChildren<DamageOnTrigger2D>().OnDamageCaused.AddListener(OnDamageCaused);
        
    }

    void OnDamageCaused(GameObject target)
    {
        if (target.GetComponent<EnemyData>() != null)
            playerGun.RechargeSingleAmmo();
        else
            playerGun.RechargeAmmo(destructibleRechargePercentage);
    }
}
