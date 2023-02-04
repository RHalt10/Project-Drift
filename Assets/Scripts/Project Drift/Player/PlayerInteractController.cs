using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A player subcontroller that controls interactions with interactable objects
/// </summary>
[System.Serializable]
public class PlayerInteractController : PlayerSubController
{

    public InteractableManager interactTarget;

    private float t;
    private float interactTime;
    private bool interacting;

    public override void Initialize()
    {

    }

    public override void OnDisable()
    {
        
    }

    public override void OnEnable()
    {
        t = 0;
        interacting = false;
    }

    public override void RecieveInput(PlayerInputType type)
    {
        if (type == PlayerInputType.StopInteract) {
            playerController.SetController(playerController.groundController);
        }
        else if (type == PlayerInputType.Interact)
        {
            foreach (GameObject obj in playerController.currentCollisions) {
                if(obj.gameObject.GetComponent<InteractableManager>() != null) {
                    interactTarget = obj.gameObject.GetComponent<InteractableManager>();
                }
            }
            Debug.Log(interactTarget);
            if(interactTarget != null) {
                interactTime = interactTarget.interactTime;
                interacting = true;

            } else {
                playerController.SetController(playerController.groundController);
            }
        }
    }

    public override void Update() {
        if(interacting) {
            if(t < interactTime) {
                t += Time.deltaTime;
            } else {
                interactTarget.CompleteInteraction.Invoke();
                interacting = false;
                playerController.SetController(playerController.groundController);
            }
        }

    }
}
