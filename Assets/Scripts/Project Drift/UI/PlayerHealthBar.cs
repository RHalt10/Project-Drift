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

    // used to test out ShakeUI()
    private void Update()
    {
        // use U key to trigger shake event
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("Pressed U");
            StartCoroutine(ShakeUI());
        }
    }

    // public variables to allow for changing the shake parameters for testing
    public float shakeAmount = 10.0f;
    public float lerpTime = 1.0f;
    // coroutine to lerp ui element to produce shaking effect
    private IEnumerator ShakeUI()
    {
        Transform UITransform = GetComponent<RectTransform>().transform;
        Vector3 StartPos = UITransform.position;
        Vector3 EndPos = new Vector3(UITransform.position.x + 10.0f, UITransform.position.y + 10.0f, UITransform.position.z);
        
        float startTime = Time.time;
        float proportion = (Time.time - startTime) / (startTime + lerpTime);
        while (proportion < 1.0f)
        {
            UITransform.position = new Vector3(Mathf.Lerp(StartPos.x, EndPos.x, proportion), Mathf.Lerp(StartPos.y, EndPos.y, proportion), StartPos.z);
            proportion = (Time.time - startTime) / (startTime + lerpTime);
            yield return null;
        }
        UITransform.position = EndPos;
    }

}
