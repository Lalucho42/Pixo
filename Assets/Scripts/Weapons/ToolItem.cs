using UnityEngine;
using System.Collections.Generic;

public class ToolItem : MonoBehaviour
{
    [Header("Identificacion")]
    public string toolName;
    public Sprite iconoUI;
    [Range(0.1f, 3f)] public float escalaIcono = 1f;

    [Header("Estadisticas de Combate")]
    public int attackDamage = 25;
    public float attackRate = 0.5f;
    public float knockbackForce = 3f;

    [Header("Estadisticas de Recursos")]
    public float resourceDamage = 50f;

    [Header("Durabilidad")]
    public int usosMaximos = 100;
    public int usosActuales;

    [Header("Evolucion (Opcional)")]
    [Tooltip("El objeto que contiene el modelo 3D base")]
    public GameObject objetoModeloBase;
    [Tooltip("El objeto que contiene el modelo 3D mejorado")]
    public GameObject objetoModeloMejorado;

    public Sprite iconoMejorado;
    public int bonusDamage = 15;
    public bool estaMejorada { get; private set; } = false;

    // --- NUEVA SECCIÓN DE AUDIO ---
    [Header("Audio de Impactos")]
    public AudioSource audioSource;
    public AudioClip sonidoGolpeEnemigo; // Sonido exclusivo para enemigos

    [System.Serializable]
    public struct SonidoRecurso
    {
        public ResourceType material;
        public AudioClip clip;
    }
    public List<SonidoRecurso> sonidosDeRecursos; // Lista de sonidos para recursos
    private Dictionary<ResourceType, AudioClip> diccionarioSonidos;
    // -------------------------------

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

        if (objetoModeloBase != null) objetoModeloBase.SetActive(true);
        if (objetoModeloMejorado != null) objetoModeloMejorado.SetActive(false);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) { rb = gameObject.AddComponent<Rigidbody>(); }
        rb.isKinematic = true;
        rb.useGravity = false;

        // INICIALIZAMOS EL DICCIONARIO DE SONIDOS
        diccionarioSonidos = new Dictionary<ResourceType, AudioClip>();
        foreach (var item in sonidosDeRecursos)
        {
            if (!diccionarioSonidos.ContainsKey(item.material))
            {
                diccionarioSonidos.Add(item.material, item.clip);
            }
        }
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
        if (usosActuales <= 0) return;

        if (owner != null && other.gameObject == owner.gameObject) return;
        if (alreadyHit.Contains(other.gameObject)) return;

        bool golpeoAlgo = false;
        int danoFinal = estaMejorada ? attackDamage + bonusDamage : attackDamage;

        // 1. SI GOLPEAMOS A UN ENEMIGO
        if (other.CompareTag("Enemy"))
        {
            HealthSystem health = other.GetComponentInParent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(danoFinal);
                alreadyHit.Add(other.gameObject);
                golpeoAlgo = true;

                // Reproducir sonido de enemigo
                ReproducirSonidoDeGolpe(sonidoGolpeEnemigo);
            }
        }

        // 2. SI GOLPEAMOS UN RECURSO
        ResourceNode node = other.GetComponentInParent<ResourceNode>();
        if (node != null)
        {
            float danoRecurso = CalculateEfficiency(node);
            node.TakeDamage(danoRecurso);
            alreadyHit.Add(other.gameObject);
            golpeoAlgo = true;

            // Buscar y reproducir sonido según el material
            if (diccionarioSonidos.TryGetValue(node.type, out AudioClip sonidoRecurso))
            {
                ReproducirSonidoDeGolpe(sonidoRecurso);
            }
        }

        if (golpeoAlgo)
        {
            GastarDurabilidad();
        }
    }

    // --- NUEVO MÉTODO PARA REPRODUCIR SONIDO ---
    private void ReproducirSonidoDeGolpe(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f); // Da variedad al sonido
            audioSource.PlayOneShot(clip);
        }
    }
    // -------------------------------------------

    private void GastarDurabilidad()
    {
        usosActuales--;
        if (usosActuales < 0) usosActuales = 0;
    }

    public void RepararArma(int cantidadRestaurada)
    {
        usosActuales += cantidadRestaurada;
        if (usosActuales > usosMaximos) usosActuales = usosMaximos;
    }

    public void AplicarMejora()
    {
        if (estaMejorada) return;

        estaMejorada = true;

        if (objetoModeloBase != null) objetoModeloBase.SetActive(false);
        if (objetoModeloMejorado != null) objetoModeloMejorado.SetActive(true);
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