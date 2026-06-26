using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComplexRepairComputer : MonoBehaviour, IInteractable
{
    public ComplexRepairableStructure estructuraDeLosEscombros;

    [Header("FASE 1: Materiales para la PC")]
    public bool laPCYaFunciona = false;
    public List<ResourceCost> costosPC;
    public GameObject pcRota;
    public GameObject pcEncendida;

    [Header("FASE 2: Materiales para la Mina")]
    public List<ResourceCost> costosMina;

    [Header("UI del Cartel")]
    public StructureRepairUI cartelVisual;

    [Header("Textos de Misión Automáticos (Opcional)")]
    public string misionAlAcercarse;
    public string misionAlCompletar;

    [Header("Efecto de Outline Visual")]
    public InteractableOutline scriptOutline;

    private BasePuzzleModule scriptDelPuzzle;
    private bool todoElNivelEstaTerminado = false;

    void Start()
    {
        scriptDelPuzzle = GetComponent<BasePuzzleModule>();

        if (scriptOutline != null)
        {
            scriptOutline.SetOutline(false);
        }

        if (laPCYaFunciona)
        {
            if (pcRota != null) pcRota.SetActive(false);
            if (pcEncendida != null) pcEncendida.SetActive(true);
            if (cartelVisual != null)
            {
                cartelVisual.ConfigurarCartel(costosMina);
                cartelVisual.Ocultar();
            }
        }
        else
        {
            if (pcRota != null) pcRota.SetActive(true);
            if (pcEncendida != null) pcEncendida.SetActive(false);
            if (cartelVisual != null)
            {
                cartelVisual.ConfigurarCartel(costosPC);
                cartelVisual.Ocultar();
            }
        }
    }

    public void Interact(Player jugador)
    {
        if (todoElNivelEstaTerminado) return;

        if (laPCYaFunciona == false)
        {
            if (RevisarSiTieneMateriales(jugador, costosPC))
            {
                laPCYaFunciona = true;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX2D("PC_Reparada");
                }
                StartCoroutine(EfectoCaidaPC());
            }
            return;
        }

        if (laPCYaFunciona == true)
        {
            if (RevisarSiTieneMateriales(jugador, costosMina))
            {
                if (scriptDelPuzzle != null)
                {
                    if (cartelVisual != null) cartelVisual.Ocultar();

                    scriptDelPuzzle.AlTerminarElPuzzle = (resultado) => {
                        if (resultado == true) FinalizarTodoElNivel();
                    };

                    scriptDelPuzzle.StartPuzzle();
                }
                else
                {
                    FinalizarTodoElNivel();
                }
            }
        }
    }

    IEnumerator EfectoCaidaPC()
    {
        if (pcRota != null) pcRota.SetActive(false);
        if (pcEncendida != null)
        {
            Vector3 sitioFinal = pcEncendida.transform.localPosition;
            pcEncendida.transform.localPosition = sitioFinal + new Vector3(0, 10, 0);
            pcEncendida.SetActive(true);

            float tiempo = 0;
            while (tiempo < 1.0f)
            {
                tiempo = tiempo + Time.deltaTime * 4.0f;
                pcEncendida.transform.localPosition = Vector3.Lerp(pcEncendida.transform.localPosition, sitioFinal, tiempo);
                yield return null;
            }
            pcEncendida.transform.localPosition = sitioFinal;
        }

        if (cartelVisual != null)
        {
            cartelVisual.ConfigurarCartel(costosMina);
            cartelVisual.Mostrar();
        }
    }

    void FinalizarTodoElNivel()
    {
        todoElNivelEstaTerminado = true;
        if (cartelVisual != null) cartelVisual.Ocultar();

        if (scriptOutline != null)
        {
            scriptOutline.SetOutline(false);
        }

        if (!string.IsNullOrEmpty(misionAlCompletar) && MissionUI.Instance != null)
        {
            MissionUI.Instance.ActualizarMision(misionAlCompletar);
        }

        if (estructuraDeLosEscombros != null) estructuraDeLosEscombros.TriggerRepair();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !todoElNivelEstaTerminado)
        {
            if (cartelVisual != null) cartelVisual.Mostrar();

            if (scriptOutline != null)
            {
                scriptOutline.SetOutline(true);
            }

            if (!string.IsNullOrEmpty(misionAlAcercarse) && MissionUI.Instance != null)
            {
                MissionUI.Instance.ActualizarMision(misionAlAcercarse);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && cartelVisual != null)
        {
            cartelVisual.Ocultar();

            if (scriptOutline != null)
            {
                scriptOutline.SetOutline(false);
            }
        }
    }

    bool RevisarSiTieneMateriales(Player p, List<ResourceCost> lista)
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
}