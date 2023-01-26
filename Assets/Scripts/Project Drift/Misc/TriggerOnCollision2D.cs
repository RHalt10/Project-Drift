using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using WSoft.Math;

/*
 * Utilized by environment interactables, such as the switch
 * Written by Brandom Fox 
 */
public class TriggerOnCollision2D : MonoBehaviour
{
    [Tooltip("Only triggers objects on these layers.")]
    public LayerMask triggerLayers;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (LayermaskFunctions.IsInLayerMask(triggerLayers, collision.gameObject.layer))
        {
            GameObject gameObject = collision.gameObject;
            if (gameObject.GetComponent<SwitchController>() != null) { gameObject.GetComponent<SwitchController>().Flip();  }
        }
    }
}
