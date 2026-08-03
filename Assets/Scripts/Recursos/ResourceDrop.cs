using UnityEngine;

public class ResourceDrop : MonoBehaviour, IInteractable
{
    public ResourceType tipoDeRecurso;
    public int cantidad = 1;

    [Header("Configuración del Imán")]
    public float radioDeAtraccion = 0.5f;
    public float velocidadDeVuelo = 6f;

    [Tooltip("Segundos antes de que el imán y la recolección se activen")]
    public float tiempoDeEspera = 3f;

    [Header("Efectos Visuales (Particle System)")]
    public ParticleSystem sistemaParticulas;

    private bool estaMagnetizado = false;
    private float tiempoDeVida = 0f;
    private Transform playerTransform;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (Player.Instance != null)
        {
            playerTransform = Player.Instance.transform;
        }

        ConfigurarParticulas();
    }

    private void ConfigurarParticulas()
    {
        if (sistemaParticulas == null)
        {
            sistemaParticulas = GetComponentInChildren<ParticleSystem>();
        }

        if (sistemaParticulas != null)
        {
            MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                var shapeModule = sistemaParticulas.shape;
                shapeModule.enabled = true;
                shapeModule.shapeType = ParticleSystemShapeType.Mesh;
                shapeModule.mesh = meshFilter.sharedMesh;
            }

            sistemaParticulas.Play();
        }
    }

    private void Update()
    {
        tiempoDeVida += Time.deltaTime;

        if (tiempoDeVida < tiempoDeEspera) return;

        if (tipoDeRecurso == ResourceType.ParteComputadora || playerTransform == null) return;

        float distancia = Vector3.Distance(transform.position, playerTransform.position);

        if (distancia <= radioDeAtraccion && !estaMagnetizado)
        {
            estaMagnetizado = true;
            if (rb != null) rb.isKinematic = true;
        }

        if (estaMagnetizado)
        {
            Vector3 objetivo = playerTransform.position + Vector3.up * 1f;
            transform.position = Vector3.MoveTowards(transform.position, objetivo, velocidadDeVuelo * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tiempoDeVida < tiempoDeEspera) return;

        if (tipoDeRecurso != ResourceType.ParteComputadora)
        {
            Player jugador = other.GetComponent<Player>();

            if (jugador != null && jugador.Inventory != null)
            {
                AgarrarObjeto(jugador);
            }
        }
    }

    public void Interact(Player jugador)
    {
        if (tiempoDeVida < tiempoDeEspera) return;

        if (tipoDeRecurso == ResourceType.ParteComputadora)
        {
            AgarrarObjeto(jugador);
        }
    }

    private void AgarrarObjeto(Player jugador)
    {
        jugador.Inventory.AddResource(tipoDeRecurso, cantidad);

        if (sistemaParticulas != null)
        {
            sistemaParticulas.Stop();
        }

        Destroy(gameObject);
    }
}