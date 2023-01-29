using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpOverTime : MonoBehaviour
{
    // public variables to allow for changing the shake parameters for testing
    public float lerpTime = 1.0f;
    public float[] offsetValues = { 0.0f, 100.0f, 0.0f };

    // private variables to store references to transform and start/end locations
    private Transform UITransform;
    private Vector3 startPos;
    private Vector3 endPos;

    // to prevent spamming
    private bool isMoving = false;

    private void Start()
    {
        // set the appropriate transform, as well as start and end positions
        UITransform = GetComponent<RectTransform>().transform;
        startPos = UITransform.position;
        endPos = new Vector3(UITransform.position.x + offsetValues[0], UITransform.position.y + offsetValues[1], UITransform.position.z + offsetValues[2]);
    }

    // used to test out ShakeUI()
    private void Update()
    {
        // use U key to trigger shake event
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("Pressed U");
            if (isMoving)
            {
                Debug.Log("In progress. Try again later.");
                return;
            }

            isMoving = true;

            StartCoroutine(MoveSequence(UITransform, startPos, endPos));
        }
    }

    // helper function used for debugging
    private IEnumerator MoveSequence(Transform objTransform, Vector3 startPos, Vector3 endPos)
    {
        yield return StartCoroutine(MoveOverTime(objTransform, startPos, endPos));
        yield return StartCoroutine(MoveOverTime(objTransform, endPos, startPos));

        isMoving = false;
    }

    // coroutine to lerp ui element to produce shaking effect
    private IEnumerator MoveOverTime(Transform objTransform, Vector3 startPos, Vector3 endPos)
    {
        // time of start of lerp
        float startTime = Time.time;
        // update proportion of distance to EndPos over time
        float proportion = (Time.time - startTime) / (lerpTime);
        // while progress is less than 0, move and update proportion
        while (proportion < 1.0f)
        {
            objTransform.position = new Vector3(Mathf.Lerp(startPos.x, endPos.x, proportion), Mathf.Lerp(startPos.y, endPos.y, proportion), Mathf.Lerp(startPos.z, endPos.z, proportion));
            proportion = (Time.time - startTime) / (lerpTime);
            yield return null;
        }
        // finalize translate to EndPos
        objTransform.position = endPos;
    }
}
