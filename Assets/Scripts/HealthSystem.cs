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

    [Header("Audio")]
    public AudioClip deathSound;

    public bool IsDead { get; private set; } = false;

    private void Awake()
    {
        currentHealth = maxHealth;
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
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
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

        if (deathSound != null && AudioManager.instance != null)
            AudioManager.instance.PlaySFX(deathSound);

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
