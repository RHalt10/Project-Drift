using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WSoft.Combat;
using WSoft.Core; 

/// <summary>
/// A script that controls the player shooting and ammo amounts
/// Note that it is not a player subcontroller
/// Written by Nikhil Ghosh '24
/// </summary>
public class PlayerGun : MonoBehaviour
{
    // Reference to Player Gun SOs *Note: When created a new gun, please add the SO to this list.
    [SerializeField] List<PlayerWeaponSO> GunSOList;

    [Header("Developer Settings")]
    [SerializeField] bool ResetOnLoad;

    private PlayerWeaponSO _currentWeapon; 
    public PlayerWeaponSO currentWeapon 
    {
        get
        {
            string name = SaveManager.Load<string>("currentWeapon");
            _currentWeapon = name == "none" ? null : GunSODictionary[name];
            return _currentWeapon;
        } 

        private set
        {
            string name = value == null ? "none" : GunSONameDictionary[value];
            SaveManager.Save<string>("currentWeapon", name);
            _currentWeapon = name == "none" ? null : GunSODictionary[name];
        } 
    }

    public Dictionary<PlayerWeaponSO, string> GunSONameDictionary = new Dictionary<PlayerWeaponSO, string>();
    public Dictionary<string, PlayerWeaponSO> GunSODictionary = new Dictionary<string, PlayerWeaponSO>();

    /* Always from 0 to 1*/
    float _currentAmmo = 0f;
    public float currentAmmo
    {
        get {
            _currentAmmo = SaveManager.Load<float>("currentAmmo");
            return _currentAmmo; 
        }

        set
        {
            _currentAmmo = Mathf.Clamp(value, 0, 1);
            SaveManager.Save<float>("currentAmmo", _currentAmmo);
            OnAmmoAmountChanged.Invoke();
        }
    }

    [SerializeField] Transform projectileSpawnPoint;

    public UnityEvent OnAmmoAmountChanged;
    public UnityEvent OnWeaponChanged;

    // Start is called before the first frame update
    void Awake()
    {
        if (ResetOnLoad)
        {
            currentWeapon = null;
            currentAmmo = 0f;
        }

        if (SaveManager.Load<string>("currentWeapon") == null)
        {
            currentWeapon = null;
        }

        GunSONameDictionary.Clear();
        GunSODictionary.Clear();

        foreach (PlayerWeaponSO weapon in GunSOList)
        {
            GunSODictionary.Add(weapon.name, weapon);
            GunSONameDictionary.Add(weapon, weapon.name);
        }
    }

    public void Shoot(Vector2 direction)
    {
        if (currentWeapon == null) return;

        if(currentAmmo < currentWeapon.BulletPercentage) return;

        currentAmmo -= currentWeapon.BulletPercentage;

        GameObject spawnedProjectile = Instantiate(currentWeapon.projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        spawnedProjectile.GetComponent<ProjectileMovement2D>().direction = direction.normalized;

        currentWeapon.shootSfx.Post(gameObject);

        EventBus.Publish(new PlayerShootEvent(currentWeapon));
    }

    public void RechargeAmmo(float percentage)
    {
        currentAmmo = currentAmmo + percentage;
    }

    public void RechargeSingleAmmo()
    {
        currentAmmo = currentAmmo + currentWeapon.BulletPercentage;
    }

    public void SwapWeapons(PlayerWeaponSO newWeapon)
    {
        currentWeapon = newWeapon;
        OnWeaponChanged.Invoke();
    }

    // Enable Gun when player touches a gun pickup
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GunPickup manager = collision.gameObject.GetComponent<GunPickup>();
        if (manager != null)
        {
            Debug.Log("(PlayerGun) Enable Gun!");
            SwapWeapons(manager.GunSO);
            currentAmmo = 1;
            Destroy(collision.gameObject);
        }
    }
}
