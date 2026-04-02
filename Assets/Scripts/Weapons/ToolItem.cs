using UnityEngine;
using System.Collections.Generic;

public class ToolItem : MonoBehaviour
{
    [Header("Estadisticas de Combate")]
    public string toolName; // "Palo", "Pico" o "Hacha"
    public int attackDamage = 25;
    public float attackRate = 0.5f;
    public float attackRange = 1.5f; // <--- FALTABA ESTA
    public float knockbackForce = 3f;
    public AudioClip attackSound;   // <--- FALTABA ESTA

    [Header("Estadisticas de Recursos")]
    public float resourceDamage = 50f;

    private Collider damageCollider;
    private List<GameObject> alreadyHit = new List<GameObject>();
    private Player owner;

    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        if (damageCollider != null)
        {
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        owner = GetComponentInParent<Player>();

        // El arma necesita Rigidbody (Kinematic) para detectar colisiones trigger
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) { rb = gameObject.AddComponent<Rigidbody>(); }
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public virtual void OnEquip() { }
    public virtual void OnUnequip() { DisableDamage(); }

    public void EnableDamage()
    {
        alreadyHit.Clear();
        if (damageCollider != null) damageCollider.enabled = true;
    }

    public void DisableDamage()
    {
        if (damageCollider != null) damageCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner.gameObject) return;
        if (alreadyHit.Contains(other.gameObject)) return;

        // Enemigos
        if (other.CompareTag("Enemy"))
        {
            HealthSystem health = other.GetComponentInParent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(attackDamage);
                alreadyHit.Add(other.gameObject);
            }
        }

        // Recursos
        ResourceNode node = other.GetComponentInParent<ResourceNode>();
        if (node != null)
        {
            float finalDmg = CalculateEfficiency(node);
            node.TakeDamage(finalDmg);
            alreadyHit.Add(other.gameObject);
        }
    }

    private float CalculateEfficiency(ResourceNode node)
    {
        if (toolName == "Hacha" && node.type == ResourceType.Madera) return resourceDamage;
        if (toolName == "Pico" && node.type == ResourceType.Piedra) return resourceDamage;
        if (toolName == "Palo") return resourceDamage * 0.3f;
        return resourceDamage * 0.1f;
    }
}