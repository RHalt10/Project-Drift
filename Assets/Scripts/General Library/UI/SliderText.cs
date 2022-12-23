/*
 * Augments the behavior of UnityEngine.UI.Slider by changing the text of a TextComponent to match the value.
 * @ Jacob Shreve '?, Nikhil Ghosh '23
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WSoft.UI
{
    public class SliderText : MonoBehaviour
    {
        [Tooltip("The text to be modified")]
        public UnityEngine.UI.Text valueText;

        void Start()
        {
            UnityEngine.UI.Slider slider = GetComponent<UnityEngine.UI.Slider>();
            slider.onValueChanged.AddListener(OnValueChanged);
            valueText.text = slider.value.ToString();
        }

        void OnValueChanged(float value)
        {
            valueText.text = value.ToString();
        }
    }
}
