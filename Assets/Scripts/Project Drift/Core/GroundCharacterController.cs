using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WSoft.Math;

public class GroundCharacterController : MonoBehaviour
{
    Rigidbody2D rb;

    public bool isEnabled = true;

    public bool canMoveOnAir = false;
    
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

    [HideInInspector]
    public Vector2 velocity;

    [SerializeField] LayerMask groundLayer;
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

        rb.velocity = velocity;
    }
}
