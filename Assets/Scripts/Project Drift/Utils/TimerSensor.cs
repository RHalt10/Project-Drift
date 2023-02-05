using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerSensor : MonoBehaviour
{
    public float Timescale;
    void Update()
    {
        Timescale = Time.timeScale;
    }
}
