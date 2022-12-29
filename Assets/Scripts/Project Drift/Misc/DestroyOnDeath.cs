using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Combat;

public class DestroyOnDeath : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<BasicHealth>().events.OnDeath.AddListener(OnDeath);
    }

    void OnDeath()
    {
        Destroy(gameObject);
    }
}
