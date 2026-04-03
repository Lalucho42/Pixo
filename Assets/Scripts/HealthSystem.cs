using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [Header("Configuracion de Vida")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Eventos")]
    public UnityEvent onDeath;
    public UnityEvent onTakeDamage;

    [Header("Audio")]
    public AudioClip deathSound;

    // Cambiamos a 'public get' para que el GameManager pueda leerlo
    public bool IsDead { get; private set; } = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if (IsDead) return;

        currentHealth -= damageAmount;
        if (onTakeDamage != null) onTakeDamage.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        if (IsDead) return;
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
    }

    // --- ESTA ES LA FUNCIÓN QUE NECESITA EL GAME MANAGER ---
    public void ResetDeath()
    {
        IsDead = false;
        currentHealth = maxHealth;
        Debug.Log("Vida reseteada: Jugador revivido.");
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        if (deathSound != null && AudioManager.instance != null)
            AudioManager.instance.PlaySFX(deathSound);

        if (onDeath != null) onDeath.Invoke();

        if (CompareTag("Player"))
        {
            // Llama al menú de muerte del GameManager
            if (GameManager.instance != null) GameManager.instance.ShowDeathMenu();
        }
        else
        {
            Destroy(gameObject, 0.1f);
        }
    }
}