using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A class that handles the basic operations of a switch
 * Written by Brandon Fox 
 * 
 * User Guide:
        1. Place switch prefab in desired location in scene 
        2. Place GameObjects you want to appear/disappear (bridges, etc.) in scene at desired locations
        3. From the hierarchy, drag GameObjects into public controlledObjects array 
            3a. This can be found by selecting the switch prefab and looking at the inspector
        4. Be sure to set active states of objects to desired states on start (active or inactive)
        5. Each time switch is hit, all controlled objects will have their active states reversed
 */

public class SwitchController : MonoBehaviour
{

    [Tooltip("Place objects that you want the switch to link to here")]
    public GameObject[] controlledObjects;

    private bool state = false;
   
    public void Flip()
    {
        state = !state;
        SwapStates();
    }

    private void SwapStates()
    {
        for (int i = 0; i < controlledObjects.Length; i++)
        {
            controlledObjects[i].gameObject.SetActive(!controlledObjects[i].gameObject.activeSelf);
        }
    }
}
