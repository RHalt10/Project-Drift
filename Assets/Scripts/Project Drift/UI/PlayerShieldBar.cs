using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WSoft.UI;

/// <summary>
/// A component that displays the current shield status
/// Written by Henry Lin '23
/// </summary>
public class PlayerShieldBar : MonoBehaviour
{
    public PlayerHealth playerHealth;

    public GameObject shieldSegmentPrefab;

    public float spacing;

    public Color validColor;
    public Color invalidColor;

    RectTransform rectTransform;


    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        playerHealth.events.OnHealthChange.AddListener(GenerateShieldBar);
        // TODO - call GenerateShieldBar for a ShieldRegen event, once implemented
        playerHealth.OnShieldRegen.AddListener(GenerateShieldBar);

        GenerateShieldBar();
    }

    void GenerateShieldBar()
    {
        for (int i = rectTransform.childCount - 1; i >= 0; i--)
            Destroy(rectTransform.GetChild(i).gameObject);

        float width = rectTransform.GetWidth();
        float segmentWidth = (width - spacing * (playerHealth.maxShield + 1)) / playerHealth.maxShield;


        for (int i = 0; i < playerHealth.maxShield; i++)
        {
            GameObject spawnedPrefab = Instantiate(shieldSegmentPrefab, rectTransform);
            RectTransform spawnedTransform = spawnedPrefab.GetComponent<RectTransform>();
            spawnedTransform.SetWidth(segmentWidth);
            spawnedTransform.anchoredPosition = new Vector2(i * (segmentWidth + spacing) + spacing, 0);

            spawnedPrefab.GetComponent<Image>().color = i >= playerHealth.currentShield ? invalidColor : validColor;
        }
    }
}
