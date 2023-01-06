using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;

public class NinjaToadController : MonoBehaviour
{
    public enum State
    {
        Idle,
        Walking,
        Shooting,
        Dashing
    }

    public float normalSpeed;
    public float timeBeforeShoot;
    public float timeAfterShoot;
    public float dashSpeed;
    public float dashTime;
    public float walkTime;

    public float unacceptablePlayerRange;

    public GameObject projectilePrefab;

    [SerializeField] State startingState;

    State _currentState;
    public State CurrentState
    {
        get => _currentState;
        set
        {
            OnStateChange(value);
            _currentState = value;
        }
    }

    Coroutine currentCoroutine = null;

    GroundCharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<GroundCharacterController>();
        CurrentState = startingState;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnStateChange(State newState)
    {
        if (currentCoroutine != null) 
            StopCoroutine(currentCoroutine);

        IEnumerator coroutine = newState switch
        {
            State.Walking => Walk(),
            State.Shooting => Shoot(),
            _ => null
        };

        currentCoroutine = coroutine == null ? null : StartCoroutine(coroutine);
    }

    IEnumerator Walk()
    {
        float currentTime = 0;
        while (currentTime < walkTime)
        {
            Vector2 nextWalkingPosiiton = ComputeNextWalkingPosition();
            Vector2 velocity = (nextWalkingPosiiton - (Vector2)transform.position).normalized * normalSpeed;
            characterController.velocity = velocity;

            currentTime += Time.deltaTime;
            yield return null;
        }

        CurrentState = State.Shooting;
    }

    IEnumerator Shoot()
    {
        characterController.velocity = Vector2.zero;

        yield return new WaitForSeconds(timeBeforeShoot);

        Vector2 playerPosition = PlayerController.Instance.transform.position;
        Vector2 direction = playerPosition - (Vector2)transform.position;
        GameObject spawnedProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        spawnedProjectile.GetComponent<ProjectileMovement2D>().direction = direction;

        yield return new WaitForSeconds(timeAfterShoot);

        CurrentState = State.Walking;
    }

    Vector2 ComputeNextWalkingPosition()
    {
        Vector2 playerPosition = PlayerController.Instance.transform.position;
        Vector2 playerDelta = (Vector2)transform.position - playerPosition;

        float radius = playerDelta.magnitude;
        float distance = normalSpeed * Time.deltaTime;
        float angleDistance = distance * 360f / (2f * Mathf.PI * radius);

        float currentAngle = Vector2.SignedAngle(Vector2.up, playerDelta);
        float nextAngle = Mathf.Deg2Rad * (currentAngle + angleDistance);

        Vector2 delta = new Vector2(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle)) * radius;
        return playerPosition + delta;
    }
}
