using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedWeaponBase : MonoBehaviour
{
    public string weaponName;
    [Tooltip("Min time between shots as well as animation length")] public float cdTime;
    public GameObject projectilePrefab;
    [Tooltip("Number of shots that can be fired given 100% ammo")] public int ammoSubdivisions; 
    public float BulletPercentage { get => 1f / ammoSubdivisions; }
    public float fireCooldown { get; private set; }
    public AK.Wwise.Event shootSfx;

    /// <summary>
    /// Spawn player bullet projectiles based on FacingDirection. No need to implement costs 
    /// </summary>
    public abstract void FireProjectile(Vector2 direction, Vector2 spawnPoint);
}
