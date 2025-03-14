using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : NetworkBehaviour
{
    InputSystem_Actions action;

    public Vector2 moveInputValue;
    public Vector2 lookInputValue;
    public Action OnJump;
    public Action OnShoot;
    public Action OnAim;
    public Action OnStopAim;
    public Action OnInteract;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        action = new InputSystem_Actions();

        action.Enable();

        action.Player.Move.performed += Move_performed;
        action.Player.Move.canceled += Move_canceled;

        action.Player.Look.performed += Look_performed;
        action.Player.Look.canceled += Look_canceled;

        action.Player.Jump.performed += Jump_performed;
        action.Player.Jump.canceled += Jump_canceled;

        action.Player.Shoot.performed += Shoot_performed;
        action.Player.Shoot.canceled += Shoot_canceled;

        action.Player.Aim.performed += Aim_performed;
        action.Player.Aim.canceled += Aim_canceled;

        action.Player.Interact.performed += Interact_performed;
        action.Player.Interact.canceled += Interact_canceled;
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

    }
    private void Shoot_performed(InputAction.CallbackContext context)
    {
        OnShoot?.Invoke();
    }
    private void Aim_performed(InputAction.CallbackContext context)
    {
        OnAim?.Invoke();
    }
    private void Aim_canceled(InputAction.CallbackContext context)
    {
        OnStopAim?.Invoke();
    }

    public override void OnDestroy()
    {
        if (!IsOwner) return;
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
