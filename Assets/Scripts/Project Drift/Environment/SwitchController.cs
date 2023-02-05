using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * A class that handles the basic operations of a switch
 * Written by Brandon Fox
 * 
 * User Guide:
        1. Place switch prefab in desired location in scene 
        2. Place GameObjects you want to appear/disappear (bridges, etc.) in scene at desired locations
        3. From the hierarchy, drag GameObjects into public controlledObjects array (If you want all the child of a gameobject to swap states, just drag the an empty parent gameobject in NOTE: THE PARENT WONT SWAP STATES JUST THE CHILDREN!)
            3a. This can be found by selecting the switch prefab and looking at the inspector
        4. Be sure to set active states of objects to desired states on start (active or inactive)
        5. Each time switch is hit, all controlled objects will have their active states reversed
 */

public class SwitchController : MonoBehaviour
{

    [Tooltip("Place objects that you want the switch to link to here")]
    public GameObject[] controlledObjects; //put into the order you want them to switch states of, if onEndEncounterAlsoSwitch set true, dont put any items from onEncounterSwitchState in here
    [Range(0, 2)]
    public float timeBetweenEachSwitch; //time between each switch option for jUiCe - allie

    private bool state = false;

    [SerializeField] AK.Wwise.Event switchSFX;

    //to let encounter system or any script with a listener know that coroutine for switching finished
    //ex: incase want player movement or some enemies movement disabled until switched
    public UnityEvent FinishedSwitching;

    private void Awake()
    {
        FinishedSwitching = new UnityEvent();
    }

    public void Flip()
    {
        state = !state;
        //no coroutine needed if 0
        if (timeBetweenEachSwitch == 0)
        {
            SwapStates();
        }
        else
        {
            StartCoroutine(SwapStatesStaggered());
        }

        switchSFX.Post(gameObject);

    }

    private void SwapStates()
    {
        for (int i = 0; i < controlledObjects.Length; i++)
        {

            if (controlledObjects[i].transform.childCount > 0)
            {
                //incase wanna group by parenting
                foreach (Transform child in controlledObjects[i].transform)
                {
                    child.gameObject.SetActive(!child.gameObject.activeSelf);
                }
            }
            else
            {
                controlledObjects[i].gameObject.SetActive(!controlledObjects[i].gameObject.activeSelf);
            }

        }
        FinishedSwitching.Invoke();
    }

    IEnumerator SwapStatesStaggered()
    {
        for (int i = 0; i < controlledObjects.Length; i++)
        {
            if (controlledObjects[i].transform.childCount > 0)
            {
                //incase wanna group by parenting
                foreach (Transform child in controlledObjects[i].transform)
                {
                    child.gameObject.SetActive(!child.gameObject.activeSelf);
                    yield return new WaitForSeconds(timeBetweenEachSwitch);
                }
            }
            else
            {
                controlledObjects[i].gameObject.SetActive(!controlledObjects[i].gameObject.activeSelf);
                yield return new WaitForSeconds(timeBetweenEachSwitch);
            }

        }
        FinishedSwitching.Invoke();
        yield return null;
    }
}
