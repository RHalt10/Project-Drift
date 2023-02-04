using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeSpreadRangedEnemyController : MonoBehaviour
{

    //Note: states are kind of unnecessary since this enemy does only one thing, but I'll still do it in case we want this enemy to do more
    public enum EnemyState
    {
        Attack
    }

    private EnemyState state;
    public EnemyState State
    {
        get { return state; }
        private set
        {
            TransitionTo(value);
            state = value;
        }
    }

    void TransitionTo(EnemyState newState)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        IEnumerator newCoroutine = newState switch
        {
            EnemyState.Attack => Attack(),
            _ => null
        };

        coroutine = StartCoroutine(newCoroutine);
    }

    //data for enemy's ranged attacks
    public float attackCooldown = 5.0f;
    public float attackWindup = 1.0f;
    public int projectileDamage = 1;

    //number of projectiles fired per shot
    public int numProjectilesPerSpread = 3;
    public float projectileSpeed = 2.5f; //temp value for a medium speed for projectiles
    //the angles of difference between each fired projectile (in degrees, not radians); please don't set some absurd value
    public float projectileAngleSpace = 30.0f;


    //TODO: considering creating specific projectile prefab for this enemy (currently uses the enemy projectile prefab in TempPrefabs)
    //the projectile object that will be spawned and fired off
    public GameObject projectilePrefab;

    //track coroutine and the controller
    Coroutine coroutine;
    GroundCharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = this.GetComponent<GroundCharacterController>();

        State = EnemyState.Attack;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //the action for firing shots on a periodic basis
    IEnumerator Attack()
    {
        //begin attack, record the direction to shoot and start with the windup
        Vector2 shootingDirection = DirectionToPlayer();
        yield return new WaitForSeconds(attackWindup);

        //shoot cone of projectiles in the shootingDirection
        FireProjectiles(shootingDirection);

        //when done shooting, enter a cooldown before enemy restarts the attack again
        yield return new WaitForSeconds(attackCooldown);
        State = EnemyState.Attack;
    }

    //spawns spread of projectiles towards a specified direction
    void FireProjectiles(Vector2 direction)
    {
        Vector2[] directions = new Vector2[numProjectilesPerSpread];

        
        //there are (numProjectilesPerSpread - 1) angles between these projectiles
        //calculate how far rotated the clockwise most projectile will be relative to the middle direction
        float startRotatation = projectileAngleSpace * (numProjectilesPerSpread - 1) / 2.0f;

        //set the numProjectilesPerSpread directions to shoot in
        for (int i = 0; i < numProjectilesPerSpread; i++)
        {
            directions[i] = RotateVector(direction, startRotatation - projectileAngleSpace * i);
            //spawn a projectile in that direction
            SpawnProjectile(directions[i]);
        }


    }

    //helper function for spawning a projectile on this enemy that goes a particular direction
    void SpawnProjectile(Vector2 direction)
    {
        //TODO: set proper direction that it faces; currently I'd presume it does the default identity direction
        GameObject projectile = Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);
        projectile.GetComponent<WSoft.Combat.ProjectileMovement2D>().direction = direction;
        projectile.GetComponent<WSoft.Combat.ProjectileMovement2D>().speed = projectileSpeed;
        projectile.GetComponent<WSoft.Combat.DamageOnCollision2D>().damage = projectileDamage;
    }

    //helper function for calculating a vector after being rotated some degrees
    public Vector2 RotateVector(Vector2 d, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float new_x = d.x * Mathf.Cos(radian) - d.y * Mathf.Sin(radian);
        float new_y = d.x * Mathf.Sin(radian) + d.y * Mathf.Cos(radian);
        return new Vector2(new_x, new_y);
    }

    //helper function for finding unit vector direction to player
    Vector2 DirectionToPlayer()
    {
        Vector2 d = PlayerController.Instance.transform.position - this.transform.position;
        return d.normalized;
    }
}
