using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Pathfinding;
using TMPro;
using System.IO;

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

    //TODO: may need a timer variable to track cooldown
    //private float timer = 0;

    //TODO: possible variables needed for pathfinding; still testing out the pathfinding
    Seeker seeker;
    Pathfinding.Path pathToPlayer;
    int currentWaypoint = 0;
    public float repathRate = 0.5f;
    float repathTimer = 0;
    public float nextWaypointDistance = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        controller = this.GetComponent<GroundCharacterController>();
        //rigid = this.GetComponent<Rigidbody2D>();

        //start looking for a path to the player
        seeker = this.GetComponent<Seeker>();
        seeker.pathCallback += OnPathComplete;
        seeker.StartPath(this.transform.position, PlayerController.Instance.transform.position);
        repathTimer = repathRate;

        //by default we'll have the lunging enemy start off moving towards the player
        State = EnemyState.MoveToPlayer;


        
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: make use of a timer for the attack cooldown in the future
        /*
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        */

        
        //update the path periodically when the seeker is already done finding a path
        if (repathTimer <= 0 && seeker.IsDone())
        {
            repathTimer = repathRate;
            seeker.StartPath(this.transform.position, PlayerController.Instance.transform.position);
        }
        if (repathTimer > 0)
        {
            repathTimer -= Time.deltaTime;
        }
        
        
    }

    //move to player and decide how it wants to move
    IEnumerator MoveToPlayer()
    {
        //move until player is in range AND there's no obstacle blocking the way
        while (DistanceToPlayer() > attackRange || ObstacleBetweenThisAndPlayer())
        {
            //TODO: below is commented until some pathfinding questions have been resolved; comment the above when pathfinding is ready
            controller.velocity = NextStepToPlayer() * movementSpeed;

            yield return null;
        }

        //by now, the player is in range for this enemy to lunge attack
        State = EnemyState.Attack;
    }

    //do the lunge attack
    IEnumerator Attack()
    {
        //TODO: there's a weird bug when adding the below comments causes unity to crashe if players stays within attack range after the lunge attack
        /*
        //don't initiate attack if cooldown is still up
        if (timer > 0)
        {
            State = EnemyState.MoveToPlayer;
        }
        */

        //pause for windup
        controller.velocity = Vector2.zero;
        yield return new WaitForSeconds(attackWindup);
        
        //begin lunge now that windup is over
        //record direction to lunge towards
        Vector2 lungeDirection = directionToPlayer();

        //TODO: groundCharacterController seems to be a velocity-based system and has no space for forces?
        //current way of lunging is to use a decreasing timer and have speed fall linearly with it
        //Note: total distance traveled throughout this lunge is about 0.5 * lungeTopSpeed * lungeDuration
        float lungeTimer = lungeDuration;
        while (lungeTimer > 0)
        {
            controller.velocity = lungeDirection * (lungeTopSpeed * lungeTimer / lungeDuration);
            lungeTimer -= Time.deltaTime;
            yield return null;
        }
        //TODO: designers want the damaging hitbox on top of the enemy when it throws itself; might want to wait until we see the assets/visual

        //done attacking, now leave a short duration of being still as an opening for players
        controller.velocity = Vector2.zero;

        //TODO: temporary way of doing cooldown is being inactive while cooldown is up; need to change so that enemies move while attack cooldown is up
        yield return new WaitForSeconds(attackCooldown);
        /*
        //yield return new WaitForSeconds(0.5f);
        //reset timer to the attack cooldown
        timer = attackCooldown;
        */

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

    //TODO: implementation of functions for pathfinding astar below; functions have been left unused

    bool ObstacleBetweenThisAndPlayer()
    {
        Vector2 d = directionToPlayer();
        //check if an obstacle between this enemy and the player
        bool result = Physics2D.Raycast(this.transform.position, d, attackRange, LayerMask.GetMask("Obstacle"));
        return result;
    }

    //use pathfinding to find the next direction to get to player
    Vector2 NextStepToPlayer()
    {
        if (pathToPlayer == null)
        {
            //no path found yet, just wait
            return Vector2.zero;
        }

        bool reachedEndOfPath = false;
        // The distance to the next waypoint in the path
        float distanceToWaypoint;
        
        distanceToWaypoint = Vector3.Distance(this.transform.position, pathToPlayer.vectorPath[currentWaypoint]);
        //you're close enough to current waypoint of the path and might wanna consider moving to the next waypoint
        if (distanceToWaypoint < nextWaypointDistance)
        {
            // check if there is another waypoint
            if (currentWaypoint + 1 < pathToPlayer.vectorPath.Count)
            {
                currentWaypoint++;
            }
            else
            {
                // you'ved reached the end of the path instead
                reachedEndOfPath = true;
            }
        }

        //if close enough to the target point (end of path), then just stop
        if (reachedEndOfPath)
        {
            return Vector2.zero;
        }

        //the next step/direction is go is the waypoint in path we currently want to head to
        Vector3 d = (pathToPlayer.vectorPath[currentWaypoint] - this.transform.position).normalized;
        return d;
    }

    //helper function that gets called when the path to player is found
    public void OnPathComplete(Pathfinding.Path p)
    {
        Debug.Log("Yay, this enemy got a path to the player back. Did it have an error? " + p.error);

        // Path pooling. To avoid unnecessary allocations paths are reference counted.
        p.Claim(this);
        if (!p.error)
        {
            if (pathToPlayer != null) pathToPlayer.Release(this);
            pathToPlayer = p;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            currentWaypoint = 0;
        }
        else
        {
            p.Release(this);
        }
    }

    //a precaution for removing callback references
    public void OnDisable()
    {
        seeker.pathCallback -= OnPathComplete;
    }

    //a precaution for bringing back callback references after being disabled
    public void OnEnable()
    {
        seeker.pathCallback += OnPathComplete;
    }
}
