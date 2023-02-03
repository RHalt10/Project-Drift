using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Math;


/*
 * Falling Tile script
 * Written by Brandon Fox 
 * 
 * User Guide:
        1. Place falling tile prefab in desired location in scene 
        3. From the hierarchy, drag Falling Tile prefab into Falling Tile list on Environment Interactables Manager inspector
        4. Can set if individual falling tile will respawn and how long it takes for it to fall
 */
public class FallingTile : MonoBehaviour
{
    // simply check to see if willFallOnAir value to determine if dashing or not over tile
    // if not dashing, tile will fall, if dashing then doesnt trigger fall
    [Tooltip("Only triggers objects on these layers.")]
    public LayerMask triggerLayers;

    // make custom fall time 
    [Header("Settings")]
    [Tooltip("How long can a player stand on this before it falls? Default is 1 second")]
    public float timeUntillFall = 1f;
    [Tooltip("Will the tile respawn? Default false")]
    public bool respawn = false;

    private Coroutine fallCoroutineContainer;

    private void Start()
    {
        if (timeUntillFall < 0) { timeUntillFall = 0.01f; }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (LayermaskFunctions.IsInLayerMask(triggerLayers, collision.gameObject.layer))
        {
            GameObject gameObject = collision.gameObject;

            // if player is dashing
            if (gameObject.GetComponent<GroundCharacterController>()) { CheckFall(gameObject.GetComponent<GroundCharacterController>().willFallOnAir); }

        }
    }


    public void CheckFall(bool isWalking)
    {
        // if player is dashing, do not initiate fall sequence
        if (!isWalking) { return; }

        // otherwise, begin fall sequence
        fallCoroutineContainer = StartCoroutine(Fall());
        
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(timeUntillFall);
        FindObjectOfType<EnvironmentManager>().InitiateRespawn(1);
        gameObject.SetActive(false);
        yield return null;
    }
}
