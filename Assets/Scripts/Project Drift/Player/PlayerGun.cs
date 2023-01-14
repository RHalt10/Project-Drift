using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WSoft.Combat;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] PlayerWeaponSO startingWeapon;
    public PlayerWeaponSO currentWeapon { get; private set; }

    int _currentAmmo = 0;
    public int currentAmmo
    {
        get { return _currentAmmo; }
        set
        {
            _currentAmmo = value;
            OnAmmoAmountChanged.Invoke();
        }
    }

    [SerializeField] Transform projectileSpawnPoint;

    public UnityEvent OnAmmoAmountChanged;
    public UnityEvent OnWeaponChanged;

    // Start is called before the first frame update
    void Awake()
    {
        currentWeapon = startingWeapon;
        currentAmmo = currentWeapon.maxAmmo;
    }

    public void Shoot(Vector2 direction)
    {
        if(currentAmmo == 0)
            return;

        currentAmmo--;

        GameObject spawnedProjectile = Instantiate(currentWeapon.projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        spawnedProjectile.GetComponent<ProjectileMovement2D>().direction = direction.normalized;

        EventBus.Publish(new PlayerShootEvent(currentWeapon));
    }

    public void RechargeSingleAmmo()
    {
        currentAmmo = Mathf.Min(currentAmmo + 1, currentWeapon.maxAmmo);
    }

    public void SwapWeapons(PlayerWeaponSO newWeapon)
    {
        currentWeapon = newWeapon;
        OnWeaponChanged.Invoke();
    }
}
