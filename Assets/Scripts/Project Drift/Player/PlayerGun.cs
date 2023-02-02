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
    // Reference to Player Gun SOs *Note: When created a new gun, please add the prefab to this list.
    [SerializeField] List<GameObject> AllGunPrefabList;
    [Tooltip("Change during runtime and swap weapon (key: 1) to equip")] [SerializeField] 
    List<GameObject> equippedWeaponPrefabs;
    [Tooltip("Clear equipped weapons saved on disk")] public bool ResetOnLoad; 
    [Tooltip("Use equipped weapons provided by inspector instead of saved weapons on disk")] 
    public bool UseInspectorWeapons = false; 
    private int _equippedWeaponIndex
    {
        get { return SaveManager.Load<int>("equippedWeaponIndex"); }
        set { SaveManager.Save<int>("equippedWeaponIndex", value); }
    }

    public GameObject currentWeaponObj;
    public RangedWeaponBase currentWeapon 
    { 
        get 
        { 
            if( currentWeaponObj == null ) return null;
            RangedWeaponBase weaponInfo = currentWeaponObj.GetComponent<RangedWeaponBase>(); 
            if(weaponInfo == null) { Debug.LogError("Current weapon '" + currentWeaponObj.name + "' not a weapon"); }
            return weaponInfo; 
        }
    }
    
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

    private bool _isFiring = false;
    private PlayerController playerController;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] Transform attackRoot;
    public UnityEvent OnAmmoAmountChanged;
    public UnityEvent OnWeaponChanged;
    public Dictionary<string, GameObject> GunDictionary = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        playerController = GetComponent<PlayerController>();

        foreach (GameObject weapon in AllGunPrefabList)
        {
            GunDictionary.Add(weapon.name, weapon);
        }

        if(UseInspectorWeapons) {
            if(equippedWeaponPrefabs.Count != 0) {
                SetNewWeapon(equippedWeaponPrefabs[_equippedWeaponIndex]);
            } else {
                currentWeaponObj = null;
            }
        } else {
            _loadEquippedWeapons();
        }

        if (ResetOnLoad)
        {
            equippedWeaponPrefabs.Clear();
            _saveEquippedWeapons();
        } 

        currentAmmo = 1f;
    }

    public void Shoot(Vector2 direction)
    {
        if (currentWeapon == null) return;
        if (currentAmmo < currentWeapon.BulletPercentage) return;

        if(!_isFiring)
        {
            currentAmmo -= currentWeapon.BulletPercentage;
            
            currentWeapon.FireProjectile(direction, projectileSpawnPoint.position);
            StartCoroutine(_ShootCR());
        }
    }

    private void OnDestroy() {
        _saveEquippedWeapons();
    }

    // Save list of currently equipped guns. Called in OnDestroy().
    private void _saveEquippedWeapons()
    {
        List<string> equipped = new List<string>();
        foreach (GameObject weapon in equippedWeaponPrefabs)
        {
            equipped.Add(weapon.name);
        } 
        SaveManager.Save<List<string>>("equippedWeaponList", equipped);
    }

    // Load list of currently equipped weapons from save data and set currently equipped gun.
    // Called on Awake()
    private void _loadEquippedWeapons()
    {
        equippedWeaponPrefabs.Clear();
        List<string> equipped = SaveManager.Load<List<string>>("equippedWeaponList");
        foreach (string weaponName in equipped)
        {
            equippedWeaponPrefabs.Add(GunDictionary[weaponName]);
        }
        
        if(equippedWeaponPrefabs.Count != 0) {
            SetNewWeapon(equippedWeaponPrefabs[_equippedWeaponIndex]);
        } else {
            currentWeaponObj = null;
        }
    }

    private IEnumerator _ShootCR()
    {
        currentWeapon.shootSfx.Post(gameObject);
        EventBus.Publish(new PlayerShootEvent(currentWeapon));
        
        _isFiring = true;

        yield return new WaitForSeconds(currentWeapon.cdTime);

        _isFiring = false;
    }

    public void RechargeAmmo(float percentage)
    {
        currentAmmo = currentAmmo + percentage;
    }

    public void RechargeSingleAmmo()
    {
        currentAmmo = currentAmmo + currentWeapon.BulletPercentage;
    }

    public void SwapWeapons()
    {
        _equippedWeaponIndex++;
        if(_equippedWeaponIndex >= equippedWeaponPrefabs.Count)
            _equippedWeaponIndex = 0;
        
        if( equippedWeaponPrefabs.Count > 0 )
            SetNewWeapon(equippedWeaponPrefabs[_equippedWeaponIndex]);
    }

    // Instantiate weapon instance from prefab, sets currently equipped weapon.
    private void SetNewWeapon(GameObject weaponPrefab)
    {
        if (currentWeaponObj != null)
            Destroy(currentWeaponObj);

        currentWeaponObj = GameObject.Instantiate(weaponPrefab, attackRoot, false);
        HideGun();
        OnWeaponChanged.Invoke();
    }

    // Enable Gun when player touches a gun pickup
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GunPickup manager = collision.gameObject.GetComponent<GunPickup>();

        if (manager != null)
        {
            if( !equippedWeaponPrefabs.Contains(manager.weaponPrefab) )
            {
                Debug.Log("(PlayerGun) Enable Gun!");
                equippedWeaponPrefabs.Add(manager.weaponPrefab);
                SetNewWeapon(manager.weaponPrefab);

            }   
            
            currentAmmo = 1;
            Destroy(collision.gameObject);
        }
    }

    private void HideGun()
    {
        if (currentWeaponObj == null) return;
        SpriteRenderer sr = currentWeaponObj.GetComponentInChildren<SpriteRenderer>();
        Color tmp = sr.color;
        tmp.a = 0;
        sr.color = tmp;
    }
}
