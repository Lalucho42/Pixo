using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourceCost
{
    public ResourceType tipoRecurso;
    public int cantidad;
    public Sprite icono;
}

public class RepairableStructure : MonoBehaviour, IInteractable
{
    [Header("Costos de Reparacion")]
    public List<ResourceCost> costosDeReparacion;

    [Header("Visuales 3D")]
    public GameObject modeloRoto;
    public GameObject modeloReparado;

    [Header("UI Flotante")]
    public StructureRepairUI uiFlotante;

    private bool estaReparado = false;

    private void Start()
    {
        if (uiFlotante != null)
        {
            uiFlotante.ConfigurarCartel(costosDeReparacion);
            uiFlotante.Ocultar();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (estaReparado) return;
        Player player = other.GetComponent<Player>();
        if (player != null && uiFlotante != null) uiFlotante.Mostrar();
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null && uiFlotante != null) uiFlotante.Ocultar();
    }

    public void Interact(Player player)
    {
        if (estaReparado) return;

        foreach (ResourceCost costo in costosDeReparacion)
        {
            if (!player.Inventory.HasResource(costo.tipoRecurso, costo.cantidad)) return;
        }

        foreach (ResourceCost costo in costosDeReparacion)
        {
            player.Inventory.ConsumeResource(costo.tipoRecurso, costo.cantidad);
        }

        estaReparado = true;
        if (uiFlotante != null) uiFlotante.Ocultar();

        StartCoroutine(EfectoImpactoConstruccion());
    }

    private IEnumerator EfectoImpactoConstruccion()
    {
        if (modeloRoto != null)
        {
            Renderer[] graficosRotos = modeloRoto.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in graficosRotos)
            {
                r.enabled = false;
            }
        }

        if (modeloReparado != null)
        {
            Vector3 posOriginal = modeloReparado.transform.localPosition;
            Vector3 escalaOriginal = modeloReparado.transform.localScale;

            Vector3 posCielo = posOriginal + new Vector3(0, 15f, 0);
            modeloReparado.transform.localPosition = posCielo;
            modeloReparado.SetActive(true);

            float progreso = 0f;
            float velocidadCaida = 3.5f;
            while (progreso < 1f)
            {
                progreso += Time.deltaTime * velocidadCaida;
                float easeIn = progreso * progreso;
                modeloReparado.transform.localPosition = Vector3.Lerp(posCielo, posOriginal, easeIn);
                yield return null;
            }

            modeloReparado.transform.localPosition = posOriginal;

            if (modeloRoto != null) modeloRoto.SetActive(false);

            Vector3 escalaAplastada = new Vector3(escalaOriginal.x * 1.15f, escalaOriginal.y * 0.7f, escalaOriginal.z * 1.15f);
            modeloReparado.transform.localScale = escalaAplastada;

            progreso = 0f;
            float velocidadRebote = 8f;
            while (progreso < 1f)
            {
                progreso += Time.deltaTime * velocidadRebote;
                modeloReparado.transform.localScale = Vector3.Lerp(escalaAplastada, escalaOriginal, progreso);
                yield return null;
            }
            modeloReparado.transform.localScale = escalaOriginal;
        }
    }
}