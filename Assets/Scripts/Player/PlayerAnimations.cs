using UnityEngine;

public class PlayerAnimations
{
    private Player player;
    private int speedHash, rollIdleHash, rollMoveHash, jumpHash, groundedHash, vVelHash;
    private int attPaloHash, attPicoHash, attHachaHash;

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

        player.InputHandler.OnRollEvent += HandleRollSelection;

        player.Jump.OnJumpInitiated += () => player.Animator.SetTrigger(jumpHash);

        player.Combat.OnAttackRequested += HandleAttackAnims;
    }

    private void HandleRollSelection()
    {
        if (player.IsMovementLocked) return;

        if (player.ColliderHandler.IsRolling || Time.time < lastRollTime + rollCooldown) return;

        lastRollTime = Time.time;
        float currentSpeed = player.Animator.GetFloat(speedHash);

        if (currentSpeed < 0.2f) player.Animator.SetTrigger(rollIdleHash);
        else player.Animator.SetTrigger(rollMoveHash);
    }

    public void Tick(float dt)
    {
        float targetSpeed = 0f;

        if (!player.IsMovementLocked && player.InputHandler.MoveInput.magnitude > 0.1f)
        {
            targetSpeed = player.InputHandler.IsRunning ? 1f : 0.5f;
        }

        float curSpeed = player.Animator.GetFloat(speedHash);
        player.Animator.SetFloat(speedHash, Mathf.Lerp(curSpeed, targetSpeed, dt * 10f));

        player.Animator.SetBool(groundedHash, player.Controller.isGrounded);
        player.Animator.SetFloat(vVelHash, player.Jump.VerticalVelocity);

        bool canCombo = player.WeaponManager != null &&
                        player.WeaponManager.CurrentTool != null &&
                        player.WeaponManager.CurrentTool.toolName == "Palo";

        player.Animator.SetBool("IsCombo", canCombo && player.InputHandler.IsAttackHeld);
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