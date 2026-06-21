using UnityEngine;
using System;

public class PlayerJump
{
    private Player player;
    public float VerticalVelocity { get; private set; }
    public event Action OnJumpInitiated;

    private float jumpCooldown = 0.5f;
    private float lastJumpTime = -1f;
    private float tiempoEnElAire = 0f;

    public PlayerJump(Player playerBrain)
    {
        player = playerBrain;
        player.InputHandler.OnJumpEvent += HandleJumpInput;
    }

    private void HandleJumpInput()
    {
        if (player.IsMovementLocked) return;

        if (player.Controller.isGrounded && Time.time >= lastJumpTime + jumpCooldown)
        {
            lastJumpTime = Time.time;
            OnJumpInitiated?.Invoke();
        }
    }

    public void ApplyJumpForce()
    {
        VerticalVelocity = Mathf.Sqrt(2f * player.jumpHeightIdle * player.gravity);
    }

    public void Tick(float deltaTime)
    {
        bool tocandoSueloCC = player.Controller.isGrounded;
        bool sueloCercaRaycast = VerificarSueloRealBajoLosPies();

        bool sueloEstabilizado = VerticalVelocity <= 0f && (tocandoSueloCC || sueloCercaRaycast);

        if (sueloEstabilizado)
        {
            if (tiempoEnElAire > 0.20f)
            {
                // UML PERFECTO: Emitimos el disparo de impacto plano directo al manager global
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX2D("Player_Aterrizar");
                }
            }

            VerticalVelocity = -2f;
            tiempoEnElAire = 0f;
        }
        else
        {
            VerticalVelocity -= player.gravity * deltaTime;
            if (!tocandoSueloCC && !sueloCercaRaycast)
            {
                tiempoEnElAire += deltaTime;
            }
        }

        player.Controller.Move(Vector3.up * VerticalVelocity * deltaTime);
    }

    private bool VerificarSueloRealBajoLosPies()
    {
        Vector3 origin = player.transform.position + Vector3.up * 0.2f;
        RaycastHit[] hits = Physics.RaycastAll(origin, Vector3.down, 0.6f);

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject != player.gameObject && !hit.collider.isTrigger)
            {
                return true;
            }
        }
        return false;
    }
}