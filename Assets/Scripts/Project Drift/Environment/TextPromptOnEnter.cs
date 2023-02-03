using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


/*
 * A class that handles displaying text prompts
 * Written by Allie Lavacek
 * 
 * User Guide:
        1. Place textPromptTrigger prefab in the location you want the player to cross to display the prompt
        2. Adjust textToShow field to the text you want to be shown and display time accordingly
        3. From hierarchy place the InstructionTextSystem into the InstructionTextSystem part of the inspector, 
            this should only have a gameobject w/ a TextMeshProUGUI component childed and possibly a gameobject w/ Image component
        4. Make sure textMeshProToChange is set to the textmeshpro in the scene you wish to change the text of
 */

public class TextPromptOnEnter : MonoBehaviour
{
    [SerializeField] InstructionTextController InstructionTextSystem;
    [SerializeField] string textToShow;
    [SerializeField] bool useImageBackground; //black block unless told otherwise

    [Range(0, 50)]
    [SerializeField] float paddingForBackgroundBox;
    [Range(2, 20)]
    [SerializeField] float displayTime;

    TextMeshProUGUI textMeshProToChange;
    Image backgroundBox;
    bool textBeingShown; //track whether in coroutine for cleaner fade in and out

    private void Start()
    {
        foreach (Transform child in InstructionTextSystem.gameObject.transform)
        {
            if (child.gameObject.GetComponent<TextMeshProUGUI>())
            {
                textMeshProToChange = child.gameObject.GetComponent<TextMeshProUGUI>();
            }
            else if (useImageBackground && child.gameObject.GetComponent<Image>())
            {
                backgroundBox = child.gameObject.GetComponent<Image>();
            }
        }

        textBeingShown = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //only player should trigger coroutine
        if (collision.gameObject.tag == "Player" && !textBeingShown)
        {
            StartCoroutine(ShowPrompt());
        }
    }

    IEnumerator ShowPrompt()
    {
        textBeingShown = true; //track whether in coroutine for cleaner fade in and out

        textMeshProToChange.gameObject.SetActive(true);
        if (useImageBackground)
        {
            backgroundBox.gameObject.SetActive(true);
        }

        //change text accordingly
        textMeshProToChange.text = textToShow;
        //change size of background box (if there is one) to fix the size of the new text
        if (useImageBackground)
        {
            backgroundBox.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(textMeshProToChange.preferredWidth + paddingForBackgroundBox, textMeshProToChange.preferredHeight + paddingForBackgroundBox);
        }

        //start transparent
        float fadeOutAmount = 0.0f;
        textMeshProToChange.color = new Color(textMeshProToChange.color.r, textMeshProToChange.color.g, textMeshProToChange.color.b, 0);
        if (useImageBackground)
        {
            backgroundBox.color = new Color(backgroundBox.color.r, backgroundBox.color.g, backgroundBox.color.b, 0);
        }

        //fade in
        while (textMeshProToChange.color.a < 1)
        {
            fadeOutAmount = textMeshProToChange.color.a + (5.0f * Time.deltaTime);
            textMeshProToChange.color = new Color(textMeshProToChange.color.r, textMeshProToChange.color.g, textMeshProToChange.color.b, fadeOutAmount);

            if (useImageBackground)
            {
                //textMeshProToChange and backgroundBox will always have the same a value
                backgroundBox.color = new Color(backgroundBox.color.r, backgroundBox.color.g, backgroundBox.color.b, fadeOutAmount);
            }

            yield return new WaitForSeconds(0.025f);
        }
        
        //fade in and out take about .5f seconds total
        yield return new WaitForSeconds(displayTime - 0.5f);

        //fade out
        while (textMeshProToChange.color.a > 0)
        {
            fadeOutAmount = textMeshProToChange.color.a - (5.0f * Time.deltaTime);
            textMeshProToChange.color = new Color(textMeshProToChange.color.r, textMeshProToChange.color.g, textMeshProToChange.color.b, fadeOutAmount);

            if (useImageBackground)
            {
                //textMeshProToChange and backgroundBox will always have the same a value
                backgroundBox.color = new Color(backgroundBox.color.r, backgroundBox.color.g, backgroundBox.color.b, fadeOutAmount);
            }

            yield return new WaitForSeconds(0.025f);
        }

        textMeshProToChange.gameObject.SetActive(false);

        if (useImageBackground)
        {
            backgroundBox.gameObject.SetActive(false);
        }

        textBeingShown = false;

        yield break;

    }
}
