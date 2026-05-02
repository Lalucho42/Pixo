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

    [Header("Bloqueo de Paso")]
    public GameObject paredInvisible; 

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

        if (paredInvisible != null) paredInvisible.SetActive(true);
        if (modeloReparado != null) modeloReparado.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (estaReparado) return;
        if (other.CompareTag("Player") && uiFlotante != null) uiFlotante.Mostrar();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && uiFlotante != null) uiFlotante.Ocultar();
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

        StartCoroutine(EfectoCaidaSimple());
    }

    private IEnumerator EfectoCaidaSimple()
    {
        if (modeloRoto != null) modeloRoto.SetActive(false);

        if (modeloReparado != null)
        {
            Vector3 posFinal = modeloReparado.transform.localPosition;
            modeloReparado.transform.localPosition = posFinal + new Vector3(0, 15f, 0);
            modeloReparado.SetActive(true);

            float progreso = 0f;
            while (progreso < 1f)
            {
                progreso += Time.deltaTime * 3.5f;
                modeloReparado.transform.localPosition = Vector3.Lerp(modeloReparado.transform.localPosition, posFinal, progreso);
                yield return null;
            }
            modeloReparado.transform.localPosition = posFinal;
        }

        
        if (paredInvisible != null) paredInvisible.SetActive(false);
    }
}