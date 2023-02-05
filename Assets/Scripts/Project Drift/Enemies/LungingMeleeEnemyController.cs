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
            EnemyState.MoveToPlayer => MoveToPlayer(),
            EnemyState.Attack => Attack(),
            EnemyState.AvoidObstacle => AvoidObstacle(),
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
    //decides the duration and top speed of the lunge
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
    IEnumerator MoveToPlayer()
    {
        //move or avoid obstacle until player is in range
        while (DistanceToPlayer() > attackRange)
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

        //TODO: there is a flaw where enemy is in attack range but there could still be an obstacle blocking it; this wasn't addressed in behavior chart?

        //by now, enemy is in range to start lunge attack
        State = EnemyState.Attack;
    }

    //do the lunge attack
    IEnumerator Attack()
    {
        //don't initiate attack if cooldown is still up
        if (timer > 0)
        {
            State = EnemyState.MoveToPlayer;
        }

        //pause for windup
        controller.velocity = Vector2.zero;
        yield return new WaitForSeconds(attackWindup);
        
        //begin lunge now that windup is over
        //record direction to lunge towards
        Vector2 lungeDirection = directionToPlayer();

        //TODO: groundCharacterController seems to be a velocity-based system and has no space for forces?
        //current way of lunging is to use a decreasing timer and have speed fall linearly with it
        //Note: total distance traveled throughout this lunge is about 0.5 * lungeTopSpeed * lungeDuration
        timer = lungeDuration;
        while (timer > 0)
        {
            controller.velocity = lungeDirection * (lungeTopSpeed * timer / lungeDuration);
            yield return null;
        }
        //TODO: designers want the damaging hitbox on top of the enemy when it throws itself; might want to wait until we see the assets/visual

        //done attacking, now leave a short duration of being still as an opening for players
        controller.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);

        //reset timer to the attack cooldown
        timer = attackCooldown;

        //when done attacking, go move to player
        State = EnemyState.MoveToPlayer;
    }

    //move until an obstacle is avoided
    IEnumerator AvoidObstacle()
    {
        //check for obstacle
        Vector2 playerDirection = directionToPlayer();
        bool obstacleInFront = Physics2D.Raycast(this.transform.position, playerDirection, attackRange, LayerMask.GetMask("Obstacle"));

        while (obstacleInFront)
        {
            //use a helper function to determine which direction to avoid obstacle based on the current direction to player
            controller.velocity = DirectionToAvoidObstacle(playerDirection) * movementSpeed;

            //briefly move in that direction around the obstacle
            yield return new WaitForSeconds(0.5f);

            //check direction and obstacle in front again
            playerDirection = directionToPlayer();
            obstacleInFront = Physics2D.Raycast(this.transform.position, playerDirection, attackRange, LayerMask.GetMask("Obstacle"));
        }

        //no more obstacle in front, go back to moving
        State = EnemyState.MoveToPlayer;
    }

    //helper function to locate distance from player
    double DistanceToPlayer()
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


    //TODO: improve on the current method on finding a direction to avoid an obstacle; current method just tries 4 directions to see if any are clear
    //check 45 and 90 degrees clockwise/counterclockwise directions of playerDirection
    //helper function that gives a suggested direction to avoid an obstacle
    Vector2 DirectionToAvoidObstacle(Vector2 playerDirection)
    {
        int[] angles = { 45, -45, 90, -90 };

        for (int i = 0; i < 4; i++)
        {
            //check a direction that is an angle of angles[i] away from playerDirection
            Vector2 newDirection = Quaternion.Euler(0, 0, angles[i]) * (Vector3)playerDirection;
            bool directionIsClear = !Physics2D.Raycast(this.transform.position, newDirection, attackRange, LayerMask.GetMask("Obstacle"));
            if (directionIsClear)
            {
                return newDirection;
            }
        }
        
        //at this point, none of the 4 suggested directions work, we'll just have the enemy back up for space
        return playerDirection * -1;
    }
}
