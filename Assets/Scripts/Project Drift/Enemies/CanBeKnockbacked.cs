using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeKnockbacked : MonoBehaviour
{
    private GroundCharacterController controller;
    Subscription<KnockbackEvent> knockback_event;
    private void Awake()
    {
        knockback_event = EventBus.Subscribe<KnockbackEvent>(OnKnockback);
    }
    private void Start()
    {
        controller = GetComponent<GroundCharacterController>();
    }

    void OnKnockback(KnockbackEvent e)
    {
        if (e.Participant != gameObject) return;
        StartCoroutine(KnockbackRoutine(e.KnockbackForce, e.Duration));
    }

    public void Knockback(Vector3 knockbackForce, float duration)
    {
        StartCoroutine(KnockbackRoutine(knockbackForce, duration));
    }

    IEnumerator KnockbackRoutine(Vector3 force, float duration)
    {
        controller.velocity = force;
        yield return new WaitForSecondsRealtime(duration);
        controller.velocity = Vector2.zero;
    }


}
