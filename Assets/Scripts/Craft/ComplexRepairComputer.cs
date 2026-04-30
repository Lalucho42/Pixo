using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComplexRepairComputer : MonoBehaviour, IInteractable
{
    [Header("Referencia a la Mina")]
    public ComplexRepairableStructure estructuraObjetivo;

    [Header("FASE 1: Arreglar la PC")]
    public List<ResourceCost> materialesParaPC;
    public GameObject modeloComputadoraRota;
    public GameObject modeloComputadoraEncendida;

    [Header("FASE 2: Arreglar los Escombros")]
    public List<ResourceCost> materialesParaMina;

    [Header("UI")]
    public StructureRepairUI uiFlotante;

    private bool laPCYaFunciona = false;
    private bool todoTerminado = false;

    void Start()
    {
        if (modeloComputadoraRota != null) modeloComputadoraRota.SetActive(true);
        if (modeloComputadoraEncendida != null) modeloComputadoraEncendida.SetActive(false);

        if (uiFlotante != null)
        {
            uiFlotante.ConfigurarCartel(materialesParaPC);
            uiFlotante.Ocultar();
        }
    }

    public void Interact(Player elJugador)
    {
        if (todoTerminado) return;

        // --- PASO 1: ARREGLAR LA COMPUTADORA ---
        if (laPCYaFunciona == false)
        {
            if (RevisarYQuitarMateriales(elJugador, materialesParaPC))
            {
                laPCYaFunciona = true;
                // Iniciamos el efecto de caida para la propia computadora
                StartCoroutine(EfectoCaidaComputadora());
            }
            return;
        }

        // --- PASO 2: ARREGLAR LA MINA ---
        if (laPCYaFunciona == true)
        {
            if (RevisarYQuitarMateriales(elJugador, materialesParaMina))
            {
                FinalizarTodo();
            }
        }
    }

    // --- NUEVO: Efecto de caida para la computadora ---
    IEnumerator EfectoCaidaComputadora()
    {
        if (modeloComputadoraRota != null) modeloComputadoraRota.SetActive(false);

        if (modeloComputadoraEncendida != null)
        {
            Vector3 posicionFinalPC = modeloComputadoraEncendida.transform.localPosition;
            // La movemos arriba para que caiga
            modeloComputadoraEncendida.transform.localPosition = posicionFinalPC + new Vector3(0, 10, 0);
            modeloComputadoraEncendida.SetActive(true);

            float progreso = 0;
            while (progreso < 1.0f)
            {
                progreso = progreso + Time.deltaTime * 4f;
                modeloComputadoraEncendida.transform.localPosition = Vector3.Lerp(modeloComputadoraEncendida.transform.localPosition, posicionFinalPC, progreso);
                yield return null;
            }
            modeloComputadoraEncendida.transform.localPosition = posicionFinalPC;
        }

        // Una vez que cayo la PC, cambiamos el cartel para la mina
        if (uiFlotante != null)
        {
            uiFlotante.ConfigurarCartel(materialesParaMina);
            uiFlotante.Mostrar();
        }
    }

    void FinalizarTodo()
    {
        todoTerminado = true;
        if (uiFlotante != null) uiFlotante.Ocultar();
        if (estructuraObjetivo != null) estructuraObjetivo.TriggerRepair();
    }

    bool RevisarYQuitarMateriales(Player p, List<ResourceCost> lista)
    {
        for (int i = 0; i < lista.Count; i++)
        {
            if (p.Inventory.HasResource(lista[i].tipoRecurso, lista[i].cantidad) == false) return false;
        }
        for (int i = 0; i < lista.Count; i++)
        {
            p.Inventory.ConsumeResource(lista[i].tipoRecurso, lista[i].cantidad);
        }
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && uiFlotante != null && !todoTerminado) uiFlotante.Mostrar();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && uiFlotante != null) uiFlotante.Ocultar();
    }
}