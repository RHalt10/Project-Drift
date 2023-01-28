using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// For checkpoint object. Sets most recent checkpoint location to teleport to on death.
/// Written by Kevin Han
/// </summary>  
[RequireComponent(typeof(BoxCollider2D))]
public class GameCheckpoint : MonoBehaviour
{
    [Tooltip("Checkpoint identifier")]
    [SerializeField] string checkPointName;

    [Tooltip("Change where player respawns for this checkpoint zone.")]
    [SerializeField] private Vector2 respawnOffset = Vector2.zero;
    private Vector2 checkpointLocation;

    private void Start() {
        checkpointLocation = new Vector2(transform.position.x, transform.position.y) + respawnOffset;
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player")
        {
            PlayerRespawnManager.current_checkpoint =
            new PlayerRespawnManager.CheckpointInfo(checkPointName, checkpointLocation);
        }
    }

    // Draw editor gizmos
    private void OnDrawGizmos() {
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        // position where player would respawn
        Gizmos.DrawSphere((Vector2)transform.position + respawnOffset, 0.1f);
        
        // checkpoint area
        Gizmos.DrawCube(transform.position, transform.localScale);    
    }
}

