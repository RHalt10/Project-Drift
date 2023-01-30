using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackEvent
{
    public Vector3 KnockbackForce;
    public GameObject Participant;
    public float Duration;
    public KnockbackEvent(Vector3 _force, float _seconds, GameObject _participant)
    {
        KnockbackForce = _force; 
        Duration = _seconds;
        Participant = _participant;
    }
}

public class StunEvent
{
    public float StunSeconds;
    public GameObject Participant;
    public StunEvent(float _seconds, GameObject _participant)
    {
        StunSeconds = _seconds; 
        Participant = _participant;
    }
}


