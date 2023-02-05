using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component that shakes the attached UI object using Hooke's Law
/// Written by Henry Lin '23
/// </summary>
public class ShakeOnDamage : MonoBehaviour
{
    public PlayerHealth playerHealth;

    RectTransform rectTransform;

    // Hooke's Law variables
    // k is roughly how stiff/reactive the "spring" is
    public float k_stiff = 0.3f;
    // higher value seems to produce more of the intended "springiness"
    public float friction = 0.9f;
    public float offset = 50.0f;

    private float vel = 0.0f;

    // testing variable
    private float startY;


    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // new event, for use in shake callback
        playerHealth.events.OnDamage.AddListener(DisplaceForShake);

        // set original y position
        startY = rectTransform.position.y;
    }

    // debug trigger the shake
    private void Update()
    {
        // displacement from target
        float x = startY - rectTransform.position.y;
        float accel = k_stiff * x;
        vel += accel;
        vel *= friction;
        rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y + vel, rectTransform.position.z);
    }

    // callback function to initiate shake effect
    private void DisplaceForShake()
    {
        rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y + offset, rectTransform.position.z);
    }
}
