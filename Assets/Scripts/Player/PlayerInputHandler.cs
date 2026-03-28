using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler
{
    private PlayerControls playerControls;

    public Vector2 MoveInput { get; private set; }
    public bool IsRunning { get; private set; }
    public event Action OnRollEvent;

    // --- EL CAMBIO MÁGICO ---
    // En lugar de usar eventos, leemos el mouse directamente en tiempo real
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

        // ¡BORRAMOS las líneas de Look.performed y Look.canceled!

        playerControls.Gameplay.Run.performed += OnRunPerformed;
        playerControls.Gameplay.Run.canceled += OnRunCanceled;

        playerControls.Gameplay.Roll.performed += OnRollPerformed;
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

    public void Enable()
    {
        playerControls.Enable();
    }

    public void Disable()
    {
        playerControls.Disable();
    }
}