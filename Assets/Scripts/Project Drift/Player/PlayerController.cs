using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public enum PlayerInputType
{
    Dash,
    Heal,
    MeleePressed,
    MeleeReleased,
    StartAim,
    StopAim,
    Shoot,
    SwapRanged,
}

/// <summary>
/// The script that contains all the player states, and handles player input processing
/// </summary>
[System.Serializable]
public abstract class PlayerSubController
{
    [HideInInspector]
    public PlayerController playerController;

    /// <summary>
    /// Called when this subcontroller is getting initialized when the player starts. You can assume the player controller is initialized.
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// Called when this subcontroller becomes the new subcontroller of the character
    /// </summary>
    public abstract void OnEnable();

    /// <summary>
    /// Called when this subcontroller stops being the new subcontroller of the character
    /// </summary>
    public abstract void OnDisable();

    /// <summary>
    /// Called when the subcontroller is the current subcontroller of the character
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Called when the subcontroller receives input from the player controller. Only called when it is the current subcontroller.
    /// </summary>
    /// <param name="type"></param>
    public abstract void RecieveInput(PlayerInputType type);
}

public class PlayerController : MonoBehaviour
{
    public PlayerGroundController groundController;
    public PlayerDashController dashController;
    public PlayerAttackController attackController;
    public PlayerAimController aimController;
    public PlayerShootController shootController;

    public Vector2 movementInput { get; private set; }
    public Vector2 aimInput { get; private set; }

    public PlayerSubController currentController { get; private set; }
    public string debugCurrCtrlr;

    public static PlayerController Instance { get; private set; }

    public PlayerAbilitySO currentAbility;

    private void Awake()
    {
        Instance = this;
        
        groundController.playerController = this;
        groundController.Initialize();
        dashController.playerController = this;
        dashController.Initialize();
        attackController.playerController = this;
        attackController.Initialize();
        aimController.playerController = this;
        aimController.Initialize();
        shootController.playerController = this;
        shootController.Initialize();

        currentAbility.Initialize(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        SetController(groundController);
    }

    void Update()
    {
        debugCurrCtrlr = currentController.ToString();
        CalculateAimInput();
        currentController.Update();
    }

    public void SetController(PlayerSubController newController)
    {
        if (currentController != null)
            currentController.OnDisable();

        currentController = newController;
        currentController.OnEnable();
    }

    public void MovePlayer(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }

    public void Dash(InputAction.CallbackContext ctx)
    {
        currentController.RecieveInput(PlayerInputType.Dash);
    }

    public void Melee(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
            currentController.RecieveInput(PlayerInputType.MeleeReleased);
        else
            currentController.RecieveInput(PlayerInputType.MeleePressed);
    }

    public void Aim(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
            currentController.RecieveInput(PlayerInputType.StopAim);
        else
            currentController.RecieveInput(PlayerInputType.StartAim);
    }

    public void Shoot(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            currentController.RecieveInput(PlayerInputType.Shoot);
    }

    public void SwapWeapon(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            currentController.RecieveInput(PlayerInputType.SwapRanged);
    }

    public void Special(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (currentAbility.CanBeActivated())
                currentAbility.Activate();
        }
    }

    void CalculateAimInput()
    {
        
        if (Gamepad.current != null)
        {
            aimInput = Gamepad.current.rightStick.ReadValue();
        }
        else
        {
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            aimInput = (mouseWorldPosition - (Vector2)transform.position).normalized;
        }
        
    }
}
