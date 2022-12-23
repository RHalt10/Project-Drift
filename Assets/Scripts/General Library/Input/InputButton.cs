using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

namespace WSoft.Input
{
    public class InputButton : MonoBehaviour
    {
        Button button;

        public Image buttonImage;
        public TMP_Text keyText;

        [Space]
        public ControlPrompt.ControlPrompt controlPrompt;
        public GamepadButton gamepadButton;
        public string key;

        // Start is called before the first frame update
        void Start()
        {
            button = GetComponent<Button>();
        }

        // Update is called once per frame
        void Update()
        {
            Sprite sprite = controlPrompt.Sprite;
            if (sprite == null || Gamepad.current == null)
            {
                keyText.gameObject.SetActive(true);
                buttonImage.gameObject.SetActive(false);
                keyText.text = key;

                if (!string.IsNullOrEmpty(key) && Keyboard.current.FindKeyOnCurrentKeyboardLayout(key).wasPressedThisFrame)
                {
                    button.onClick.Invoke();
                }
            }
            else
            {
                keyText.gameObject.SetActive(false);
                buttonImage.gameObject.SetActive(true);
                buttonImage.sprite = sprite;
                if (Gamepad.current[gamepadButton].wasPressedThisFrame)
                {
                    button.onClick.Invoke();
                }
            }
        }
    }

}