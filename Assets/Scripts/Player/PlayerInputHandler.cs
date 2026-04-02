using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler
{
    private PlayerControls playerControls;

    public Vector2 MoveInput { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsJumpHeld { get; private set; }

    public event Action OnRollEvent;
    public event Action OnInteractEvent;
    public event Action OnFlashlightEvent;
    public event Action OnJumpEvent;
    public event Action OnAttackEvent;

    // Evento para la rueda del mouse (pasa un número para saber si sube o baja)
    public event Action<float> OnScrollEvent;

    // Getter tradicional
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

        // Conectamos los botones a funciones tradicionales
        playerControls.Gameplay.Move.performed += OnMovePerformed;
        playerControls.Gameplay.Move.canceled += OnMoveCanceled;

        playerControls.Gameplay.Run.performed += OnRunPerformed;
        playerControls.Gameplay.Run.canceled += OnRunCanceled;

        playerControls.Gameplay.Jump.performed += OnJumpPerformedState;
        playerControls.Gameplay.Jump.canceled += OnJumpCanceledState;

        playerControls.Gameplay.Roll.performed += OnRollPerformed;
        playerControls.Gameplay.Interact.performed += OnInteractPerformed;
        playerControls.Gameplay.Flashlight.performed += OnFlashlightPerformed;
        playerControls.Gameplay.Jump.performed += OnJumpEventTrigger;
        playerControls.Gameplay.Attack.performed += OnAttackPerformed;

        // Conectamos la rueda del mouse
        playerControls.Gameplay.Scroll.performed += OnScrollPerformed;
    }

    // --- Funciones tradicionales para cada acción ---

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

    private void OnJumpPerformedState(InputAction.CallbackContext context)
    {
        IsJumpHeld = true;
    }

    private void OnJumpCanceledState(InputAction.CallbackContext context)
    {
        IsJumpHeld = false;
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
        if (OnInteractEvent != null)
        {
            OnInteractEvent.Invoke();
        }
    }

    private void OnFlashlightPerformed(InputAction.CallbackContext context)
    {
        if (OnFlashlightEvent != null)
        {
            OnFlashlightEvent.Invoke();
        }
    }

    private void OnJumpEventTrigger(InputAction.CallbackContext context)
    {
        if (OnJumpEvent != null)
        {
            OnJumpEvent.Invoke();
        }
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (OnAttackEvent != null)
        {
            OnAttackEvent.Invoke();
        }
    }

    private void OnScrollPerformed(InputAction.CallbackContext context)
    {
        // Leemos solo el valor Y de la rueda (hacia adelante o hacia atrás)
        float scrollY = context.ReadValue<Vector2>().y;

        if (scrollY != 0f && OnScrollEvent != null)
        {
            OnScrollEvent.Invoke(scrollY);
        }
    }

    // --- Prender y apagar ---

    public void Enable()
    {
        playerControls.Enable();
    }

    public void Disable()
    {
        playerControls.Disable();
    }
}