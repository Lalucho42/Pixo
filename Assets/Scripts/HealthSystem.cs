using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent onTakeDamage;

    public bool IsDead { get; private set; } = false;

    private void Awake()
    {
        // CONDICIÓN ESPECIAL: Si este script pertenece al jugador, arranca herido con 50 de vida.
        // Si pertenece a un enemigo o dummy, arranca con su vida máxima normal.
        if (CompareTag("Player"))
        {
            currentHealth = 50;
        }
        else
        {
            currentHealth = maxHealth;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (IsDead) return;

        currentHealth -= damageAmount;
        Debug.Log("HealthSystem (" + gameObject.name + ") took damage: " + damageAmount + ". Current health: " + currentHealth);
        onTakeDamage?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        if (IsDead) return;

        // Guardamos el valor exacto de la vida antes de aplicar la curación
        int vidaAntesDeCurar = currentHealth;

        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);

        // REGLA DE AUDIO: Si quien se cura es el Jugador, y la curación fue efectiva (recuperó al menos 1 punto),
        // disparamos el efecto de sonido global a través de nuestro AudioManager unificado.
        if (CompareTag("Player") && currentHealth > vidaAntesDeCurar)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX2D("Player_Curarse");
            }
        }
    }

    public void ResetDeath()
    {
        IsDead = false;
        currentHealth = maxHealth; 
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        onDeath?.Invoke();

        if (CompareTag("Player"))
        {
            if (GameManager.instance != null) GameManager.instance.ShowDeathMenu();
        }
        else
        {
            Destroy(gameObject, 0.1f);
        }
    }
}