using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;


//Created by Allie '23
//This script is transitions that take place specifically in the same scene
public class CameraTransitionTrigger : MonoBehaviour
{   
    
    [SerializeField] bool canReEnterCurrentArea;

    //black screen or image covering the screen while scene transition takes place
    [SerializeField] GameObject transitionScreen;

    [SerializeField] Vector3 teleportPlayerTo;

    //make the cinamachine you want to transition to has its confiner 2d component set (if you want bounds on that section of the map's camera)
    //if you do have a bounding shape set, make sure the cinamachine is inside those bounds.
    [SerializeField] CinemachineVirtualCamera cinamachine;

    [Range(1, 10)]
    [SerializeField] float transitionTime;
    private GameObject player;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            player = other.gameObject;
            StartCoroutine(FadeAndTeleport());

            if (!canReEnterCurrentArea)
            {
                gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }
    }

    IEnumerator FadeAndTeleport()
    {
        float fadeOutAmount = 0.0f;


        Color screenColor = transitionScreen.GetComponent<Image>().color;
        //should already have a set to 0 but just in case
        transitionScreen.GetComponent<Image>().color = new Color(screenColor.r, screenColor.g, screenColor.b, 0f);

        while (transitionScreen.GetComponent<Image>().color.a < 1)
        {
            fadeOutAmount = transitionScreen.GetComponent<Image>().color.a + (5.0f * Time.deltaTime * 1/(transitionTime/2));
            transitionScreen.GetComponent<Image>().color = new Color(screenColor.r, screenColor.g, screenColor.b, fadeOutAmount);

            yield return new WaitForSeconds(0.025f);
        }

        
        player.GetComponent<Transform>().position = teleportPlayerTo;
        //set new active camera 
        Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.SetActive(false);
        cinamachine.VirtualCameraGameObject.SetActive(true);
        yield return new WaitForSeconds(transitionTime/2);


        while (transitionScreen.GetComponent<Image>().color.a > 0)
        {
            fadeOutAmount = transitionScreen.GetComponent<Image>().color.a - (5.0f * Time.deltaTime * 1/(transitionTime/2));
            transitionScreen.GetComponent<Image>().color = new Color(screenColor.r, screenColor.g, screenColor.b, fadeOutAmount);

            yield return new WaitForSeconds(0.025f);
        }


        yield break;
    }
}
