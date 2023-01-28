using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


/*
 * A class that handles displaying text prompts
 * Written by Allie Lavacek
 * 
 * User Guide:
        1. Place textPromptTrigger prefab in the location you want the player to cross to display the prompt
        2. Adjust textToShow field to the text you want to be shown and display time accordingly
        3. Make sure textMeshProToChange is set to the textmeshpro in the scene you wish to change the text of
 */

public class TextPromptOnEnter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshProToChange;
    [SerializeField] string textToShow;
    [Range(2, 20)]
    [SerializeField] float displayTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //only player should trigger coroutine
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(ShowPrompt());
        }
    }

    IEnumerator ShowPrompt()
    {
        textMeshProToChange.gameObject.SetActive(true);
        textMeshProToChange.text = textToShow;

        float fadeOutAmount = 0.0f;
        textMeshProToChange.color = new Color(textMeshProToChange.color.r, textMeshProToChange.color.g, textMeshProToChange.color.b, 0);

        //fade in
        while (textMeshProToChange.color.a < 1)
        {
            fadeOutAmount = textMeshProToChange.color.a + (5.0f * Time.deltaTime);
            textMeshProToChange.color = new Color(textMeshProToChange.color.r, textMeshProToChange.color.g, textMeshProToChange.color.b, fadeOutAmount);

            yield return new WaitForSeconds(0.025f);
        }
        
        //fade in and out take about .5f seconds total
        yield return new WaitForSeconds(displayTime - 0.5f);

        //fade out
        while (textMeshProToChange.color.a > 0)
        {
            fadeOutAmount = textMeshProToChange.color.a - (5.0f * Time.deltaTime);
            textMeshProToChange.color = new Color(textMeshProToChange.color.r, textMeshProToChange.color.g, textMeshProToChange.color.b, fadeOutAmount);

            yield return new WaitForSeconds(0.025f);
        }

        textMeshProToChange.gameObject.SetActive(false);
        yield break;

    }
}
