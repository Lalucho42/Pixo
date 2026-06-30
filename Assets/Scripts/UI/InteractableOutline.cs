using UnityEngine;
using System.Collections.Generic;

public class InteractableOutline : MonoBehaviour
{
    [Header("Configuración del Outline")]
    [Tooltip("Arrastra aquí todas las mallas que forman este objeto y que requieran outline.")]
    public MeshRenderer[] mallasObjetivo;

    public string nombrePropiedadGrosor = "_OutlineThickness";
    public float grosorActivo = 0.025f;

    [Header("Modo de Activación")]
    [Tooltip("SÍ: Se activa solo por Trigger (Herramientas). NO: Lo controla otro script (Computadora).")]
    public bool usarTriggersAutomaticos = true;

    private List<Material> materialesOutlineInstancias = new List<Material>();
    private bool esActivo = false;

    private void Start()
    {
        if (mallasObjetivo == null || mallasObjetivo.Length == 0)
        {
            MeshRenderer mallaPropia = GetComponent<MeshRenderer>();
            if (mallaPropia != null)
            {
                mallasObjetivo = new MeshRenderer[] { mallaPropia };
            }
        }

        if (mallasObjetivo == null || mallasObjetivo.Length == 0) return;

        foreach (MeshRenderer malla in mallasObjetivo)
        {
            if (malla == null) continue;

            Material[] materiales = malla.materials;

            if (materiales.Length > 1)
            {
                Material matOutline = materiales[materiales.Length - 1];
                matOutline.SetFloat(nombrePropiedadGrosor, 0f); 
                materialesOutlineInstancias.Add(matOutline);
            }
            else
            {
                Debug.LogWarning($"[Outline] El objeto '{malla.gameObject.name}' no tiene un segundo material asignado en su MeshRenderer.");
            }
        }
    }

    public void SetOutline(bool active)
    {
        if (materialesOutlineInstancias.Count == 0 || active == esActivo) return;

        esActivo = active;
        float grosorDestino = active ? grosorActivo : 0f;

        foreach (Material mat in materialesOutlineInstancias)
        {
            if (mat != null)
            {
                mat.SetFloat(nombrePropiedadGrosor, grosorDestino);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!usarTriggersAutomaticos) return;

        if (other.GetComponent<Player>() != null || other.CompareTag("Player"))
        {
            SetOutline(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!usarTriggersAutomaticos) return;

        if (other.GetComponent<Player>() != null || other.CompareTag("Player"))
        {
            SetOutline(false);
        }
    }
}