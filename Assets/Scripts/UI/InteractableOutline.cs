using UnityEngine;

public class InteractableOutline : MonoBehaviour
{
    [Header("Configuración del Outline")]
    [Tooltip("Arrastra aquí el objeto 3D que tiene la malla (ej: pickaxe)")]
    public MeshRenderer mallaObjetivo;

    [Tooltip("El nombre de REFERENCIA en el Shader Graph. Ej: _OutlineThickness")]
    public string nombrePropiedadGrosor = "_OutlineThickness";

    [Tooltip("El grosor que tendrá el borde cuando mires el objeto")]
    public float grosorActivo = 0.025f;

    private Material materialOutlineInstancia;
    private bool esActivo = false;

    private void Start()
    {
        if (mallaObjetivo == null) return;

        // Obtenemos los materiales (esto crea una instancia única para este objeto)
        Material[] materiales = mallaObjetivo.materials;

        // Asumimos que el outline es el SEGUNDO material (el último de la lista)
        if (materiales.Length > 1)
        {
            materialOutlineInstancia = materiales[materiales.Length - 1];

            // Forzamos que empiece invisible (0)
            materialOutlineInstancia.SetFloat(nombrePropiedadGrosor, 0f);
        }
        else
        {
            Debug.LogWarning($"[Outline] {gameObject.name} no tiene un segundo material asignado en su MeshRenderer.");
        }
    }

    public void SetOutline(bool active)
    {
        if (materialOutlineInstancia == null || active == esActivo) return;

        esActivo = active;
        float grosorDestino = active ? grosorActivo : 0f;

        materialOutlineInstancia.SetFloat(nombrePropiedadGrosor, grosorDestino);
    }

    // --- NUEVA DETECCIÓN AUTOMÁTICA POR TRIGGERS ---

    private void OnTriggerEnter(Collider other)
    {
        // Si lo que entra al trigger tiene el script 'Player' (es el jugador)
        if (other.GetComponent<Player>() != null || other.CompareTag("Player"))
        {
            SetOutline(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Si lo que sale del trigger es el jugador
        if (other.GetComponent<Player>() != null || other.CompareTag("Player"))
        {
            SetOutline(false);
        }
    }
}