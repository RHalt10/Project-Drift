using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Environment Manager, used to keep track of smash trash, falling tiles
 * Written by Brandon Fox 
 * 
 * User Guide:
        1. Place environment interactables prefabs in designated lists in inspector
        2. Check individual settings for each environment interactable on inspector
        3. Have fun
 */
public class EnvironmentManager : MonoBehaviour
{

    [Header("Smash Trash")]
    public SmashTrash[] SmashTrash;

    [Tooltip("How long until respawn? By defualt set to 30 seconds")]
    public float sTRespawnTime = 30.0f;

    [Header("Falling Tiles")]
    public FallingTile[] FallingTiles;

    [Tooltip("How long until respawn? By defualt set to 30 seconds")]
    public float fTRespawnTime = 30.0f;

    private Coroutine SmashTrashRespawnCoroutineContainer;
    private Coroutine FallingTilesRespawnCoroutineContainer;

    // will change this to enum later
    public void InitiateRespawn(int type)
    {
        if (type == 0)
        {
            SmashTrashRespawnCoroutineContainer = StartCoroutine(SmashTrashRespawn());
        }
        if (type == 1)
        {
            FallingTilesRespawnCoroutineContainer = StartCoroutine(FallingTileRespawn());
        }
    }

    // Will try to respawn any smash trash in scene that has respawn set to true 
    private IEnumerator SmashTrashRespawn()
    {
         yield return new WaitForSeconds(sTRespawnTime);

         for (int i = 0; i < SmashTrash.Length; ++i)
            {
             if (SmashTrash[i].respawn)
                 {
                    SmashTrash[i].gameObject.SetActive(true);
                    SmashTrash[i].Respawn();
                 }

            }
         yield return null;
    }

    // tried to respawn any and all falling tiles in scene
    private IEnumerator FallingTileRespawn()
    {
        yield return new WaitForSeconds(fTRespawnTime);

        for (int i = 0; i < FallingTiles.Length; ++i)
        {
            if (FallingTiles[i].respawn)
            {
                FallingTiles[i].gameObject.SetActive(true);
            }
            
        }
        yield return null;
    }
}
