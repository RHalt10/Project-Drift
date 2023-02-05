using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Core;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// Attach to player character. Reloads scene on death 
/// and teleports player to last checkpoint location.
/// Written by Kevin Han
/// </summary>  
public class PlayerCheckpointManager : MonoBehaviour
{
    /// <summary>
    /// Class for checkpoint information (name and location)
    /// </summary>    
    [Serializable]
    public class CheckpointInfo
    {
        public string name = "";
        public int WeaponID;
        public float Ammo;
        private float _xLocation = 0;
        private float _yLocation = 0;
        public CheckpointInfo(string name_, int _weaponID, float _ammo, Vector2 location)
        {
            name = name_;
            WeaponID = _weaponID;
            Ammo = _ammo;
            _xLocation = location.x;
            _yLocation = location.y;
        }

        public Vector2 Getlocation()
        {
            return new Vector2(_xLocation, _yLocation);
        }
    }
    
    // Checkpoint to respawn player at
    public static CheckpointInfo current_checkpoint
    {
        get { return SaveManager.Load<CheckpointInfo>("current_checkpoint"); }
        set { SaveManager.Save<CheckpointInfo>("current_checkpoint", value); }
    }
    PlayerHealth playerHealth;

    // Start is called before the first frame update
    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerHealth.events.OnDeath.AddListener(SceneReload);
        
        // Avoid nullref in GroundCharacterController script.
        StartCoroutine(LateStart());
    }
    
    private IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        RespawnPlayer();
    }

    private void SceneReload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Teleport player to last checkpoint location and restore health/stamina.
    private void RespawnPlayer()
    {
        if(current_checkpoint != null) 
        {
            PlayerGun gunManager = GetComponent<PlayerGun>();
            
            gunManager._equippedWeaponIndex = current_checkpoint.WeaponID;
            gunManager.LoadEquippedWeapons();

            gunManager.currentAmmo = current_checkpoint.Ammo;
            
            GetComponent<GroundCharacterController>().Teleport(current_checkpoint.Getlocation());
        }
        
        playerHealth.Heal(100);
        // TODO: restore stamina to full (already complete if scene starts with full stamina)
    }

    public CheckpointInfo DebugCP;

    private void Update()
    {
        DebugCP = current_checkpoint;
    }
}
