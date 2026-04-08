using System.Collections.Generic;
using UnityEngine;

public enum CraftingActionType { Medikit, MejorarArma, RepararArma }

[System.Serializable]
public class CraftingRecipe
{
    public string nombreReceta;
    public CraftingActionType accion;
    public List<ResourceCost> costos;
}

public class CraftingUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelPrincipal;

    [Header("Generador Dinámico")]
    public GameObject prefabBotonReceta;
    public Transform contenedorBotones;

    [Header("Lista de Recetas (Inspector)")]
    public List<CraftingRecipe> recetasDisponibles;

    private Player jugadorActual;
    private bool botonesGenerados = false;

    private void Start() { CerrarMesa(); }

    public void AbrirMesa(Player player)
    {
        jugadorActual = player;
        if (panelPrincipal != null) panelPrincipal.SetActive(true);

        jugadorActual.IsMovementLocked = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        // Generamos los botones solo la primera vez que abrimos la mesa
        if (!botonesGenerados)
        {
            GenerarBotones();
            botonesGenerados = true;
        }
    }

    public void CerrarMesa()
    {
        if (panelPrincipal != null) panelPrincipal.SetActive(false);

        if (jugadorActual != null)
        {
            jugadorActual.IsMovementLocked = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            jugadorActual = null;
        }
        Time.timeScale = 1f;
    }

    private void GenerarBotones()
    {
        foreach (Transform child in contenedorBotones) Destroy(child.gameObject);

        foreach (CraftingRecipe receta in recetasDisponibles)
        {
            GameObject nuevoBoton = Instantiate(prefabBotonReceta, contenedorBotones);
            RecipeButtonUI btnScript = nuevoBoton.GetComponent<RecipeButtonUI>();
            if (btnScript != null) btnScript.Configurar(receta, this);
        }
    }

    // Esta función la llaman los botones generados
    public void IntentarCraftear(CraftingRecipe receta)
    {
        if (jugadorActual == null) return;

        // Si el jugador tiene con qué pagar, la transacción se aprueba
        if (jugadorActual.Crafting.TryCraft(receta.costos))
        {
            switch (receta.accion)
            {
                case CraftingActionType.Medikit:
                    jugadorActual.Crafting.AplicarEfectoMedikit();
                    break;
                case CraftingActionType.MejorarArma:
                    jugadorActual.Crafting.AplicarMejoraArma();
                    break;
                case CraftingActionType.RepararArma:
                    jugadorActual.Crafting.AplicarReparacionArma();
                    break;
            }
        }
    }
}