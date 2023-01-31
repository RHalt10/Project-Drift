using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RangedWeaponBase : MonoBehaviour
{
    public string weaponName;
    [Tooltip("Min time between shots as well as animation length")] public float cdTime;
    public GameObject projectilePrefab;
    [Tooltip("Number of shots that can be fired given 100% ammo")] public int ammoSubdivisions; 
    public float BulletPercentage { get => 1f / ammoSubdivisions; }

    public float fireCooldown { get; private set; }
    public AK.Wwise.Event shootSfx;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
