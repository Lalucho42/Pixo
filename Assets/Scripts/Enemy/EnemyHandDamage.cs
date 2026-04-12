using UnityEngine;

public class EnemyHandDamage : MonoBehaviour
{
    private int damage;
    private bool canDamage = false;
    private Collider handCollider;

    private void Awake()
    {
        handCollider = GetComponent<Collider>();
        if (handCollider != null)
        {
            handCollider.isTrigger = true;
            handCollider.enabled = false; // Empieza apagado
        }
    }

    // El cerebro (EnemyAI) configura el daño antes de golpear
    public void Setup(int damageAmount)
    {
        damage = damageAmount;
    }

    public void SetDamageState(bool state)
    {
        canDamage = state;
        if (handCollider != null) handCollider.enabled = state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDamage) return;

        // Solo golpeamos si es el jugador
        if (other.CompareTag("Player"))
        {
            HealthSystem playerHealth = other.GetComponentInParent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                // Apagamos para no golpear mil veces en el mismo movimiento
                SetDamageState(false);
            }
        }
    }
}