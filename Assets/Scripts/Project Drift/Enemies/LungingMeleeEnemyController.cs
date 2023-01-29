using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LungingMeleeEnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        MoveToPlayer,
        Attack,
        AvoidObstacle
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
            EnemyState.MoveToPlayer => moveToPlayer(),
            EnemyState.Attack => attack(),
            EnemyState.AvoidObstacle => avoidObstacle(),
            _ => null
        };

        coroutine = StartCoroutine(newCoroutine);
    }

    //stats for behavior/movement
    public float movementSpeed = 3.0f; //the same as the player, whose ground speed is 3.0f at the moment

    //data for attack/lunge
    public float attackCooldown = 2.0f;
    public float attackWindup = 1.0f;

    //can't use forces with groundCharacterController so doing lunge by time & top speed
    //public float lungeForce = 100.0f;
    //TODO: decides the duration and top speed of the lunge
    public float lungeDuration = 1.0f; //temp values
    public float lungeTopSpeed = 5.0f; //temp values

    //range from player before deciding to initiate lunge
    public float attackRange = 2.0f;

    //track coroutine and the controller
    Coroutine coroutine;
    GroundCharacterController controller;

    //tracks time
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        controller = this.GetComponent<GroundCharacterController>();
        //rigid = this.GetComponent<Rigidbody2D>();

        //by default we'll have the lunging enemy start off moving towards the player
        State = EnemyState.MoveToPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    //move to player and decide how it wants to move
    IEnumerator moveToPlayer()
    {
        //move or avoid obstacle until player is in range
        while (distanceToPlayer() > attackRange)
        {
            Vector2 direction = directionToPlayer();

            //check if a nearby obstacle is blocking its attack range detection
            bool obstacleInFront = Physics2D.Raycast(this.transform.position, direction, attackRange, LayerMask.GetMask("Obstacle"));

            if (obstacleInFront)
            {
                State = EnemyState.AvoidObstacle;
            }
            else
            {
                //move towards player while there's no obstacle in front
                controller.velocity = direction * movementSpeed;
            }

            yield return null;
        }

        //TODO: there is a flaw where enemy is in attack range but there could still be an obstacle blocking it; designers didn't address this in their behavior chart?

        //by now, enemy is in range to start lunge attack
        State = EnemyState.Attack;
    }

    //do the lunge attack
    IEnumerator attack()
    {
        //pause for windup
        controller.velocity = Vector2.zero;
        yield return new WaitForSeconds(attackWindup);

        //begin lunge now that windup is over
        //record direction to lunge towards
        Vector2 lungeDirection = directionToPlayer();

        //TODO: groundCharacterController seems to be a velocity-based system and has no space for forces?
        //TODO: temporary way is to use a decreasing timer and have speed fall linearly with it
        timer = lungeDuration;
        while (timer > 0)
        {
            controller.velocity = lungeDirection * (lungeTopSpeed * timer / lungeDuration);
            yield return null;
        }


        //TODO: is attack cooldown for being still or it can still move during then, just can't attack? going with former for now
        //done attacking, now leave a cooldown of 2s
        controller.velocity = Vector2.zero;
        yield return new WaitForSeconds(attackCooldown);

        //when done attacking, go move to player
        State = EnemyState.MoveToPlayer;
    }

    //TODO: go around obstacle
    IEnumerator avoidObstacle()
    {
        //TODO: avoid obstacle

        //TODO: take out behavior below; it just mimics normal moving for now until actual avoidance is implemented
        controller.velocity = directionToPlayer() * movementSpeed;
        yield return new WaitForSeconds(0.0f);

        State = EnemyState.MoveToPlayer;
    }

    //helper function to locate distance from player
    double distanceToPlayer()
    {
        Vector2 playerPosition = PlayerController.Instance.transform.position;
        return Vector2.Distance(playerPosition, this.transform.position);
    }

    //helper function for unit vector direction to player
    Vector2 directionToPlayer()
    {
        Vector2 d = PlayerController.Instance.transform.position - this.transform.position;
        return d.normalized;
    }
}
