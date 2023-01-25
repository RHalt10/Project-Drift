using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public GameObject[] controlledObjects;

    private bool state = false;
   
    public void Flip()
    {
        state = !state;
        SwapStates();
    }

    private void SwapStates()
    {
        for (int i = 0; i < controlledObjects.Length; i++)
        {
            controlledObjects[i].gameObject.SetActive(!controlledObjects[i].gameObject.activeSelf);
        }
    }
}
