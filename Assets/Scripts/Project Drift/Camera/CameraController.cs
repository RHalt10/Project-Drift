using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    //disable virtual cameras at start that aren't chosen by cinemachine brain
    [SerializeField] bool disableNonActiveVCams;

    void Start()
    {
        if (disableNonActiveVCams)
        {
            foreach (Transform vcam in transform)
            {
                //if the camera is not the active one, disable it
                if (!(GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject == vcam.gameObject)) 
                {
                    vcam.gameObject.SetActive(false);
                }
            }
        }
    }
}
