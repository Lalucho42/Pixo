using UnityEngine;

public class PlayerAnimationEventReceiver : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public void LockMovement()
    {
        if (player != null) player.IsMovementLocked = true;
    }

    public void UnlockMovement()
    {
        if (player != null) player.IsMovementLocked = false;
    }

    public void StartRollImpulse()
    {
        if (player != null && player.ColliderHandler != null)
        {
            player.ColliderHandler.SetRollState(true);
        }
    }

    public void StopRollImpulse()
    {
        if (player != null && player.ColliderHandler != null)
        {
            player.ColliderHandler.SetRollState(false);
        }
    }

    public void ExecuteJumpImpulse()
    {
        if (player != null && player.Jump != null)
        {
            player.Jump.ApplyJumpForce();
        }
    }

    public void StartHit()
    {
        if (player != null && player.WeaponManager != null && player.WeaponManager.CurrentTool != null)
        {
            player.WeaponManager.CurrentTool.EnableDamage();
        }
    }

    public void EndHit()
    {
        if (player != null && player.WeaponManager != null && player.WeaponManager.CurrentTool != null)
        {
            player.WeaponManager.CurrentTool.DisableDamage();
        }
    }
}