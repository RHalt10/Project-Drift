using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component that stores the player's current inventory
/// Written by Nikhil Ghosh '24
/// </summary>
public class PlayerInventoryManager : MonoBehaviour
{
    public static PlayerInventoryManager Instance { get; private set; }

    public int keys { get; private set; }

    public int currency { get; private set; }

    public AK.Wwise.Event keyAcquiredSfx;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        keys = 0;
    }

    public void AcquireKey()
    {
        keys++;
        keyAcquiredSfx.Post(gameObject);
    }

    // allows for setting specific values
    public void SetValue(string resource, int value)
    {
        if (resource == "keys")
        {
            keys = value;
        }
        else if (resource == "currency")
        {
            currency = value;
        }

        // return for all other resource string values
        return;
    }
}

