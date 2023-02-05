using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WSoft.UI;

/// <summary>
/// A component that displays the current player health
/// </summary>
public class PlayerHealthBar : MonoBehaviour
{
    public PlayerHealth playerHealth;

    public GameObject healthSegmentPrefab;

    public float spacing;

    public Color validColor = Color.green;
    public Color invalidColor = Color.gray;
    public Color midColor = Color.yellow;
    public Color lowColor = Color.red;
    
    public float midThreshold = 0.6f;
    public float lowThreshold = 0.4f;

    RectTransform rectTransform;


    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        playerHealth.events.OnHealthChange.AddListener(GenerateHealthBar);
        playerHealth.OnHealthUpgrade.AddListener(GenerateHealthBar);

        GenerateHealthBar();
    }

    void GenerateHealthBar()
    {
        for (int i = rectTransform.childCount - 1; i >= 0; i--)
            Destroy(rectTransform.GetChild(i).gameObject);

        float width = rectTransform.GetWidth();
        float segmentWidth = (width - spacing * (playerHealth.maxHealth + 1)) / playerHealth.maxHealth;

        // set color depending on player health
        float playerHealthPercent = (float)playerHealth.Current / (float)playerHealth.maxHealth;

        Color currentColor;
        if (playerHealthPercent <= lowThreshold)
        {
            currentColor = lowColor;
        }
        else if (playerHealthPercent <= midThreshold)
        {
            currentColor = midColor;
        }
        else
        {
            currentColor = validColor;
        }


        for (int i = 0; i < playerHealth.maxHealth; i++)
        {
            GameObject spawnedPrefab = Instantiate(healthSegmentPrefab, rectTransform);
            RectTransform spawnedTransform = spawnedPrefab.GetComponent<RectTransform>();
            spawnedTransform.SetWidth(segmentWidth);
            spawnedTransform.anchoredPosition = new Vector2(i * (segmentWidth + spacing) + spacing, 0);

            spawnedPrefab.GetComponent<Image>().color = i >= playerHealth.currentHealth ? invalidColor : currentColor;
        }
    }
}
