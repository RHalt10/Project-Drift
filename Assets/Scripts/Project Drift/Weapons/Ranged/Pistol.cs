using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;

public class Pistol : RangedWeaponBase
{
    public override void FireProjectile(Vector2 direction, Vector2 spawnPoint)
    {
        GameObject spawnedProjectile = Instantiate(projectilePrefab, spawnPoint, Quaternion.identity);
        spawnedProjectile.GetComponent<ProjectileMovement2D>().direction = direction.normalized;
    }
}
