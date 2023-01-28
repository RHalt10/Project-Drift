using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Core;

/// <summary>
/// asdf
/// </summary>
public class GameCheckpoint : MonoBehaviour
{
    public static Vector3 current_checkpoint
    {
        get { return SaveManager.Load<Vector3>("location"); }
        set { SaveManager.Save<Vector3>("location", value); }
    }
    public static string checkPointName;

    [Tooltip("Change where player respawns for this checkpoint zone.")]
    [SerializeField] private Vector3 respawnOffset;
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player")
        {
            current_checkpoint = transform.position + respawnOffset;
        }
    }

    // Draw editor gizmos
    private void OnDrawGizmos() {
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        // position where player would respawn
        Gizmos.DrawSphere(transform.position + respawnOffset, 0.1f);
        
        // checkpoint area
        Gizmos.DrawCube(transform.position, transform.localScale);    
    }
}

