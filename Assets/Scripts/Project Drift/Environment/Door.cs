using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    public UnityEvent OnDoorAttemptedEntry;
    public UnityEvent OnDoorUnlocked;

    public int keysNeeded;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInventoryManager inventoryManager = collision.gameObject.GetComponent<PlayerInventoryManager>();
            if (inventoryManager.keys >= keysNeeded)
            {
                Open();
            }
            else
            {
                OnDoorAttemptedEntry.Invoke();
            }
        }
    }

    void Open()
    {
        OnDoorUnlocked.Invoke();
        Destroy(gameObject);
    }
}
