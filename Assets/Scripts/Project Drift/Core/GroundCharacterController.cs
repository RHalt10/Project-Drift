using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WSoft.Math;

/// <summary>
/// A script that controls a character on the ground
/// When a velocity is passed in, the ground character controller
/// will move on ground tiles, taking into account the settings specified.
/// Written by Nikhil Ghosh '24
/// </summary>
public class GroundCharacterController : MonoBehaviour
{
    Rigidbody2D rb;

    /// <summary>
    /// Is this controller enabled. If this is false, the script will stop calculating, saving performance
    /// </summary>
    public bool isEnabled = true;

    /// <summary>
    /// Is this controller allowed to move when there is no ground underneath
    /// </summary>
    public bool canMoveOnAir = false;
    
    /// <summary>
    /// If this controller is not on ground, will it fall.
    /// </summary>
    [SerializeField] bool _willFallOnAir = true;
    public bool willFallOnAir
    {
        get { return _willFallOnAir; }
        set
        {
            _willFallOnAir = value;

            if (!isOnGround && value)
                OnFall.Invoke();
        }
    }

    /// <summary>
    /// The velocity that the character controller is moving at.
    /// If not enabled or can't move, it will not move.
    /// </summary>
    //[HideInInspector]
    public Vector2 velocity;
    
    /// <summary>
    /// Multiplier for current velocity. 1 by default.
    /// </summary>
    public float velocityMultiplier = 1;

    /// <summary> The ground layermask (for checking if the character is on ground) </summary>
    [SerializeField] LayerMask groundLayer;
    /// <summary> The obstacle layermask (for checking if the character is going on an obstacle) </summary>
    [SerializeField] LayerMask obstacleLayer;

    HashSet<Collider2D> groundColliders = new HashSet<Collider2D>();
    public bool isOnGround => groundColliders.Count > 0;

    public UnityEvent OnFall;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled)
        {
            if (isOnGround)
                Move();
            else if (willFallOnAir)
                Fall();
        }
    }

    /// <summary>
    /// Move this character to the new position. The controller will recalculate if it is on the ground.
    /// </summary>
    public void Teleport(Vector2 newPosition)
    {
        rb.MovePosition(newPosition);

        groundColliders.Clear();
        foreach (Collider2D collider in Physics2D.OverlapPointAll(newPosition, groundLayer))
            groundColliders.Add(collider);

        if (!isOnGround && willFallOnAir)
            OnFall.Invoke();
    }

    public bool IsPointOnGround(Vector2 position)
    {
        return Physics2D.OverlapPoint(position, groundLayer) != null;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(LayermaskFunctions.IsInLayerMask(groundLayer, collision.gameObject.layer))
        {
            groundColliders.Add(collision);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (LayermaskFunctions.IsInLayerMask(groundLayer, collision.gameObject.layer))
        {
            groundColliders.Remove(collision);

            if (groundColliders.Count == 0)
            {
                if (willFallOnAir)
                    OnFall.Invoke();
            }
        }
    }

    void Fall()
    {
        rb.velocity = Vector2.zero;
    }

    void Move()
    {
        if (velocity.magnitude < 0.01f || Time.timeScale < 0.01f)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 delta = velocity * (Time.deltaTime + 0.07f);
        Collider2D col = Physics2D.OverlapPoint((Vector2)transform.position + delta, obstacleLayer);
        if (col != null)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (!canMoveOnAir)
        {
            col = Physics2D.OverlapPoint((Vector2)transform.position + delta, groundLayer);
            if (col == null)
            {
                rb.velocity = Vector2.zero;
                return;
            }
        }

        rb.velocity = velocity * velocityMultiplier;
    }
}
