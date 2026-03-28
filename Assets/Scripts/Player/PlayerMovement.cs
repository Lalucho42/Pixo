using UnityEngine;

public class PlayerMovement
{
    private Player player;

    public PlayerMovement(Player playerBrain)
    {
        player = playerBrain;
    }

    public void Tick(float deltaTime)
    {
        Vector3 finalMovement = Vector3.zero;

        // --- CAMBIO ACÁ: Leemos la orden exacta de la animación ---
        if (player.ApplyRollImpulse == true)
        {
            // Usamos la nueva velocidad de impulso del inspector
            finalMovement = player.transform.forward * player.rollImpulseSpeed;
        }
        else
        {
            // Movimiento normal por teclado/joystick (El mismo que ya teníamos)
            Vector2 input = player.InputHandler.MoveInput;

            Vector3 camaraForward = Camera.main.transform.forward;
            Vector3 camaraRight = Camera.main.transform.right;

            camaraForward.y = 0f;
            camaraRight.y = 0f;
            camaraForward.Normalize();
            camaraRight.Normalize();

            Vector3 moveDirection = (camaraForward * input.y) + (camaraRight * input.x);

            if (moveDirection.magnitude >= 0.1f)
            {
                float currentSpeed = player.walkSpeed;

                if (player.InputHandler.IsRunning == true)
                {
                    currentSpeed = player.runSpeed;
                }

                finalMovement = moveDirection * currentSpeed;

                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, player.rotationSpeed * deltaTime);
            }
        }

        player.Controller.Move(finalMovement * deltaTime);
        player.Controller.Move(Vector3.down * 9.81f * deltaTime);
    }
}