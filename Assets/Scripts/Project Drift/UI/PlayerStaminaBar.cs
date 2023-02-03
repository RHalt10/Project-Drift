using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStaminaBar : MonoBehaviour
{
    public PlayerStamina playerStamina;
    
    RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        playerStamina.OnStaminaAmountChanged.AddListener(UpdateStamina);

        UpdateStamina();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            playerStamina.UseStamina(0.2f);
        }
    }

    // callback function to update stamina UI
    void UpdateStamina()
    {
        float proportion = playerStamina.currentStamina / 1.0f;
        rectTransform.localScale = new Vector3(proportion, 1.0f, 1.0f);
    }
}
