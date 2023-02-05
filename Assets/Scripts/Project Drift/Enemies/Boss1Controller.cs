using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Controller : MonoBehaviour
{
    public enum BossState{
        MoveTowardsPlayer,
        SpinAttack,
        RushAttack,
        //ScreenBomb, 
        //HomingShot,
        //Leap
    }
    BossState state;
    public BossState State
    {
        get { return state; }
        private set
        {
            TransitionTo(value);
            state = value;
        }
    }

    [SerializeField] BossState startingState;

    Coroutine currentCoroutine = null;
    GroundCharacterController characterController;
    Vector2 playerPosition;

    public bool halfHealth = false;
    public bool phase2 = false;
    public float closeRadius;
    public float movementSpeed;
    public float breathingTime;

    [Header("Rush Attack")]
    public float rushAttackSpeedMax;
    public float rushAttackSpeed;
    public float rushAttackDeceleration;
    public float rushAttackSlowDistance; //distance to edge when rush attack decelerates 

    [Header("Spin Attack")]
    public float spinAttackDuration;
    public float spinAttackRotation; 
    GameObject hitbox;

    [Header("Homing Shot")]
    public GameObject homingProjectile;

    private float time;
    private float tolerance = 0.01f;

    void Start()
    {
        characterController = GetComponent<GroundCharacterController>();
        State = startingState;
        hitbox = transform.GetChild(0).gameObject;
        hitbox.transform.position = transform.position;
    }
    private void Update()
    {
        playerPosition = PlayerController.Instance.transform.position;
        time += Time.deltaTime;
    }
    void TransitionTo(BossState newState)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        IEnumerator coroutine = newState switch
        {
            BossState.MoveTowardsPlayer => Walk(),
            BossState.SpinAttack => SpinAttack(), //homing shot as placeholder for this week, change later 
            BossState.RushAttack => RushAttack(),
            _ => null
        };
        currentCoroutine = coroutine == null ? null : StartCoroutine(coroutine);
    }
    IEnumerator Walk()
    {
        while (true) 
        {
            if(time > breathingTime)
            {
                if(Vector2.Distance(playerPosition, transform.position) < closeRadius)
                {
                    State = BossState.SpinAttack;
                    yield return null;
                }
                else
                {
                    State = BossState.RushAttack;
                    yield return null;
                }
            }
            characterController.velocity = (playerPosition - (Vector2)transform.position).normalized * movementSpeed;
            yield return null;
        }
    }
    IEnumerator RushAttack()
    {
        Vector2 playerPositionNow = PlayerController.Instance.transform.position;
        Vector2 direction = playerPositionNow - (Vector2)transform.position;
    
        while (DistanceFromNearestEdge() > tolerance && rushAttackSpeed > tolerance)
        {
            if (DistanceFromNearestEdge() < rushAttackSlowDistance)
            {
                rushAttackSpeed *= rushAttackDeceleration;
            }
            characterController.velocity = rushAttackSpeed * direction;
            yield return null;
        }
        time = 0;
        rushAttackSpeed = rushAttackSpeedMax;
        State = BossState.MoveTowardsPlayer;
    }
    IEnumerator SpinAttack()
    {
        //characterController.velocity = Vector2.zero;
        hitbox.transform.position = new Vector2(hitbox.transform.position.x, hitbox.transform.position.y - 1);
        float startRotation = transform.eulerAngles.z;
        float endRotation = startRotation + spinAttackRotation;
        float t = 0.0f;
        while (t < spinAttackDuration)
        {
            t += Time.deltaTime;
            float zRotation = Mathf.Lerp(startRotation, endRotation, t / spinAttackDuration) % spinAttackRotation;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zRotation);
            yield return null;
        }
        time = 0;
        hitbox.transform.position = transform.position;
        State = BossState.MoveTowardsPlayer;
    }
    IEnumerator HomingShot() //starts in phase 2 
    {
        characterController.velocity = Vector2.zero;
        if (!GameObject.Find(homingProjectile.name) && !GameObject.Find(homingProjectile.name + "(Clone)"))
        {
            Instantiate(homingProjectile, transform.position, Quaternion.identity);
        }
        yield return null;
        time = 0;
        State = BossState.MoveTowardsPlayer;
    }
    private float DistanceFromNearestEdge()
    {
        float size = GetComponent<CircleCollider2D>().radius;
        Collider2D groundCollider = GameObject.Find("Ground").GetComponent<Collider2D>();

        float leftEdgePosition = groundCollider.bounds.center.x - groundCollider.bounds.extents.x + size;
        float rightEdgePosition = groundCollider.bounds.center.x + groundCollider.bounds.extents.x - size;
        float topEdgePosition = groundCollider.bounds.center.y + groundCollider.bounds.extents.y - size;
        float bottomEdgePosition = groundCollider.bounds.center.y - groundCollider.bounds.extents.y + size;

        float distanceFromLeft = - leftEdgePosition + transform.position.x;
        float distanceFromRight = rightEdgePosition - transform.position.x;
        float distanceFromTop = topEdgePosition - transform.position.y;
        float distanceFromBottom = - bottomEdgePosition + transform.position.y;

        return Mathf.Min(distanceFromLeft, Mathf.Min(distanceFromRight, Mathf.Min(distanceFromTop, Mathf.Min(distanceFromBottom))));
    }
}


/*
 * while 
 * if(distance to player < closeRadius){
 *      spin attack();
 *  }
 *  else{
 *      charge attack();
 *     
 * cooldown
 * 
 * same as above 
 * 
 * leap attack 
 * 
 * } // loop
 * 
 * 
 * if phase 2 - boss at half health 
 * screen bomb() 
 * cooldown
 * 
 * homingshot() 
 * 
 * restart loop
 * 
 * if homing shot not in field 
 *  shoot homing shot instead of charge shot 
 *  
 */