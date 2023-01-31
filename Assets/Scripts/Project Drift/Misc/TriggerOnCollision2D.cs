using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using WSoft.Math;

/*
 * Utilized by environment interactables such as the switch, smash trash
 * Written by Brandon Fox 
 */
public class TriggerOnCollision2D : MonoBehaviour
{
    [Tooltip("Only triggers objects on these layers.")]
    public LayerMask triggerLayers;

    // done like this until I can figure out why OnTriggerEnter doesn't work for bullets
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (LayermaskFunctions.IsInLayerMask(triggerLayers, collision.gameObject.layer))
        {
            GameObject gameObject = collision.gameObject;

            // if switch is hit
            if (gameObject.GetComponent<SwitchController>()) { gameObject.GetComponent<SwitchController>().Flip();  }

            // if smash trash is hit
            else if (gameObject.GetComponent<SmashTrash>()) { gameObject.GetComponent<SmashTrash>().TakeHit(); }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;

        // if switch is hit
        if (gameObject.GetComponent<SwitchController>()) { gameObject.GetComponent<SwitchController>().Flip(); }

        // if smash trash is hit
        else if (gameObject.GetComponent<SmashTrash>()) { gameObject.GetComponent<SmashTrash>().TakeHit(); }
    }
}
