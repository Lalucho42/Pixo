using UnityEngine;

public class PlayerAnimationEventReceiver : MonoBehaviour
{
    private Player player;

    [Header("Ajuste de Pasos")]
    public float cooldownPasos = 0.22f;
    private float ultimoTiempoPaso = 0f;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public void PlayFootstep()
    {
        if (player == null || AudioManager.Instance == null || player.AudioSource == null) return;

        if (player.ColliderHandler != null && player.ColliderHandler.IsRolling) return;
        if (player.InputHandler.MoveInput.magnitude < 0.1f) return;
        if (Time.time < ultimoTiempoPaso + cooldownPasos) return;

        bool estaEnSueloReal = player.Controller.isGrounded || VerificarSueloCorto();
        if (!estaEnSueloReal) return;

        ultimoTiempoPaso = Time.time;

        // UML PERFECTO: Solo le mandamos el texto de la base de datos y nuestro parlante corporal
        AudioManager.Instance.PlaySFX3D("Player_Paso", player.AudioSource);
    }

    private bool VerificarSueloCorto()
    {
        Vector3 origin = player.transform.position + Vector3.up * 0.2f;
        return Physics.Raycast(origin, Vector3.down, 0.5f);
    }

    public void StartRollImpulse()
    {
        if (player != null && player.ColliderHandler != null)
        {
            player.ColliderHandler.SetRollState(true);

            // UML PERFECTO: Bypass 2D instantáneo directo al Manager
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX2D("Player_Rodar");
            }
        }
    }

    public void ExecuteJumpImpulse()
    {
        if (player != null && player.Jump != null)
        {
            player.Jump.ApplyJumpForce();

            // UML PERFECTO: Bypass 2D instantáneo directo al Manager
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX2D("Player_Salto");
            }
        }
    }

    public void LockMovement() { if (player != null) player.IsMovementLocked = true; }
    public void UnlockMovement() { if (player != null) player.IsMovementLocked = false; }
    public void StopRollImpulse() { if (player != null && player.ColliderHandler != null) player.ColliderHandler.SetRollState(false); }
    public void StartHit() { if (player != null && player.WeaponManager != null && player.WeaponManager.CurrentTool != null) player.WeaponManager.CurrentTool.EnableDamage(); }
    public void EndHit() { if (player != null && player.WeaponManager != null && player.WeaponManager.CurrentTool != null) player.WeaponManager.CurrentTool.DisableDamage(); }
}