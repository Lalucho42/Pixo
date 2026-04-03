using UnityEngine;

public class PlayerAnimations
{
    private Player player;
    private int speedHash, rollIdleHash, rollMoveHash, jumpHash, groundedHash, vVelHash;
    private int attPaloHash, attPicoHash, attHachaHash;

    public PlayerAnimations(Player playerBrain)
    {
        player = playerBrain;
        speedHash = Animator.StringToHash("Speed");

        // --- NUEVOS HASHES PARA LOS DOS ROLLS ---
        rollIdleHash = Animator.StringToHash("RollIdle");
        rollMoveHash = Animator.StringToHash("RollMove");

        jumpHash = Animator.StringToHash("Jump");
        groundedHash = Animator.StringToHash("IsGrounded");
        vVelHash = Animator.StringToHash("VerticalVelocity");

        attPaloHash = Animator.StringToHash("AttackPalo");
        attPicoHash = Animator.StringToHash("AttackPico");
        attHachaHash = Animator.StringToHash("AttackHacha");

        // Suscripción al evento de rodar
        player.InputHandler.OnRollEvent += HandleRollSelection;

        player.InputHandler.OnJumpEvent += () => { if (player.Controller.isGrounded) player.Animator.SetTrigger(jumpHash); };
        player.Combat.OnAttackRequested += HandleAttackAnims;
    }

    private void HandleRollSelection()
    {
        // Si la velocidad actual en el Animator es muy baja, asumimos que está en Idle
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
        // 0.0 = Idle | 0.5 = Caminar | 1.0 = Correr
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
        string tool = player.WeaponManager.CurrentTool.toolName;
        if (tool == "Palo") player.Animator.SetTrigger(attPaloHash);
        else if (tool == "Pico") player.Animator.SetTrigger(attPicoHash);
        else if (tool == "Hacha") player.Animator.SetTrigger(attHachaHash);
    }
}