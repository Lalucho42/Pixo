using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 25;

    private void OnTriggerEnter(Collider other)
    {
        HealthSystem health = other.GetComponentInParent<HealthSystem>();
        
        if (health != null && health.CompareTag("Player"))
        {
            if (health.currentHealth < health.maxHealth)
            {
                health.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }
}
