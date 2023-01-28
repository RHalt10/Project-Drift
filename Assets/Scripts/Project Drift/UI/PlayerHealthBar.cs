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
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        float width = rectTransform.GetWidth();
        float segmentWidth = (width - spacing * (playerHealth.maxHealth + 1)) / playerHealth.maxHealth;

        for (int i = 0; i < playerHealth.maxHealth; i++)
        {
            GameObject spawnedPrefab = Instantiate(healthSegmentPrefab, transform);
            RectTransform spawnedTransform = spawnedPrefab.GetComponent<RectTransform>();
            spawnedTransform.SetWidth(segmentWidth);
            spawnedTransform.anchoredPosition = new Vector2(i * (segmentWidth + spacing) + spacing, 0);

            spawnedPrefab.GetComponent<Image>().color = i >= playerHealth.Current ? invalidColor : validColor;
        }
    }

}
