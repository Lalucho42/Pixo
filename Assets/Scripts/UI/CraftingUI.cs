using System.Collections.Generic;
using UnityEngine;

public enum CraftingActionType { Medikit, AbrirMenuMejora, AbrirMenuReparacion, MejorarArma, RepararArma }

[System.Serializable]
public class CraftingRecipe
{
    public string nombreReceta;
    public CraftingActionType accion;
    [Tooltip("El nombre exacto del arma en el script ToolItem")]
    public string nombreArmaObjetivo;
    public List<ResourceCost> costos;
}

public class CraftingUI : MonoBehaviour
{
    [Header("UI: Panel Principal")]
    public GameObject panelPrincipal;
    public Transform contenedorPrincipal;
    public List<CraftingRecipe> recetasPrincipales;

    [Header("UI: Panel Mejora")]
    public GameObject panelMejora;
    public Transform contenedorMejora;
    public List<CraftingRecipe> recetasMejora;

    [Header("UI: Panel Reparacion")]
    public GameObject panelReparacion;
    public Transform contenedorReparacion;
    public List<CraftingRecipe> recetasReparacion;

    [Header("Referencias Globales")]
    public GameObject prefabBotonReceta;
    private Player jugadorActual;

    private void Start() { CerrarMesa(); }

    public Player ObtenerJugador() { return jugadorActual; }

    public void AbrirMesa(Player player)
    {
        jugadorActual = player;
        jugadorActual.IsMovementLocked = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        CambiarPanel(panelPrincipal, contenedorPrincipal, recetasPrincipales);
    }

    public void CerrarMesa()
    {
        if (panelPrincipal != null) panelPrincipal.SetActive(false);
        if (panelMejora != null) panelMejora.SetActive(false);
        if (panelReparacion != null) panelReparacion.SetActive(false);

        if (jugadorActual != null)
        {
            jugadorActual.IsMovementLocked = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            jugadorActual = null;
        }
        Time.timeScale = 1f;
    }

    public void VolverAlPrincipal()
    {
        CambiarPanel(panelPrincipal, contenedorPrincipal, recetasPrincipales);
    }

    private void CambiarPanel(GameObject panelActivo, Transform contenedor, List<CraftingRecipe> listaRecetas)
    {
        panelPrincipal.SetActive(false);
        panelMejora.SetActive(false);
        panelReparacion.SetActive(false);

        panelActivo.SetActive(true);

        foreach (Transform child in contenedor) Destroy(child.gameObject);

        foreach (CraftingRecipe receta in listaRecetas)
        {
            GameObject nuevoBoton = Instantiate(prefabBotonReceta, contenedor);
            nuevoBoton.transform.localScale = Vector3.one;
            nuevoBoton.GetComponent<RecipeButtonUI>()?.Configurar(receta, this);
        }
    }

    public void EjecutarAccion(CraftingRecipe receta)
    {
        if (receta.accion == CraftingActionType.AbrirMenuMejora)
        {
            CambiarPanel(panelMejora, contenedorMejora, recetasMejora);
            return;
        }
        if (receta.accion == CraftingActionType.AbrirMenuReparacion)
        {
            CambiarPanel(panelReparacion, contenedorReparacion, recetasReparacion);
            return;
        }

        List<ResourceCost> costoFinal = receta.costos;
        ToolItem armaElegida = null;
        int usosAReparar = 0;

        if (receta.accion == CraftingActionType.MejorarArma || receta.accion == CraftingActionType.RepararArma)
        {
            armaElegida = jugadorActual.Crafting.ObtenerArma(receta.nombreArmaObjetivo);
            if (armaElegida == null) return;

            if (receta.accion == CraftingActionType.MejorarArma && armaElegida.estaMejorada) return;

            if (receta.accion == CraftingActionType.RepararArma)
            {
                if (armaElegida.usosActuales >= armaElegida.usosMaximos) return;
                usosAReparar = armaElegida.usosMaximos - armaElegida.usosActuales;
                costoFinal = jugadorActual.Crafting.CalcularCostoReparacion(receta.costos, armaElegida);
            }
        }

        if (jugadorActual.Crafting.TryCraft(costoFinal))
        {
            if (receta.accion == CraftingActionType.Medikit) jugadorActual.Crafting.AplicarEfectoMedikit();
            else if (receta.accion == CraftingActionType.MejorarArma) jugadorActual.Crafting.AplicarMejoraArma(armaElegida);
            else if (receta.accion == CraftingActionType.RepararArma) jugadorActual.Crafting.AplicarReparacionArma(armaElegida, usosAReparar);

            if (panelPrincipal.activeSelf) CambiarPanel(panelPrincipal, contenedorPrincipal, recetasPrincipales);
            else if (panelMejora.activeSelf) CambiarPanel(panelMejora, contenedorMejora, recetasMejora);
            else if (panelReparacion.activeSelf) CambiarPanel(panelReparacion, contenedorReparacion, recetasReparacion);
        }
    }
}