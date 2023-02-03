using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingSFX : MonoBehaviour
{
    public AK.Wwise.Event walkingSFX;
    private PlayerController playerController;
    private PlayerSubController currentController;
    private PlayerSubController groundController;
    private float footstepInterval = 0.3f;
    private float currentTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        groundController = playerController.groundController;
    }

    // Update is called once per frame
    void Update()
    {
        currentController = playerController.currentController;

        if (currentController == groundController
            && playerController.movementInput != Vector2.zero)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= footstepInterval)
            {
                currentTime = 0f;
                walkingSFX.Post(gameObject);
            }
        }
    }
}
