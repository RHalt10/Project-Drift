using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Math;

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

    private Coroutine fallCoroutineContainer;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (LayermaskFunctions.IsInLayerMask(triggerLayers, collision.gameObject.layer))
        {
            GameObject gameObject = collision.gameObject;

            // if player is dashing
            if (gameObject.GetComponent<GroundCharacterController>()) { CheckFall(gameObject.GetComponent<GroundCharacterController>().willFallOnAir); }

        }
    }


    public void CheckFall(bool isDashing)
    {
        // if player is dashing, do not initiate fall sequence
        if (isDashing) { return; }

        // otherwise, begin fall sequence
        if (fallCoroutineContainer == null) { 
            fallCoroutineContainer = StartCoroutine(Fall());
        }
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(timeUntillFall);
        gameObject.SetActive(false);
    }
}
