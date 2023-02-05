using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WSoft.Combat;

/// <summary>
/// A component that displays the current ranged weapon
/// Written by Henry Lin '23
/// </summary>
public class WeaponUIRanged : MonoBehaviour
{
    public PlayerGun playerGun;
    public Sprite pistolSprite;
    public Sprite bolaslauncherSprite;
    public Sprite blunderbussSprite;

    private Image rangedImage;

    private void Awake()
    {
        rangedImage = GetComponent<Image>();
    }

    private void Start()
    {
        playerGun.OnWeaponChanged.AddListener(UpdateRangedWeaponUI);

        UpdateRangedWeaponUI();
    }

    // callback function when Melee Weapon is changed
    private void UpdateRangedWeaponUI()
    {
        RangedWeaponBase currentRangedWeapon = playerGun.currentWeapon;
        if (currentRangedWeapon == null)
        {
            rangedImage.sprite = null;
            rangedImage.color = Color.clear;
            return;
        }

        rangedImage.color = Color.white;
        if (currentRangedWeapon.weaponName == "Pistol")
        {
            rangedImage.sprite = pistolSprite;
        }
        else if (currentRangedWeapon.weaponName == "Bolas Launcher")
        {
            rangedImage.sprite = bolaslauncherSprite;
        }
        else if (currentRangedWeapon.weaponName == "Blunderbuss")
        {
            rangedImage.sprite = blunderbussSprite;
        }
    }
}
