using UnityEngine;

public class PlayerAnimations
{
    private Player player;
    private int speedHash;
    private int rollHash;

    private float rollTimer = 0f;

    public PlayerAnimations(Player playerBrain)
    {
        player = playerBrain;

        speedHash = Animator.StringToHash("Speed");
        rollHash = Animator.StringToHash("Roll");

        player.InputHandler.OnRollEvent += HandleRoll;
    }

    public void Tick(float deltaTime)
    {
        if (rollTimer > 0f)
        {
            rollTimer = rollTimer - deltaTime;
        }

        // --- Usamos los nuevos umbrales enteros (0, 1, 2) ---
        float targetSpeed = 0f;

        if (player.InputHandler.MoveInput.magnitude > 0.1f)
        {
            if (player.InputHandler.IsRunning == true)
            {
                targetSpeed = 2f; // Correr
            }
            else
            {
                targetSpeed = 1f; // Caminar
            }
        }

        float currentSpeed = player.Animator.GetFloat(speedHash);
        float smoothedSpeed = Mathf.Lerp(currentSpeed, targetSpeed, deltaTime * 10f);

        player.Animator.SetFloat(speedHash, smoothedSpeed);
    }

    private void HandleRoll()
    {
        // Leemos el cooldown desde el script Player
        if (rollTimer <= 0f)
        {
            player.Animator.SetTrigger(rollHash);
            rollTimer = player.rollCooldown; // Seteamos el tiempo leído del Inspector
        }
        else
        {
            Debug.Log("Habilidad en recarga: " + rollTimer);
        }
    }
}