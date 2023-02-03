using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAudio : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event audioEvent;

    // Start is called before the first frame update
    void PostEvent()
    {
        audioEvent.Post(gameObject);
    }
}
