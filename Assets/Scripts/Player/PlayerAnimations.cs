using UnityEngine;

public class PlayerAnimations
{
    private Player player;
    private int speedHash, rollIdleHash, rollMoveHash, jumpHash, groundedHash, vVelHash;
    private int attPaloHash, attPicoHash, attHachaHash;

    // Control de tiempo para evitar spam
    private float rollCooldown = 0.6f;
    private float lastRollTime = -1f;

    public PlayerAnimations(Player playerBrain)
    {
        player = playerBrain;
        speedHash = Animator.StringToHash("Speed");
        rollIdleHash = Animator.StringToHash("RollIdle");
        rollMoveHash = Animator.StringToHash("RollMove");
        jumpHash = Animator.StringToHash("Jump");
        groundedHash = Animator.StringToHash("IsGrounded");
        vVelHash = Animator.StringToHash("VerticalVelocity");

        attPaloHash = Animator.StringToHash("AttackPalo");
        attPicoHash = Animator.StringToHash("AttackPico");
        attHachaHash = Animator.StringToHash("AttackHacha");

        // Suscripción corregida para evitar el doble llamado
        player.InputHandler.OnRollEvent += HandleRollSelection;

        // La animación de salto ahora espera la orden del módulo de salto (que tiene su propio cooldown)
        player.Jump.OnJumpInitiated += () => player.Animator.SetTrigger(jumpHash);

        player.Combat.OnAttackRequested += HandleAttackAnims;
    }

    private void HandleRollSelection()
    {
        // --- BLOQUEO ANTI-SPAM ---
        // 1. Verificamos si el módulo físico ya dice que estamos rodando
        // 2. Verificamos que haya pasado un tiempo mínimo de seguridad
        if (player.ColliderHandler.IsRolling || Time.time < lastRollTime + rollCooldown)
        {
            return;
        }

        lastRollTime = Time.time;
        float currentSpeed = player.Animator.GetFloat(speedHash);

        if (currentSpeed < 0.2f)
        {
            player.Animator.SetTrigger(rollIdleHash);
        }
        else
        {
            player.Animator.SetTrigger(rollMoveHash);
        }
    }

    public void Tick(float dt)
    {
        float targetSpeed = 0f;
        if (player.InputHandler.MoveInput.magnitude > 0.1f)
        {
            targetSpeed = player.InputHandler.IsRunning ? 1f : 0.5f;
        }

        float curSpeed = player.Animator.GetFloat(speedHash);
        player.Animator.SetFloat(speedHash, Mathf.Lerp(curSpeed, targetSpeed, dt * 10f));

        player.Animator.SetBool(groundedHash, player.Controller.isGrounded);
        player.Animator.SetFloat(vVelHash, player.Jump.VerticalVelocity);
    }

    private void HandleAttackAnims()
    {
        if (player.WeaponManager == null || player.WeaponManager.CurrentTool == null) return;

        string tool = player.WeaponManager.CurrentTool.toolName;
        if (tool == "Palo") player.Animator.SetTrigger(attPaloHash);
        else if (tool == "Pico") player.Animator.SetTrigger(attPicoHash);
        else if (tool == "Hacha") player.Animator.SetTrigger(attHachaHash);
    }
}