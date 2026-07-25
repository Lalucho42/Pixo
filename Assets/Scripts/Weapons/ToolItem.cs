using UnityEngine;
using System.Collections.Generic;

public class ToolItem : MonoBehaviour
{
    [Header("Identificacion")]
    public string toolName;
    public Sprite iconoUI;
    public float escalaIcono = 1f;

    [Header("Estadisticas de Combate")]
    public int attackDamage = 25;
    public float attackRate = 0.5f;
    public float knockbackForce = 3f;

    [Header("Estadisticas de Recursos")]
    public float resourceDamage = 50f;

    [Header("Durabilidad")]
    public int usosMaximos = 100;
    public int usosActuales;

    [Header("Evolucion")]
    public GameObject objetoModeloBase;
    public GameObject objetoModeloMejorado;
    public Sprite iconoMejorado;
    public int bonusDamage = 15;
    public bool estaMejorada { get; private set; } = false;

    [Header("Audio")]
    public AudioSource audioSource;

    private Collider damageCollider;
    private List<GameObject> alreadyHit = new List<GameObject>();
    private Player owner;

    private void Awake()
    {
        owner = GetComponentInParent<Player>();
        usosActuales = usosMaximos;

        damageCollider = GetComponent<Collider>();
        if (damageCollider == null) damageCollider = GetComponentInChildren<Collider>();

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

        Vector3 puntoImpacto = transform.position;
        Quaternion rotacionImpacto = Quaternion.LookRotation(transform.forward);

        ResourceNode node = other.GetComponentInParent<ResourceNode>();
        TutorialDummy dummy = other.GetComponentInParent<TutorialDummy>();
        HealthSystem health = other.GetComponentInParent<HealthSystem>();

        if (node != null)
        {
            float danoRecurso = CalculateEfficiency(node);
            node.TakeDamage(danoRecurso);
            alreadyHit.Add(other.gameObject);
            golpeoAlgo = true;

            string tagDinamico = "Impacto_" + node.type.ToString();

            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX3D(tagDinamico, audioSource);
            if (VFXManager.Instance != null) VFXManager.Instance.SpawnVFX(tagDinamico, puntoImpacto, rotacionImpacto);
        }
        else if (dummy != null)
        {
            dummy.TakeDamage(danoFinal);
            alreadyHit.Add(other.gameObject);
            golpeoAlgo = true;

            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX3D("Impacto_Enemigo", audioSource);
            if (VFXManager.Instance != null) VFXManager.Instance.SpawnVFX("Impacto_Enemigo", puntoImpacto, rotacionImpacto);
        }
        else if (health != null)
        {
            health.TakeDamage(danoFinal);
            alreadyHit.Add(other.gameObject);
            golpeoAlgo = true;

            EnemyAI enemy = other.GetComponentInParent<EnemyAI>();
            if (enemy != null)
            {
                enemy.EjecutarEfectosRecibirDaño(other.transform.position + Vector3.up);
            }
            else
            {
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX3D("Impacto_Enemigo", audioSource);
                if (VFXManager.Instance != null) VFXManager.Instance.SpawnVFX("Impacto_Enemigo", puntoImpacto, rotacionImpacto);
            }
        }

        if (golpeoAlgo) GastarDurabilidad();
    }

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