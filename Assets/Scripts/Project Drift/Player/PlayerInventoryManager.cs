using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component that stores the player's current inventory
/// Written by Nikhil Ghosh '24
/// </summary>
public class PlayerInventoryManager : MonoBehaviour
{
    public static PlayerInventoryManager Instance { get; private set; }

    [SerializeField] Transform MeleeWeaponParentObject;

    public int keys { get; private set; }

    public int currency { get; private set; }

    public AK.Wwise.Event keyAcquiredSfx;
    
    public MeleeWeaponBase MeleeWeapon
    {
        get
        {
            if(meleeWeapon == null)
            {
                Debug.LogError("Error: No melee weapon detected!");
                return null;
            }

            return meleeWeapon;
        }
    }

    private MeleeWeaponBase meleeWeapon = null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        keys = 0;
    }

    public void AcquireKey()
    {
        keys++;
        keyAcquiredSfx.Post(gameObject);
    }

    // allows for setting specific values
    public void SetValue(string resource, int value)
    {
        if (resource == "keys")
        {
            keys = value;
        }
        else if (resource == "currency")
        {
            currency = value;
        }

        // return for all other resource string values
        return;
    }

    public void EquiptMeleeWeapon(GameObject WeaponPrefab)
    {
        if (MeleeWeaponParentObject.childCount > 1)
        {
            Debug.LogError("Error: More than one melee weapon equipped!");
            return;
        }

        if (WeaponPrefab.GetComponent<MeleeWeaponBase>() == null)
        {
            Debug.LogError("Error: Tried to equipt a weapon without MeleeWeaponBase component!");
            return;
        }

        if (MeleeWeaponParentObject.childCount > 0) Destroy(MeleeWeaponParentObject.GetChild(0));

        GameObject clone = Instantiate(WeaponPrefab, MeleeWeaponParentObject.position, Quaternion.identity);
        clone.transform.SetParent(MeleeWeaponParentObject);
        clone.transform.position = MeleeWeaponParentObject.position;
        meleeWeapon = clone.GetComponent<MeleeWeaponBase>();
        meleeWeapon.owningInventoryManager = this;
    }
}

