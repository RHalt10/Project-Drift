using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIFade : MonoBehaviour
{
    public float fadeTime = 3.0f;

    private CanvasGroup targetCanvas;
    private bool fade = true;
    private bool isFading = false;

    // Start is called before the first frame update
    void Start()
    {
        targetCanvas = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            if (isFading)
            {
                return;
            }

            isFading = true;
            StartCoroutine(FadeUI());
        }
    }

    IEnumerator FadeUI()
    {
        float startAlpha = targetCanvas.alpha;
        float endAlpha = fade ? 0.0f : 1.0f;

        float startTime = Time.time;
        float progress = (Time.time - startTime) / fadeTime;
        while (progress < 1.0f)
        {
            targetCanvas.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);
            progress = (Time.time - startTime) / fadeTime;
            yield return null;
        }
        targetCanvas.alpha = endAlpha;

        fade = !fade;
        isFading = false;
    }
}
