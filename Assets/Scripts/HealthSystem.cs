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

        int vidaAntesDeCurar = currentHealth;

        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);

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
    }
}