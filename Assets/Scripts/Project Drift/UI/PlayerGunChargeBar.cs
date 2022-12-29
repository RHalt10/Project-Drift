using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WSoft.UI;

public class PlayerGunChargeBar : MonoBehaviour
{
    public PlayerGun playerGun;

    public GameObject gunSegmentPrefab;

    public float spacing;

    public Color validColor;
    public Color invalidColor;

    RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        playerGun.OnAmmoAmountChanged.AddListener(GenerateChargeBar);
        playerGun.OnWeaponChanged.AddListener(GenerateChargeBar);

        GenerateChargeBar();
    }

    void GenerateChargeBar()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        float width = rectTransform.GetWidth();
        int maxAmmo = playerGun.currentWeapon.maxAmmo;
        float segmentWidth = (width - spacing * (maxAmmo + 1)) / maxAmmo;

        for (int i = 0; i < maxAmmo; i++)
        {
            GameObject spawnedPrefab = Instantiate(gunSegmentPrefab, transform);
            RectTransform spawnedTransform = spawnedPrefab.GetComponent<RectTransform>();
            spawnedTransform.SetWidth(segmentWidth);
            spawnedTransform.anchoredPosition = new Vector2(i * (segmentWidth + spacing) + spacing, 0);

            spawnedPrefab.GetComponent<Image>().color = i >= playerGun.currentAmmo ? invalidColor : validColor;
        }
    }
}
