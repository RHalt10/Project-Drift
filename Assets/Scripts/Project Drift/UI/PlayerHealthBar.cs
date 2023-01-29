using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WSoft.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public PlayerHealth playerHealth;

    public GameObject healthSegmentPrefab;

    public float spacing;

    public Color validColor;
    public Color invalidColor;

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

        playerHealth.events.OnHealthChange.AddListener(GenerateHealthBar);
        playerHealth.OnHealthUpgrade.AddListener(GenerateHealthBar);
        // new event, for use in shake callback
        playerHealth.events.OnDamage.AddListener(DisplaceForShake);

        GenerateHealthBar();

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

    void GenerateHealthBar()
    {
        for (int i = rectTransform.childCount - 1; i >= 0; i--)
            Destroy(rectTransform.GetChild(i).gameObject);

        float width = rectTransform.GetWidth();
        float segmentWidth = (width - spacing * (playerHealth.maxHealth + 1)) / playerHealth.maxHealth;

        for (int i = 0; i < playerHealth.maxHealth; i++)
        {
            GameObject spawnedPrefab = Instantiate(healthSegmentPrefab, rectTransform);
            RectTransform spawnedTransform = spawnedPrefab.GetComponent<RectTransform>();
            spawnedTransform.SetWidth(segmentWidth);
            spawnedTransform.anchoredPosition = new Vector2(i * (segmentWidth + spacing) + spacing, 0);

            spawnedPrefab.GetComponent<Image>().color = i >= playerHealth.Current ? invalidColor : validColor;
        }
    }

    // callback function to initiate shake effect
    private void DisplaceForShake()
    {
        rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y + offset, rectTransform.position.z);
    }

}
