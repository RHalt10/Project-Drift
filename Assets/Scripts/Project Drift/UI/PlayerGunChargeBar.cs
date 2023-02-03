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

    float width => rectTransform.GetWidth();
    int maxAmmo => playerGun.currentWeapon != null ? playerGun.currentWeapon.ammoSubdivisions : 0;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        playerGun.OnAmmoAmountChanged.AddListener(GenerateChargeBar);
        playerGun.OnWeaponChanged.AddListener(GenerateChargeBar);

        if (playerGun.currentWeapon != null) GenerateChargeBar();
    }

    void GenerateChargeBar()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
 
        int validAmmo = (int)(playerGun.currentAmmo * maxAmmo);

        for (int i = 0; i < validAmmo; i++)
            SpawnChargeBar(i, validColor);

        float nearestBarDifference = playerGun.currentAmmo * maxAmmo - validAmmo;
        float bulletPercentage = playerGun.currentWeapon.BulletPercentage;
        if (nearestBarDifference >= 0.01f)
        {
            GameObject leftPrefab = SpawnChargeBar(validAmmo, validColor);
            Image leftImage = leftPrefab.GetComponent<Image>();
            leftImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            leftImage.fillAmount = (nearestBarDifference % bulletPercentage) / bulletPercentage;

            GameObject rightPrefab = SpawnChargeBar(validAmmo, invalidColor);
            Image rightImage = rightPrefab.GetComponent<Image>();
            rightImage.fillOrigin = (int)Image.OriginHorizontal.Right;
            rightImage.fillAmount = 1 - ((nearestBarDifference % bulletPercentage) / bulletPercentage);
        }
        else if (validAmmo != maxAmmo)
            SpawnChargeBar(validAmmo, invalidColor);

        for (int i = validAmmo + 1; i < maxAmmo; i++)
            SpawnChargeBar(i, invalidColor);
    }

    GameObject SpawnChargeBar(int i, Color color)
    {
        float segmentWidth = (width - spacing * (maxAmmo + 1)) / maxAmmo;

        GameObject spawnedPrefab = Instantiate(gunSegmentPrefab, transform);
        RectTransform spawnedTransform = spawnedPrefab.GetComponent<RectTransform>();
        spawnedTransform.SetWidth(segmentWidth);
        spawnedTransform.anchoredPosition = new Vector2(i * (segmentWidth + spacing) + spacing, 0);

        spawnedPrefab.GetComponent<Image>().color = color;
        return spawnedPrefab;
    }
}
