using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashInputToggle : MonoBehaviour
{
    public Image movement;
    public Image aim;

    public Color activeColor;
    public Color inactiveColor;

    void Start()
    {
        SetMovement();
    }

    public void SetMovement()
    {
        movement.color = activeColor;
        aim.color = inactiveColor;

        PlayerController.Instance.dashController.useMovementInput = true;
    }

    public void SetAim()
    {
        movement.color = inactiveColor;
        aim.color = activeColor;

        PlayerController.Instance.dashController.useMovementInput = false;
    }
}
