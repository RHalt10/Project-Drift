using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionTextController : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        //turn make all children
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
