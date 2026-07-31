using System.Collections;
using UnityEngine;

public class CraftingUI : MonoBehaviour
{
    public static CraftingUI Instance;

    public GameObject panelCrafting;
    private Player jugadorActual;

    public bool IsOpen => panelCrafting != null && panelCrafting.activeSelf;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (panelCrafting != null) panelCrafting.SetActive(false);
    }

    public void AbrirMenu(Player player)
    {
        jugadorActual = player;
        panelCrafting.SetActive(true);

        jugadorActual.IsMovementLocked = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        RefrescarMenu();
    }

    public void RefrescarMenu()
    {
        if (jugadorActual == null) return;

        CraftingSlotUI[] slots = panelCrafting.GetComponentsInChildren<CraftingSlotUI>(true);
        foreach (CraftingSlotUI slot in slots)
        {
            slot.ConfigurarYActualizar(jugadorActual);
        }
    }

    public void CerrarMenu()
    {
        if (jugadorActual != null)
        {
            jugadorActual.IsMovementLocked = false;
        }

        panelCrafting.SetActive(false);
        jugadorActual = null;

        StopAllCoroutines();
        StartCoroutine(AnclarCursorAlFinalDeFrame());
    }

    private IEnumerator AnclarCursorAlFinalDeFrame()
    {
        yield return new WaitForEndOfFrame();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool IntentarCraftear(CraftingRecipe receta)
    {
        if (jugadorActual == null || receta == null) return false;

        if (receta.tipoReceta == RecipeType.CurarVida)
        {
            if (jugadorActual.Health.currentHealth >= jugadorActual.Health.maxHealth) return false;
        }
        else if (receta.tipoReceta == RecipeType.MejorarHerramienta)
        {
            ToolItem herramienta = BuscarHerramienta(receta.toolName);
            if (herramienta == null || herramienta.estaMejorada) return false;
        }
        else if (receta.tipoReceta == RecipeType.DesbloquearHerramienta)
        {
            ToolItem herramienta = BuscarHerramienta(receta.toolName);
            if (herramienta != null) return false;
        }

        foreach (ResourceCost costo in receta.costos)
        {
            if (!jugadorActual.Inventory.HasResource(costo.tipoRecurso, costo.cantidad))
            {
                return false;
            }
        }

        foreach (ResourceCost costo in receta.costos)
        {
            jugadorActual.Inventory.ConsumeResource(costo.tipoRecurso, costo.cantidad);
        }

        switch (receta.tipoReceta)
        {
            case RecipeType.CurarVida:
                jugadorActual.Health.Heal(receta.cantidadCuracion);
                break;

            case RecipeType.DesbloquearHerramienta:
                jugadorActual.WeaponManager.ActivateWeaponByName(receta.toolName);
                break;

            case RecipeType.MejorarHerramienta:
                ToolItem tool = BuscarHerramienta(receta.toolName);
                if (tool != null) tool.AplicarMejora();
                break;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX2D("Puzzle_Correcto");
        }

        RefrescarMenu();
        return true;
    }

    private ToolItem BuscarHerramienta(string nombre)
    {
        if (jugadorActual == null || jugadorActual.WeaponManager == null) return null;
        return jugadorActual.WeaponManager.unlockedWeapons.Find(t => t.toolName == nombre);
    }
}