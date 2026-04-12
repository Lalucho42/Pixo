using UnityEngine;
using System.Collections.Generic;

public class ToolItem : MonoBehaviour
{
    [Header("Identificación")]
    public string toolName;
    public Sprite iconoUI;
    [Range(0.1f, 3f)] public float escalaIcono = 1f;

    [Header("Estadísticas de Combate")]
    public int attackDamage = 25;
    public float attackRate = 0.5f;
    public float knockbackForce = 3f;
    public AudioClip attackSound;

    [Header("Estadísticas de Recursos")]
    public float resourceDamage = 50f;

    [Header("Durabilidad")]
    public int usosMaximos = 100;
    public int usosActuales;

    [Header("Evolución (Opcional)")]
    // --- ESTOS SON LOS DOS HUECOS MÁGICOS ---
    [Tooltip("El objeto que contiene el modelo 3D viejo (debe estar activado)")]
    public GameObject objetoModeloBase;
    [Tooltip("El objeto que contiene el modelo 3D mejorado (debe estar desactivado)")]
    public GameObject objetoModeloMejorado;

    public Sprite iconoMejorado;
    public int bonusDamage = 15;
    public bool estaMejorada { get; private set; } = false;

    private Collider damageCollider;
    private List<GameObject> alreadyHit = new List<GameObject>();
    private Player owner;

    private void Awake()
    {
        owner = GetComponentInParent<Player>();
        usosActuales = usosMaximos;

        damageCollider = GetComponent<Collider>();
        if (damageCollider != null)
        {
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        // --- PREPARACIÓN AL ARRANCAR ---
        // Nos aseguramos de que el modelo base esté prendido y el mejorado apagado.
        if (objetoModeloBase != null) objetoModeloBase.SetActive(true);
        if (objetoModeloMejorado != null) objetoModeloMejorado.SetActive(false);

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

        if (attackSound != null && AudioManager.instance != null)
            AudioManager.instance.PlaySFX(attackSound);
    }

    public void DisableDamage()
    {
        if (damageCollider != null) damageCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (usosActuales <= 0) return;

        if (owner != null && other.gameObject == owner.gameObject) return;
        if (alreadyHit.Contains(other.gameObject)) return;

        bool golpeoAlgo = false;
        int danoFinal = estaMejorada ? attackDamage + bonusDamage : attackDamage;

        if (other.CompareTag("Enemy"))
        {
            HealthSystem health = other.GetComponentInParent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(danoFinal);
                alreadyHit.Add(other.gameObject);
                golpeoAlgo = true;
            }
        }

        ResourceNode node = other.GetComponentInParent<ResourceNode>();
        if (node != null)
        {
            float danoRecurso = CalculateEfficiency(node);
            node.TakeDamage(danoRecurso);
            alreadyHit.Add(other.gameObject);
            golpeoAlgo = true;
        }

        if (golpeoAlgo)
        {
            GastarDurabilidad();
        }
    }

    private void GastarDurabilidad()
    {
        usosActuales--;
        if (usosActuales < 0) usosActuales = 0;

        if (usosActuales == 0)
        {
            Debug.Log(toolName + " se ha roto.");
        }
    }

    public void RepararArma(int cantidadRestaurada)
    {
        usosActuales += cantidadRestaurada;
        if (usosActuales > usosMaximos) usosActuales = usosMaximos;
        Debug.Log(toolName + " reparada. Usos: " + usosActuales);
    }

    // --- AQUÍ ESTÁ LA LÓGICA SIMPLE DE LA MEJORA ---
    public void AplicarMejora()
    {
        if (estaMejorada) return;

        estaMejorada = true;

        // 1. Apagamos el objeto contenedor del modelo viejo
        if (objetoModeloBase != null) objetoModeloBase.SetActive(false);

        // 2. Prendemos el objeto contenedor del modelo nuevo
        if (objetoModeloMejorado != null) objetoModeloMejorado.SetActive(true);

        Debug.Log(toolName + " fue mejorada al máximo nivel.");
    }

    private float CalculateEfficiency(ResourceNode node)
    {
        float multiplier = estaMejorada ? 1.5f : 1f;

        if (toolName == "Hacha" && node.type == ResourceType.Madera) return resourceDamage * multiplier;
        if (toolName == "Pico" && node.type == ResourceType.Piedra) return resourceDamage * multiplier;
        if (toolName == "Palo") return resourceDamage * 0.3f * multiplier;

        return resourceDamage * 0.1f * multiplier;
    }
}