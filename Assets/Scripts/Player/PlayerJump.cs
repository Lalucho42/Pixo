using UnityEngine;
using System;

public class PlayerJump
{
    private Player player;
    public float VerticalVelocity { get; private set; }
    public event Action OnJumpInitiated;

    private float jumpCooldown = 0.5f;
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
            OnJumpInitiated?.Invoke();
        }
    }

    public void ApplyJumpForce()
    {
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
            VerticalVelocity -= player.gravity * deltaTime;
        }

        player.Controller.Move(Vector3.up * VerticalVelocity * deltaTime);
    }
}
