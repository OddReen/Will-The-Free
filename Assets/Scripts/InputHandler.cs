using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static InputHandler instance;

    InputSystem_Actions action;

    public Vector2 moveInputValue;
    public Vector2 lookInputValue;
    public Action OnInGameMenu;
    public Action OnJump;
    public Action OnShootDown;
    public Action OnShootUp;
    public Action OnReload;
    public Action OnAim;
    public Action OnStopAim;
    public Action OnInteract;

    private void Awake()
    {
        OnSpawn();
    }
    public void OnSpawn()
    {
        if (instance == null)
        {
            instance = this;
        }

        action = new InputSystem_Actions();

        action.Enable();

        action.Player.InGameMenu.performed += InGameMenu_performed;
        action.Player.InGameMenu.canceled += InGameMenu_canceled;

        action.Player.Move.performed += Move_performed;
        action.Player.Move.canceled += Move_canceled;

        action.Player.Look.performed += Look_performed;
        action.Player.Look.canceled += Look_canceled;

        action.Player.Jump.performed += Jump_performed;
        action.Player.Jump.canceled += Jump_canceled;

        action.Player.Shoot.performed += Shoot_performed;
        action.Player.Shoot.canceled += Shoot_canceled;

        action.Player.Reload.performed += Reload_performed;
        action.Player.Reload.canceled += Reload_canceled;

        action.Player.Aim.performed += Aim_performed;
        action.Player.Aim.canceled += Aim_canceled;

        action.Player.Interact.performed += Interact_performed;
        action.Player.Interact.canceled += Interact_canceled;
    }

    private void InGameMenu_canceled(InputAction.CallbackContext context)
    {

    }
    private void InGameMenu_performed(InputAction.CallbackContext context)
    {
        OnInGameMenu?.Invoke();
    }

    private void Interact_canceled(InputAction.CallbackContext context)
    {

    }
    private void Interact_performed(InputAction.CallbackContext context)
    {
        OnInteract?.Invoke();
    }

    private void Look_performed(InputAction.CallbackContext context)
    {
        lookInputValue = context.ReadValue<Vector2>();
    }
    private void Look_canceled(InputAction.CallbackContext context)
    {
        lookInputValue = Vector2.zero;
    }
    private void Move_performed(InputAction.CallbackContext context)
    {
        moveInputValue = context.ReadValue<Vector2>();
    }
    private void Move_canceled(InputAction.CallbackContext context)
    {
        moveInputValue = context.ReadValue<Vector2>();
    }
    private void Jump_canceled(InputAction.CallbackContext context)
    {
    }
    private void Jump_performed(InputAction.CallbackContext context)
    {
        OnJump?.Invoke();
    }
    private void Shoot_canceled(InputAction.CallbackContext context)
    {
        OnShootUp?.Invoke();
    }
    private void Shoot_performed(InputAction.CallbackContext context)
    {
        OnShootDown?.Invoke();
    }
    private void Reload_canceled(InputAction.CallbackContext context)
    {

    }
    private void Reload_performed(InputAction.CallbackContext context)
    {
        OnReload?.Invoke();
    }
    private void Aim_performed(InputAction.CallbackContext context)
    {
        OnAim?.Invoke();
    }
    private void Aim_canceled(InputAction.CallbackContext context)
    {
        OnStopAim?.Invoke();
    }

    public void OnDestroy()
    {
        action.Disable();

        action.Player.Move.performed -= Move_performed;
        action.Player.Move.canceled -= Move_canceled;

        action.Player.Look.performed -= Look_performed;
        action.Player.Look.canceled -= Look_canceled;

        action.Player.Jump.performed -= Jump_performed;
        action.Player.Jump.canceled -= Jump_canceled;

        action.Player.Shoot.performed -= Shoot_performed;
        action.Player.Shoot.canceled -= Shoot_canceled;

        action.Player.Aim.performed -= Aim_performed;
        action.Player.Aim.canceled -= Aim_canceled;
    }
}
