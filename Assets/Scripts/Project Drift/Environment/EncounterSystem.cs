using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;
using UnityEngine.Events;

enum EncounterStatus
{
    preEncounter,
    startEncounter,
    inEncounter,
    postEncounter
}

public class EncounterStartEvent
{
    public EncounterSystem encounter;
    
    public EncounterStartEvent (EncounterSystem _encounter)
    {
        encounter = _encounter;
    }
}

public class EncounterEndEvent
{
    public EncounterSystem encounter;

    public EncounterEndEvent(EncounterSystem _encounter)
    {
        encounter = _encounter;
    }
}

/*
 * A class that handles the encounter switches
 * Written by Allie Lavacek
 * 
 * User Guide:
        1. Place EncounterController prefab in the room or location in scene near where the encounter is happening
            Note: Location doesn't actually matter, but since a gizmo is attached it'll just help visualize which rooms have an encounter switch
            i. you also need to place a SampleOnEnterTriggerEncounter prefab or a gameobject with a OnEnterTriggerEncounter script attached for this to work, see OnEnterTriggerEncounter.cs for more info
        2. Place GameObjects you want to appear/disappear (bushes, walls etc.) in scene at desired locations, and fill corresponding switchController's controlledObject's array (see SwitchController script for more detail
        3. From the hierarchy, drag GameObjects into with SwitchController script attached accordingly, see notes in script for more detail 
        4. Be sure to set active states of objects to desired states on start (active or inactive)
            i. OnEnterTriggerEncounter will trigger flip of onEncounterSwitchState
            ii. Defeating all enemies in toDefeat array will trigger flip onEndEncounterSwitchState
 */
public class EncounterSystem : MonoBehaviour
{
    [SerializeField] GameObject[] toDefeat;

    [Header("Start of Encounter")]

    [Tooltip("Place the switch controller you want to happen on end encounter, or leave empty if none")]
    [SerializeField] SwitchController OnEncounterSwitch;


    [Space(20)]


    [Header("End of Encounter")]

    [Tooltip("Place the switch controller you want to happen on end encounter" +
        "If onEndEncounterAlsoSwitch is true, do not put anything from onEncounterSwitchState in here, or leave empty if none")]
    
    [SerializeField] SwitchController OnEndEncounterSwitch;


    [Space(20)]


    [Header("Events")]
    public UnityEvent OnStartEncounter;
    public UnityEvent OnEndEncounter;


    int enemiesLeft;
    EncounterStatus status; //incase same switchController is passed in


    private void Awake()
    {
        OnStartEncounter = new UnityEvent();
        OnEndEncounter = new UnityEvent();
    }
    void Start()
    {
        status = EncounterStatus.preEncounter;
        enemiesLeft = toDefeat.Length;

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
        if (OnEncounterSwitch)
        {
            OnEncounterSwitch.FinishedSwitching.AddListener(EnableMovement); // when encounter starts, player and enemies wont be able to move, then once switching ends, they both can
        }
        if (OnEndEncounterSwitch)
        {
            OnEndEncounterSwitch.FinishedSwitching.AddListener(EnablePlayerMovement); //when encounter ends, enemies will be dead, and we will freeze player movement just of a moment to add emphasis to switching objects
        }   

    }

    public void StartEncounter()
    {
        if (status == EncounterStatus.preEncounter)
        {
            status = EncounterStatus.startEncounter;
            //freeze player until flip is done
            GameObject.FindGameObjectWithTag("Player").GetComponent<GroundCharacterController>().enabled = false;
            GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            OnStartEncounter.Invoke(); //for audio <3 
            EventBus.Publish(new EncounterStartEvent(this));

            if (OnEncounterSwitch)
            {
                OnEncounterSwitch.GetComponent<SwitchController>().Flip(); //when finished EnableMovement will be called
            }
            else
            {
                EnableMovement();
            }
        }
    }

    void EnableMovement()
    {   
        if (status == EncounterStatus.startEncounter)
        {
            status = EncounterStatus.inEncounter;
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

    }

    void EnemyDeath()
    {
        enemiesLeft--;

        if (enemiesLeft == 0 && status == EncounterStatus.inEncounter)
        {
            status = EncounterStatus.postEncounter;
            //freeze player until flip is done
            GameObject.FindGameObjectWithTag("Player").GetComponent<GroundCharacterController>().enabled = false;
            GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            if (OnEndEncounterSwitch)
            {
                OnEndEncounterSwitch.GetComponent<SwitchController>().Flip();
            }
            else
            {
                EnablePlayerMovement();
            }

            OnEndEncounter.Invoke(); //for audio <3 
            EventBus.Publish(new EncounterEndEvent(this));
        }
    }

    void EnablePlayerMovement()
    {
        //give player movement again
        GameObject.FindGameObjectWithTag("Player").GetComponent<GroundCharacterController>().enabled = true;
    }
}
