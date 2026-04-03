using UnityEngine;

public class PlayerMovement
{
    private Player player;
    private float walk, run, roll;
    private float currentVelocity; // Para suavizar la velocidad al terminar el roll

    public PlayerMovement(Player brain, float w, float r, float ro)
    {
        player = brain;
        walk = w;
        run = r;
        roll = ro;
    }

    public void Tick(float dt)
    {
        // 1. Bloqueo total: Si estamos en medio de un ataque o minando, no permitimos movimiento.
        if (player.IsMovementLocked) return;

        // 2. Obtener Input y Direcciones de Cámara
        Vector2 input = player.InputHandler.MoveInput;
        Vector3 forward = player.PlayerCamera.CameraForward;
        Vector3 right = player.PlayerCamera.CameraRight;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // 3. Calcular dirección de movimiento deseada
        Vector3 moveDir = (forward * input.y + right * input.x).normalized;

        // --- LÓGICA DE IMPULSO PARA EL ROLL ---
        // Nos comunicamos con el nuevo módulo ColliderHandler
        if (player.ColliderHandler.IsRolling)
        {
            // Si el jugador NO está moviendo el joystick/teclado, forzamos hacia adelante
            if (moveDir.magnitude < 0.1f)
            {
                moveDir = player.transform.forward;
            }
        }

        // --- SUAVIZADO DE VELOCIDAD (Game Feel) ---
        // Definimos la velocidad objetivo
        float targetSpeed = player.ColliderHandler.IsRolling ? roll : (player.InputHandler.IsRunning ? run : walk);

        // Si no hay input y no estamos rodando, la velocidad deseada es 0
        if (input.magnitude < 0.1f && !player.ColliderHandler.IsRolling)
        {
            targetSpeed = 0f;
        }

        // Usamos Lerp para que el cambio de velocidad (frenado) no sea instantáneo
        currentVelocity = Mathf.Lerp(currentVelocity, targetSpeed, dt * 8f);

        // 4. Aplicar Movimiento y Rotación
        // Nos movemos si hay dirección física o si seguimos deslizando por la inercia del Lerp
        if (moveDir.magnitude > 0.1f || currentVelocity > 0.1f)
        {
            // Usamos la dirección del personaje si ya no hay input pero seguimos frenando
            Vector3 finalDir = moveDir.magnitude > 0.1f ? moveDir : player.transform.forward;

            // Solo rotamos el cuerpo si hay un input real del jugador
            // Esto evita que el personaje gire sobre su eje durante el roll forzado o el frenado
            if (input.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(finalDir);
                player.transform.rotation = Quaternion.Slerp(
                    player.transform.rotation,
                    targetRotation,
                    dt * player.rotationSpeed
                );
            }

            // Mover al personaje a través del CharacterController
            player.Controller.Move(finalDir * currentVelocity * dt);
        }
    }
}