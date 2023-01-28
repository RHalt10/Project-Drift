using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the Player's ability to respawn when they fall
/// Written by Nikhil Ghosh '24
/// </summary>
public class PlayerCheckpointManager : MonoBehaviour
{
    PlayerController playerController;
    GroundCharacterController characterController;
    Vector2 lastGroundPosition;

    [SerializeField] float timeUntilFall;
    [SerializeField] AK.Wwise.Event resetPositionSfx;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<GroundCharacterController>();
        characterController.OnFall.AddListener(OnFall);
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (playerController.currentController is PlayerGroundController && characterController.isOnGround)
        {
            if (characterController.velocity.magnitude < 0.01f)
                lastGroundPosition = transform.position;
            else
            {
                Vector2 nextPoint = (Vector2)transform.position + characterController.velocity * 0.08f;
                if (characterController.IsPointOnGround(nextPoint))
                    lastGroundPosition = transform.position;
            }
        }
    }

    void OnFall()
    {
        StartCoroutine(HandleFalling());
    }

    IEnumerator HandleFalling()
    {
        yield return new WaitForSeconds(timeUntilFall);

        characterController.Teleport(lastGroundPosition);
        resetPositionSfx.Post(gameObject);
    }
}
