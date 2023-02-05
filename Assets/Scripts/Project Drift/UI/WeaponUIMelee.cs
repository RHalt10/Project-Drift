using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WSoft.Combat;

/// <summary>
/// A component that displays the current melee weapon
/// Written by Henry Lin '23
/// </summary>
public class WeaponUIMelee : MonoBehaviour
{
    public PlayerInventoryManager playerInventory;
    public Sprite swordSprite;
    public Sprite hammerSprite;
    public Sprite spearSprite;

    private Image meleeImage;
    
    private void Awake()
    {
        meleeImage = GetComponent<Image>();
    }

    private void Start()
    {
        playerInventory.OnMeleeWeaponChange.AddListener(UpdateMeleeWeaponUI);

        UpdateMeleeWeaponUI();
    }

    // callback function when Melee Weapon is changed
    private void UpdateMeleeWeaponUI()
    {
        MeleeWeaponTypes currentType = playerInventory.MeleeWeapon.MeleeWeaponType;
        if (currentType == MeleeWeaponTypes.None)
        {
            meleeImage.sprite = null;
            meleeImage.color = Color.clear;
            return;
        }

        meleeImage.color = Color.white;
        if (currentType == MeleeWeaponTypes.Sword)
        {
            meleeImage.sprite = swordSprite;
        }
        else if (currentType == MeleeWeaponTypes.Hammer)
        {
            meleeImage.sprite = hammerSprite;
        }
        else if (currentType == MeleeWeaponTypes.Spear)
        {
            meleeImage.sprite = spearSprite;
        }
    }
}
