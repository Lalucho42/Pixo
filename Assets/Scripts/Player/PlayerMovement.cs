using UnityEngine;

public class PlayerMovement
{
    private Player player;
    private float walk, run, roll;
    private float currentVelocity;

    public PlayerMovement(Player brain, float w, float r, float ro)
    {
        player = brain;
        walk = w;
        run = r;
        roll = ro;
    }

    public void Tick(float dt)
    {
        if (player.IsMovementLocked) return;

        Vector2 input = player.InputHandler.MoveInput;
        Vector3 forward = player.PlayerCamera.CameraForward;
        Vector3 right = player.PlayerCamera.CameraRight;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (forward * input.y + right * input.x).normalized;

        if (player.ColliderHandler.IsRolling)
        {
            if (moveDir.magnitude < 0.1f) moveDir = player.transform.forward;
        }

        float targetSpeed = player.ColliderHandler.IsRolling ? roll : (player.InputHandler.IsRunning ? run : walk);

        if (input.magnitude < 0.1f && !player.ColliderHandler.IsRolling)
        {
            targetSpeed = 0f;
        }

        // --- LA SOLUCIÓN AL PATINAJE ---
        // Ajustamos la fuerza del Lerp según lo que el jugador esté haciendo
        float smoothFactor = 15f; // Aceleración normal y responsiva

        if (player.ColliderHandler.IsRolling)
        {
            smoothFactor = 8f; // Suave y con inercia para el roll
        }
        else if (input.magnitude < 0.1f)
        {
            smoothFactor = 30f; // Freno de disco MUY fuerte al soltar la tecla
        }

        currentVelocity = Mathf.Lerp(currentVelocity, targetSpeed, dt * smoothFactor);

        // Cortamos el deslizamiento residual de raíz si la velocidad ya es muy baja (0.5 o menos)
        // Esto garantiza que el personaje se clave en el piso junto con la animación
        if (input.magnitude < 0.1f && !player.ColliderHandler.IsRolling && currentVelocity < 0.5f)
        {
            currentVelocity = 0f;
        }

        if (moveDir.magnitude > 0.1f || currentVelocity > 0.1f)
        {
            Vector3 finalDir = moveDir.magnitude > 0.1f ? moveDir : player.transform.forward;

            if (input.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(finalDir);
                player.transform.rotation = Quaternion.Slerp(
                    player.transform.rotation,
                    targetRotation,
                    dt * player.rotationSpeed
                );
            }

            player.Controller.Move(finalDir * currentVelocity * dt);
        }
    }
}