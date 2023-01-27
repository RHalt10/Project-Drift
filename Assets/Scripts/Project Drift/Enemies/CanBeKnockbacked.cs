using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeKnockbacked : MonoBehaviour
{
    private Rigidbody2D rb;
    Subscription<KnockbackEvent> knockback_event;
    private void Awake()
    {
        knockback_event = EventBus.Subscribe<KnockbackEvent>(OnKnockback);
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnKnockback(KnockbackEvent e)
    {
        if (e.Participant != gameObject) return;

        rb.AddForce(e.KnockbackForce, ForceMode2D.Impulse);
    }


}
