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
    [SerializeField] List<GameObject> equippedWeaponPrefabs;
    private int _equippedWeaponIndex = 0;
    public GameObject currentWeaponObj 
    {
        get;
        private set;
    }

    public RangedWeaponBase currentWeapon 
    { 
        get 
        { 
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

    // Start is called before the first frame update
    void Awake()
    {
        playerController = GetComponent<PlayerController>();

        if(equippedWeaponPrefabs.Count != 0) {
            currentWeaponObj = equippedWeaponPrefabs[0];
            SetNewWeapon(equippedWeaponPrefabs[_equippedWeaponIndex]);
        } else {
            currentWeaponObj = null;
        }
        
        currentAmmo = 1f;
        HideGun();
    }

    public void Shoot(Vector2 direction)
    {
        if(!_isFiring)
        {
            if(currentAmmo < currentWeapon.BulletPercentage)
                return;

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

    private void SetNewWeapon(GameObject weaponPrefab)
    {
        currentWeaponObj = GameObject.Instantiate(weaponPrefab, transform, false);
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
