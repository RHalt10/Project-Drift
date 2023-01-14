using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Key : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerInventoryManager>().AcquireKey();
            EventBus.Publish(new KeyAcquiredEvent());
            Destroy(gameObject);
        }
    }
}
