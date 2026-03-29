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
            damageImpulseSource.GenerateImpulse(1.0f);
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

            var controller = GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            var rb = GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script != this && (script.GetType().Name.Contains("Input") || script.GetType().Name.Contains("Movement") || script.GetType().Name.Contains("Attack")))
                {
                    script.enabled = false;
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
