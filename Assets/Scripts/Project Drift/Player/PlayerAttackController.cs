using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;

[System.Serializable]
public class PlayerAttackData
{
    public string animationTrigger;
    public GameObject gameObject;
    public float attackDuration;
    public float startDamageTime;
    public float endDamageTime;
}

[System.Serializable]
public class PlayerAttackController : PlayerSubController
{
    public float groundSpeed;
    public GameObject attackRoot;

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
        animator.SetTrigger(attackData.animationTrigger);
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

    void OnDamageCaused()
    {
        playerGun.RechargeSingleAmmo();
    }
}
