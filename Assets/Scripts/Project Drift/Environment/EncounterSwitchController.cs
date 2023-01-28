using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;


/*
 * A class that handles the encounter switches
 * Written by Allie Lavacek
 * 
 * User Guide:
        1. Place encounterSwitch prefab in the room or location in scene near where the encounter is happening
            Note: Location doesn't actually matter, but since a gizmo is attached it'll just help visualize which rooms have an encounter switch
        2. Place GameObjects you want to appear/disappear (bushes, walls etc.) in scene at desired locations
        3. From the hierarchy, drag GameObjects into public controlledObjects array 
            3a. This can be found by selecting the encounterswitch prefab and looking at the inspector
        4. Be sure to set active states of objects to desired states on start (active or inactive)
        5. From hierarchy, drag enemy GameObjects to toDefeat array that need to be defeated in order for GameObjects in controlledObjects array to change
        6. When all enemies in toDefeat array are killed, all controlled objects will have their active states reversed
 */
public class EncounterSwitchController : SwitchController
{
    [SerializeField] GameObject[] toDefeat;
    int enemiesLeft;
    // Start is called before the first frame update
    void Start()
    {
        enemiesLeft = toDefeat.Length;
        foreach (GameObject enemy in toDefeat)
        {
            if (enemy.scene.IsValid())
            {
                //only if enemy still exists, removes issues of if designer deletes one
                //of the enemies and forgets to update the encounterSwitch prefab
                enemy.GetComponent<BasicHealth>().events.OnDeath.AddListener(EnemyDeath);
            }

        }
    }

    void EnemyDeath()
    {
        enemiesLeft--;

        if (enemiesLeft == 0)
        {
            //found in base class (switchController)
            Flip();
        }
    }


    
}
