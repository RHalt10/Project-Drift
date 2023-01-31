using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Environment Manager, used to keep track of smash trash so far
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
    public float smashTrashRespawnTime = 30.0f;

    public FallingTile[][] FallingTileGroups;

    [Tooltip("How long until respawn? By defualt set to 30 seconds")]
    public float fallingTileRespawnTime = 30.0f;
    private void Start()
    {
        StartCoroutine(SmashTrashRespawn());
    }

    // Will try to respawn any smash trash in scene that has respawn set to true each interval
    // At the moment using while(true) until we can check for game instance
    public IEnumerator SmashTrashRespawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(smashTrashRespawnTime);

            for (int i = 0; i < SmashTrash.Length; ++i)
            {
                if (SmashTrash[i].respawn)
                {
                    SmashTrash[i].gameObject.SetActive(true);
                    SmashTrash[i].Respawn();
                }

            }
        }
      
    }
}
