using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    public static PlayerInventoryManager Instance { get; private set; }

    public int keys { get; private set; }

    public int currency { get; private set; }

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

