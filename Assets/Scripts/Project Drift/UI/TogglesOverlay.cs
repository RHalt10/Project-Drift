using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglesOverlay : MonoBehaviour
{
    // toggles target menu and disables current menu
    public void ToggleOverlay(GameObject targetMenu)
    {
        targetMenu.SetActive(true);
        transform.parent.gameObject.SetActive(false);
    }
}
