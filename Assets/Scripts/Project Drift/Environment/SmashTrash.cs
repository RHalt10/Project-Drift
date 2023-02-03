using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


/*
 * Smash trash script
 * Written by Brandon Fox 
 * 
 * User Guide:
        1. Place smash trash prefab in desired location in scene 
        2. Place GameObjects you want smash trash to drop upon destroying into droppedObjects list on inspector
        3. From the hierarchy, drag smash trash into Smash Trash list on Environment Interactables Manager inspector
        4. Can set if individual smash trash will respawn and how many hits it takes to destroy in smash trash inspector
 */
public class SmashTrash : MonoBehaviour
{
    [Header("Item Drop Settings")]
    [Tooltip("Place objects that you want the smash trash to drop here")]
    public GameObject[] droppedObjects;

    [Header("Smash Trash Respawn Settings")]
    [Tooltip("Do you want the smash trash to respawn? By defualt set to false")]
    public bool respawn = false;

    [Header("Health Settings")]
    [Tooltip("How many hits to break? By defualt set to 1 hit")]
    public int health = 1;
    [Tooltip("make random upon instantiation? if so choose range. Default is false")]
    public bool randomHealth = false;
    [Tooltip("healthMin MUST BE >= 1. Default is 1")]
    public int healthMin = 1;
    public int healthMax = 5;

    private int currentHealth;

    private void Start()
    {
        if (health < 1) { health = 1; }
        if (healthMin < 1) { healthMin = 1; }   
        if (randomHealth) { RandomizeHealth(); }
        currentHealth = health;
    }

    public void TakeHit()
    {
        --currentHealth;
        if (currentHealth == 0) { DropAndDestroy(); }
    }

    public void Respawn()
    {
        currentHealth = health;
    }

    private void DropAndDestroy()
    {
        for (int i = 0; i < droppedObjects.Length; i++)
        {
            Instantiate(droppedObjects[i], transform.position, Quaternion.identity);
        }
        if (respawn) { FindObjectOfType<EnvironmentManager>().InitiateRespawn(0);}
        gameObject.SetActive(false);
    }

    private void RandomizeHealth()
    {
        currentHealth = Random.Range(healthMin, healthMax + 1);
    }

}
