using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeStunned : MonoBehaviour
{
    private SpriteRenderer sr;
    Subscription<StunEvent> stun_event;
    private void Awake()
    {
        stun_event = EventBus.Subscribe<StunEvent>(OnStun);
    }
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnStun(StunEvent e)
    {
        if (e.Participant != gameObject) return;

        StartCoroutine(Temp_Stun());
    }

    IEnumerator Temp_Stun()
    {
        sr.color = Color.blue;

        yield return new WaitForSecondsRealtime(0.8f);

        sr.color = Color.red;
    }
}
