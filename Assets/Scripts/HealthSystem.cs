using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public CinemachineImpulseSource damageImpulseSource;
    public UnityEvent onDeath;
    public AudioClip deathSound;
    private bool isDead = false;

    private void OnValidate()
    {
        if (damageImpulseSource == null)
            damageImpulseSource = transform.Find("DamageImpulseSource")?.GetComponent<CinemachineImpulseSource>();
    }

    private void Awake()
    {
        if (damageImpulseSource == null)
            damageImpulseSource = transform.Find("DamageImpulseSource")?.GetComponent<CinemachineImpulseSource>();
        
        currentHealth = maxHealth;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;
        currentHealth -= damageAmount;

        if (damageImpulseSource != null)
        {
            float intensity = Mathf.Clamp(damageAmount * 0.025f, 0.1f, 0.45f);
            Vector3 dir = new Vector3(Random.Range(-1f, 1f), -0.2f, Random.Range(-1f, 1f)).normalized;
            damageImpulseSource.GenerateImpulseWithVelocity(dir * intensity);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (deathSound != null && AudioManager.instance != null) AudioManager.instance.PlaySFX(deathSound);

        onDeath.Invoke();

        if (CompareTag("Player"))
        {
            if (GameManager.instance != null)
                GameManager.instance.ShowDeathMenu();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetDeath()
    {
        isDead = false;
        currentHealth = maxHealth;
    }
}
