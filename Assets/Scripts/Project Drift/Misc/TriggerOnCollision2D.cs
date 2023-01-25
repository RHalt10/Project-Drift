using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using WSoft.Math;

public class TriggerOnCollision2D : MonoBehaviour
{
    [Tooltip("Only triggers objects on these layers.")]
    public LayerMask triggerLayers;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (LayermaskFunctions.IsInLayerMask(triggerLayers, collision.gameObject.layer))
        {
            collision.gameObject.GetComponent<SwitchController>().Flip();
        }
    }
}
