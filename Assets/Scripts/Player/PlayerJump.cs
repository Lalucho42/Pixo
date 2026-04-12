using UnityEngine;
using System;

public class PlayerJump
{
    private Player player;
    public float VerticalVelocity { get; private set; }
    public event Action OnJumpInitiated;

    // --- NUEVAS VARIABLES DE COOLDOWN ---
    private float jumpCooldown = 0.5f; // Medio segundo de bloqueo entre saltos
    private float lastJumpTime = -1f;

    public PlayerJump(Player playerBrain)
    {
        player = playerBrain;
        player.InputHandler.OnJumpEvent += HandleJumpInput;
    }

    private void HandleJumpInput()
    {
        
        if (player.IsMovementLocked) return;

        if (player.Controller.isGrounded && Time.time >= lastJumpTime + jumpCooldown)
        {
            lastJumpTime = Time.time;
            if (OnJumpInitiated != null) OnJumpInitiated.Invoke();
        }
    }

    public void ApplyJumpForce()
    {
        // Nota: Si en el paso anterior le cambiaste el nombre a "jumpHeight", borrale el "Idle" acá abajo.
        VerticalVelocity = Mathf.Sqrt(2f * player.jumpHeightIdle * player.gravity);
    }

    public void Tick(float deltaTime)
    {
        if (player.Controller.isGrounded && VerticalVelocity < 0f)
        {
            VerticalVelocity = -2f;
        }
        else
        {
            // Aplicamos gravedad constante
            VerticalVelocity -= player.gravity * deltaTime;
        }

        player.Controller.Move(Vector3.up * VerticalVelocity * deltaTime);
    }
}