using UnityEngine;

public class PlayerAnimationEventReceiver : MonoBehaviour
{
    private Player player;

    private void Awake() { player = GetComponentInParent<Player>(); }

    // Bloqueo de Movimiento
    public void LockMovement() { if (player != null) player.IsMovementLocked = true; }
    public void UnlockMovement() { if (player != null) player.IsMovementLocked = false; }

    // Rodar
    public void StartRollImpulse() { if (player != null) player.SetRollImpulse(true); }
    public void StopRollImpulse() { if (player != null) player.SetRollImpulse(false); }

    // Salto
    public void ExecuteJumpImpulse() { if (player != null && player.Jump != null) player.Jump.ApplyJumpForce(); }

    // Daño Físico (Colliders)
    public void StartHit() { if (player != null && player.WeaponManager.CurrentTool != null) player.WeaponManager.CurrentTool.EnableDamage(); }
    public void EndHit() { if (player != null && player.WeaponManager.CurrentTool != null) player.WeaponManager.CurrentTool.DisableDamage(); }
}