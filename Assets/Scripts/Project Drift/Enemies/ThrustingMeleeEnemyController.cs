using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;

public class ThrustingMeleeEnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        //IdleWalk, 
        MoveTowardsPlayer,
        AvoidObstacle,
        BackUp,
        Attack
    }

    public float movementSpeed = 1.0f; //temp, much slower than player
    public float attackCooldown = 3.0f;
    public float attackWindup = 1.5f;
    public float hitboxRadiusMin = 0.8f;
    public float hitboxRadiusMax = 1.1f;
    public float attackSpeed = 3.0f;
    public float attackDuration = 0.2f;

    EnemyState state;
    public EnemyState State
    {
        get { return state; }
        private set
        {
            TransitionTo(value);
            state = value;
        }
    }

    [SerializeField] EnemyState startingState;

    Coroutine currentCoroutine = null;
    GroundCharacterController characterController;
    RaycastHit2D hit;
    Vector2 playerPosition;

    private float time = 0;

    void Start()
    {
        characterController = GetComponent<GroundCharacterController>();
        State = startingState;
    }
    private void Update()
    {
        playerPosition = PlayerController.Instance.transform.position;
        hit = Physics2D.Raycast(transform.position, playerPosition - (Vector2)transform.position, 1f, LayerMask.GetMask("Obstacle"));
        Debug.DrawRay(transform.position, playerPosition - (Vector2)transform.position, Color.black);
        time += Time.deltaTime;
    }

    void TransitionTo(EnemyState newState)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        IEnumerator coroutine = newState switch
        {
            EnemyState.MoveTowardsPlayer => Walk(),
            EnemyState.Attack => Attack(),
            EnemyState.BackUp => BackUp(),
            EnemyState.AvoidObstacle => Avoid(),
            _ => null
        };

        currentCoroutine = coroutine == null ? null : StartCoroutine(coroutine);
    }

    IEnumerator Walk()
    {
        while (Vector2.Distance(playerPosition, transform.position) > hitboxRadiusMax)
        {
            if (hit.collider != null){
                State = EnemyState.AvoidObstacle;
                yield return null;
            }
            else
            {
                Vector2 nextWalkingPosition = ComputeNextWalkingPosition();
                Vector2 velocity = (nextWalkingPosition - (Vector2)transform.position).normalized * movementSpeed;
                characterController.velocity = velocity;

                yield return null;
            }
        }

        State = EnemyState.Attack;
    }

    IEnumerator Attack()
    {
        while (Vector2.Distance(playerPosition, transform.position) < hitboxRadiusMax)
        {
            if (Vector2.Distance(playerPosition, transform.position) < hitboxRadiusMin)
            {
                State = EnemyState.BackUp;
                yield return null;
            }
            else
            {
                if (time > attackCooldown)
                {
                    characterController.velocity = Vector2.zero;
                    Vector2 playerPositionNow = PlayerController.Instance.transform.position;

                    yield return new WaitForSeconds(attackWindup);


                    Vector2 direction = playerPositionNow - (Vector2)transform.position;

                    characterController.velocity = attackSpeed * direction;
                    yield return new WaitForSeconds(attackDuration);
                    time = 0;
                }
                yield return null;
            }
        }
        State = EnemyState.MoveTowardsPlayer;
    }
    IEnumerator BackUp()
    {
        Vector2 playerPositionNow = PlayerController.Instance.transform.position;
        Vector2 direction = (Vector2)transform.position - playerPositionNow;

        characterController.velocity = movementSpeed * direction;
        yield return new WaitForSeconds(1f);

        if (Vector2.Distance(playerPosition, transform.position) < hitboxRadiusMax)
        {
            State = EnemyState.Attack;
        }
        else
        {
            State = EnemyState.MoveTowardsPlayer;
        }
    }
    IEnumerator Avoid()
    {
        while(hit.collider != null)
        {
            characterController.velocity = Vector2.Perpendicular((Vector2)transform.position - ComputeNextWalkingPosition()).normalized * movementSpeed;
            yield return new WaitForSeconds(1f);
        }

        State = EnemyState.MoveTowardsPlayer;
    }

    Vector2 ComputeNextWalkingPosition()
    {
        //Vector2 playerPosition = PlayerController.Instance.transform.position;
        Vector2 playerDelta = (Vector2)transform.position - playerPosition;

        float radius = playerDelta.magnitude;
        float distance = movementSpeed * Time.deltaTime;
        float angleDistance = distance * 360f / (2f * Mathf.PI * radius);

        float currentAngle = Vector2.SignedAngle(Vector2.up, playerDelta);
        float nextAngle = Mathf.Deg2Rad * (currentAngle + angleDistance);

        Vector2 delta = new Vector2(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle)) * radius;
        return playerPosition + delta;
    }
}
