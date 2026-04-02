using UnityEngine;
using System;

public class PlayerJump
{
    private Player player;
    public float VerticalVelocity { get; private set; }
    public event Action OnJumpInitiated;

    public PlayerJump(Player playerBrain)
    {
        player = playerBrain;
        player.InputHandler.OnJumpEvent += HandleJumpInput;
    }

    private void HandleJumpInput()
    {
        if (player.Controller.isGrounded)
        {
            if (OnJumpInitiated != null) OnJumpInitiated.Invoke();
        }
    }

    public void ApplyJumpForce()
    {
        // Aplicamos la fuerza física real
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