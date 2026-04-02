using UnityEngine;

public class PlayerAnimations
{
    private Player player;
    private int speedHash, rollHash, jumpHash, groundedHash, vVelHash;
    private int attPaloHash, attPicoHash, attHachaHash;

    public PlayerAnimations(Player playerBrain)
    {
        player = playerBrain;
        speedHash = Animator.StringToHash("Speed");
        rollHash = Animator.StringToHash("Roll");
        jumpHash = Animator.StringToHash("Jump");
        groundedHash = Animator.StringToHash("IsGrounded");
        vVelHash = Animator.StringToHash("VerticalVelocity");

        attPaloHash = Animator.StringToHash("AttackPalo");
        attPicoHash = Animator.StringToHash("AttackPico");
        attHachaHash = Animator.StringToHash("AttackHacha");

        player.InputHandler.OnRollEvent += () => player.Animator.SetTrigger(rollHash);
        player.InputHandler.OnJumpEvent += () => { if (player.Controller.isGrounded) player.Animator.SetTrigger(jumpHash); };
        player.Combat.OnAttackRequested += HandleAttackAnims;
    }

    public void Tick(float dt)
    {
        float targetSpeed = player.InputHandler.MoveInput.magnitude > 0.1f ? (player.InputHandler.IsRunning ? 2f : 1f) : 0f;
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