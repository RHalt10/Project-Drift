using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;
using UnityEngine.Events;


/*
 * A class that handles the encounter switches
 * Written by Allie Lavacek
 * 
 * User Guide:
        1. Place EncounterController prefab in the room or location in scene near where the encounter is happening
            Note: Location doesn't actually matter, but since a gizmo is attached it'll just help visualize which rooms have an encounter switch
            i. you also need to place a SampleOnEnterTriggerEncounter prefab or a gameobject with a OnEnterTriggerEncounter script attached for this to work, see OnEnterTriggerEncounter.cs for more info
        2. Place GameObjects you want to appear/disappear (bushes, walls etc.) in scene at desired locations
        3. From the hierarchy, drag GameObjects into onEncounterSwitchState and onEndEncounterSwitchState array accordingly, see notes in script for more detail 
            i. This can be found by selecting the encounterController prefab and looking at the inspector
        4. Be sure to set active states of objects to desired states on start (active or inactive)
            i. OnEnterTriggerEncounter will flip onEncounterSwitchState
            ii. Defeating all enemies in toDefeat array will flip onEndEncounterSwitchState (and onEncounterSwitchState if onEndEncounterAlsoSwitch)
        5. From hierarchy, drag enemy GameObjects to toDefeat array that need to be defeated in order for GameObjects in 
            onEndEncounterSwitchState (and onEncounterSwitchState if onEndEncounterAlsoSwitch is true) array to change
 */
public class EncounterSystem : MonoBehaviour
{
    [SerializeField] GameObject[] toDefeat;

    [Header("Start of Encounter")]

    [Tooltip("Place the gameobjects you want the state of to switch when the encounter begins")]
    [SerializeField] GameObject[] onEncounterSwitchState; //put into the order you want them to switch states of

    [Tooltip("if every item in the onEncounterSwitchState you also want to switch state when the encounter ends, just check this box")]
    [SerializeField] bool onEndEncounterAlsoSwitch; //if every item in the onEncounterSwitchState you also want to switch state when the encounter ends, just check this box 



    [Space(10)]
    [Range(0,2)]
    [SerializeField] float onEncounterTimeBetweenEachSwitch; //set to 0 for all to switch at once



    [Space(20)]



    [Header("End of Encounter")]

    [Tooltip("Place the gameobjects you want the state of to switch when the encounter ends." +
        "If onEndEncounterAlsoSwitch is true, do not put anything from onEncounterSwitchState in here")]

    [SerializeField] GameObject[] onEndEncounterSwitchState; //put into the order you want them to switch states of, if onEndEncounterAlsoSwitch set true, dont put any items from onEncounterSwitchState in here   

    

    [Space(10)]
    [Range(0, 2)]
    [SerializeField] float onEndEncounterTimeBetweenEachSwitch; //set to 0 for all to switch at once


    [Space(20)]


    [Header("End of Encounter")]
    public UnityEvent OnStartEncounter;
    public UnityEvent OnEndEncounter;


    int enemiesLeft;
    //temp instances that will be destroyed on end encounter
    GameObject OnEncounterSwitch;
    GameObject OnEndEncounterSwitch;

    private void Awake()
    {
        OnStartEncounter = new UnityEvent();
        OnEndEncounter = new UnityEvent();
    }
    void Start()
    {
        enemiesLeft = toDefeat.Length;

        //temp switch controller instances for encounter system 
        OnEncounterSwitch = new GameObject("On Encounter Switch");
        SwitchController controllerOn = OnEncounterSwitch.AddComponent<SwitchController>();
        OnEndEncounterSwitch = new GameObject("On End Encounter Switch");
        SwitchController controllerEnd = OnEndEncounterSwitch.AddComponent<SwitchController>();

        //set variables of switch controllers components
        controllerOn.controlledObjects = onEncounterSwitchState;
        controllerOn.timeBetweenEachSwitch = onEncounterTimeBetweenEachSwitch;

        controllerEnd.timeBetweenEachSwitch = onEncounterTimeBetweenEachSwitch;
        if (onEndEncounterAlsoSwitch)
        {
            //if the ones given in onEncounterSwitchState is also expect to switch on endCounter, must merge arrays for switch controller
            GameObject[] endSwitches = new GameObject[onEncounterSwitchState.Length + onEndEncounterSwitchState.Length];
            onEncounterSwitchState.CopyTo(endSwitches, 0);
            onEndEncounterSwitchState.CopyTo(endSwitches, onEncounterSwitchState.Length);

            controllerEnd.controlledObjects = endSwitches;
        }
        else
        {
            controllerEnd.controlledObjects = onEndEncounterSwitchState;
        }

        //check enemy is still alive
        foreach (GameObject enemy in toDefeat)
        {
            if (enemy.scene.IsValid())
            {
                //only if enemy still exists, removes issues of if designer deletes one
                //of the enemies and forgets to update the encounterSwitch prefab
                enemy.GetComponent<BasicHealth>().events.OnDeath.AddListener(EnemyDeath);

                //disable movement until start encounter
                if (enemy.GetComponent<GroundCharacterController>())
                {
                    enemy.GetComponent<GroundCharacterController>().enabled = false;
                }
            }

        }

        //this wont be noticeable or do anything if onEncounterTimeBetweenEachSwitch or onEndEncounterTimeBetweenEachSwitch is 0, but if it isn't itll add to
        //juice and also make for cleaner game play
        controllerOn.FinishedSwitching.AddListener(EnableMovement); // when encounter starts, player and enemies wont be able to move, then once switching ends, they both can
        controllerEnd.FinishedSwitching.AddListener(EnablePlayerMovement); //when encounter ends, enemies will be dead, and we will freeze player movement just of a moment to add emphasis to switching objects
    }

    public void StartEncounter()
    {
        //freeze player until flip is done
        GameObject.FindGameObjectWithTag("Player").GetComponent<GroundCharacterController>().enabled = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        OnStartEncounter.Invoke(); //for audio <3 
        OnEncounterSwitch.GetComponent<SwitchController>().Flip(); //when finished EnableMovement will be called


    }

    void EnableMovement()
    {   
        //give player movement again
        GameObject.FindGameObjectWithTag("Player").GetComponent<GroundCharacterController>().enabled = true;

        //let enemies move
        foreach (GameObject enemy in toDefeat)
        {
            if (enemy.GetComponent<GroundCharacterController>())
            {
                enemy.GetComponent<GroundCharacterController>().enabled = true;
            }
        }
    }

    void EnemyDeath()
    {
        enemiesLeft--;

        if (enemiesLeft == 0)
        {
            //freeze player until flip is done
            GameObject.FindGameObjectWithTag("Player").GetComponent<GroundCharacterController>().enabled = false;
            GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().velocity = Vector2.zero;


            OnEndEncounterSwitch.GetComponent<SwitchController>().Flip();

            //event
            OnEndEncounter.Invoke();

            //make sure endEncounter can finish first
            Invoke("Destroy(OnEncounterSwitch)", 10);
            Invoke("Destroy(OnEndEncounterSwitch)", 10);
            OnEndEncounter.Invoke(); //for audio <3 
        }
    }

    void EnablePlayerMovement()
    {
        //give player movement again
        GameObject.FindGameObjectWithTag("Player").GetComponent<GroundCharacterController>().enabled = true;
    }
}
