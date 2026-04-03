using UnityEngine;

public class PlayerAnimationEventReceiver : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        // Buscamos al script Player en el objeto padre (ya que este script va en la malla 3D)
        player = GetComponentInParent<Player>();
    }

    // --- BLOQUEO DE MOVIMIENTO ---
    public void LockMovement()
    {
        if (player != null) player.IsMovementLocked = true;
    }

    public void UnlockMovement()
    {
        if (player != null) player.IsMovementLocked = false;
    }

    // --- RODAR ---
    // Ahora le avisan al módulo encargado de las matemáticas del collider
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

    // --- SALTO ---
    public void ExecuteJumpImpulse()
    {
        if (player != null && player.Jump != null)
        {
            player.Jump.ApplyJumpForce();
        }
    }

    // --- DAÑO FÍSICO (HITBOX) ---
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