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
    [SerializeField] List<GameObject> equippedWeaponPrefabs;

    private void OnDestroy() {
        _saveEquippedWeapons();
    }

    private void _saveEquippedWeapons()
    {
        List<string> equipped = new List<string>();
        foreach (GameObject weapon in equippedWeaponPrefabs)
        {
            equipped.Add(weapon.name);
        } 
        SaveManager.Save<List<string>>("equippedWeaponList", equipped);
        SaveManager.Save<int>("equippedWeaponIndex", _equippedWeaponIndex);
    }

    private void _loadEquippedWeapons()
    {
        equippedWeaponPrefabs.Clear();
        List<string> equipped = SaveManager.Load<List<string>>("equippedWeaponList");
        foreach (string weaponName in equipped)
        {
            equippedWeaponPrefabs.Add(GunDictionary[weaponName]);
        }
        _equippedWeaponIndex = SaveManager.Load<int>("equippedWeaponIndex");
        
        if(equippedWeaponPrefabs.Count != 0) {
            SetNewWeapon(equippedWeaponPrefabs[0]);
        } else {
            currentWeaponObj = null;
        }
    }

    public bool ResetOnLoad; 
    private int _equippedWeaponIndex = 0;

    private GameObject _currentWeapon; 
    public GameObject currentWeaponObj;
    // {
    //     get
    //     {
    //         // TODO: remove since we're storing ALL equipped weapons.
    //         string name = SaveManager.Load<string>("currentWeapon");
    //         _currentWeapon = name == "none" ? null : GunDictionary[name];
    //         return _currentWeapon;
    //     } 

    //     private set
    //     {
    //         string name = value == null ? "none" : GunNameDictionary[value];
    //         SaveManager.Save<string>("currentWeapon", name);
    //         _currentWeapon = name == "none" ? null : GunDictionary[name];
    //     } 
    // }

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

    public UnityEvent OnAmmoAmountChanged;
    public UnityEvent OnWeaponChanged;

    public Dictionary<GameObject, string> GunNameDictionary = new Dictionary<GameObject, string>();
    public Dictionary<string, GameObject> GunDictionary = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        playerController = GetComponent<PlayerController>();

        foreach (GameObject weapon in AllGunPrefabList)
        {
            GunDictionary.Add(weapon.name, weapon);
            GunNameDictionary.Add(weapon, weapon.name);
        }

        if (ResetOnLoad)
        {
            currentWeaponObj = null;
            currentAmmo = 0f;
        } else {
            _loadEquippedWeapons();
        }

        currentAmmo = 1f;
        
        HideGun();
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

    private IEnumerator _ShootCR()
    {
        currentWeapon.shootSfx.Post(gameObject);
        EventBus.Publish(new PlayerShootEvent(currentWeapon));
        
        _isFiring = true;
        ShowGun();

        yield return new WaitForSeconds(currentWeapon.cdTime);

        _isFiring = false;
        HideGun();
    }

    private void Update() {
        // Control gun sprite direction
        float angle = Vector2.SignedAngle(Vector2.up, playerController.aimInput);
        currentWeaponObj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void ShowGun()
    {
        SpriteRenderer sr = currentWeaponObj.GetComponentInChildren<SpriteRenderer>();
        Color tmp = sr.color;
        tmp.a = 1;
        sr.color = tmp;
    }

    private void HideGun()
    {
        SpriteRenderer sr = currentWeaponObj.GetComponentInChildren<SpriteRenderer>();
        Color tmp = sr.color;
        tmp.a = 0;
        sr.color = tmp;
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

        if (currentWeaponObj != null)
            Destroy(currentWeaponObj);
        
        SetNewWeapon(equippedWeaponPrefabs[_equippedWeaponIndex]);
        OnWeaponChanged.Invoke();
    }

    //<summary>
    // asdf
    //</summary>
    private void SetNewWeapon(GameObject weaponPrefab)
    {
        GameObject newWeapon = GameObject.Instantiate(weaponPrefab, transform, false);
        newWeapon.name = weaponPrefab.name;
        currentWeaponObj = newWeapon;
        currentWeaponObj.transform.localPosition = Vector3.zero;
        float angle = Vector2.SignedAngle(Vector2.up, playerController.aimInput);
        currentWeaponObj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Enable Gun when player touches a gun pickup
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GunPickup manager = collision.gameObject.GetComponent<GunPickup>();
        if (manager != null)
        {
            Debug.Log("(PlayerGun) Enable Gun!");
            equippedWeaponPrefabs.Add(manager.weaponPrefab);
            currentAmmo = 1;
            Destroy(collision.gameObject);
        }
    }
}
