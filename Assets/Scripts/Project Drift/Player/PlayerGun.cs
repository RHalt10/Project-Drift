using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WSoft.Combat;

/// <summary>
/// A script that controls the player shooting and ammo amounts
/// Note that it is not a player subcontroller
/// Written by Nikhil Ghosh '24
/// </summary>
public class PlayerGun : MonoBehaviour
{
    [SerializeField] GameObject startingWeaponObj;
    public GameObject currentWeaponObj { get; private set; }
    public RangedWeaponBase currentWeapon 
    { 
        get 
        { 
            RangedWeaponBase weaponInfo = currentWeaponObj.GetComponent<RangedWeaponBase>(); 
            if(weaponInfo == null) { Debug.LogError("Current weapon '" + startingWeaponObj.name + "' not a weapon"); }
            return weaponInfo; 
        }
        private set{} 
    }
    /* Always from 0 to 1*/
    float _currentAmmo = 0f;
    public float currentAmmo
    {
        get { return _currentAmmo; }
        set
        {
            _currentAmmo = Mathf.Clamp(value, 0, 1);
            OnAmmoAmountChanged.Invoke();
        }
    }

    private bool _isFiring = false;
    private float _defaultGroundSpeed;

    [SerializeField] Transform projectileSpawnPoint;

    public UnityEvent OnAmmoAmountChanged;
    public UnityEvent OnWeaponChanged;

    // Start is called before the first frame update
    void Awake()
    {
        _defaultGroundSpeed = GetComponent<PlayerController>().groundController.groundSpeed;
        currentWeaponObj = startingWeaponObj;
        currentAmmo = 1f;
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
        
        PlayerController playerMovement = GetComponent<PlayerController>();
        GroundCharacterController playerGround = GetComponent<GroundCharacterController>();
        
        _isFiring = true;

        yield return new WaitForSeconds(currentWeapon.cdTime);

        _isFiring = false;
    }

    private void Update() {
            
    }

    private void ShowGun()
    {

    }

    private void HideGun()
    {

    }

    public void RechargeAmmo(float percentage)
    {
        currentAmmo = currentAmmo + percentage;
    }

    public void RechargeSingleAmmo()
    {
        currentAmmo = currentAmmo + currentWeapon.BulletPercentage;
    }

    public void SwapWeapons(GameObject newWeapon)
    {
        currentWeaponObj = newWeapon;
        OnWeaponChanged.Invoke();
    }
}
