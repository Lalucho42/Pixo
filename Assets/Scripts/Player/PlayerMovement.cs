using UnityEngine;

public class PlayerMovement
{
    private Player player;
    private float walk, run, roll;

    public PlayerMovement(Player brain, float w, float r, float ro)
    {
        player = brain; walk = w; run = r; roll = ro;
    }

    public void Tick(float dt)
    {
        if (player.IsMovementLocked) return; // Si estamos atacando, no nos movemos

        Vector2 input = player.InputHandler.MoveInput;
        Vector3 forward = player.PlayerCamera.CameraForward;
        Vector3 right = player.PlayerCamera.CameraRight;
        forward.y = 0; right.y = 0; forward.Normalize(); right.Normalize();

        Vector3 dir = (forward * input.y + right * input.x).normalized;
        float speed = player.ApplyRollImpulse ? roll : (player.InputHandler.IsRunning ? run : walk);

        if (dir.magnitude > 0.1f)
        {
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(dir), dt * player.rotationSpeed);
            player.Controller.Move(dir * speed * dt);
        }
    }
}