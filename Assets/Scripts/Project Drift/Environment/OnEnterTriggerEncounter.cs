using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * A class that handles the triggering the encounter event
 * Written by Allie Lavacek
 * 
 * User Guide:
        1. Place OnEnterTriggerEncounter script onto a gameobject with a 2d collider set to isTrigger
            i. This trigger will be responsible for starting the encounter event when the player game object enters it
            ii. A SampleOnEnterTriggerEncounter prefab of is available in the prefab level design folder
        2. Set up an encounter controller for the area, instruction how to do so can be found in the EncounterSystem script
 */
public class OnEnterTriggerEncounter : MonoBehaviour
{
    [SerializeField] EncounterSystem encounterController;

    void Start()
    {
        //just incase!
        GetComponent<BoxCollider2D>().isTrigger = true;      
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            encounterController.StartEncounter();

            //no longer need to be active once we enter
            gameObject.SetActive(false);

        }
    }
}
