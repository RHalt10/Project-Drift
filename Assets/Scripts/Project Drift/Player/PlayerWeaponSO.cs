using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Weapon", menuName = "Project Drift/Player Weapon")]
public class PlayerWeaponSO : ScriptableObject
{
    public string weaponName;
    public GameObject projectilePrefab;
    public int ammoSubdivisions;
    
    public float BulletPercentage { get => 1f / ammoSubdivisions; }
}
