using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler
{
    private PlayerControls playerControls;

    public Vector2 MoveInput { get; private set; }
    public bool IsRunning { get; private set; }
    public event Action OnRollEvent;
    public event Action OnInteractEvent;
    public event Action OnFlashlightEvent;

    public Vector2 LookInput
    {
        get
        {
            return playerControls.Gameplay.Look.ReadValue<Vector2>();
        }
    }

    public PlayerInputHandler()
    {
        playerControls = new PlayerControls();

        playerControls.Gameplay.Move.performed += OnMovePerformed;
        playerControls.Gameplay.Move.canceled += OnMoveCanceled;

        playerControls.Gameplay.Run.performed += OnRunPerformed;
        playerControls.Gameplay.Run.canceled += OnRunCanceled;

        playerControls.Gameplay.Roll.performed += OnRollPerformed;
        playerControls.Gameplay.Interact.performed += OnInteractPerformed;
        playerControls.Gameplay.Flashlight.performed += OnFlashlightPerformed;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector2.zero;
    }

    private void OnRunPerformed(InputAction.CallbackContext context)
    {
        IsRunning = true;
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        IsRunning = false;
    }

    private void OnRollPerformed(InputAction.CallbackContext context)
    {
        if (OnRollEvent != null)
        {
            OnRollEvent.Invoke();
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (OnInteractEvent != null) OnInteractEvent.Invoke();
    }

    private void OnFlashlightPerformed(InputAction.CallbackContext context)
    {
        if (OnFlashlightEvent != null) OnFlashlightEvent.Invoke();
    }

    public void Enable()
    {
        playerControls.Enable();
    }

    public void Disable()
    {
        playerControls.Disable();
    }
}
