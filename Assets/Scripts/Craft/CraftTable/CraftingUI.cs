using UnityEngine;

public class CraftingUI : MonoBehaviour
{
    public static CraftingUI Instance;

    public GameObject panelCrafting;
    private Player jugadorActual;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (panelCrafting != null)
        {
            panelCrafting.SetActive(false);
        }
    }

    public void AbrirMenu(Player player)
    {
        jugadorActual = player;
        panelCrafting.SetActive(true);

        jugadorActual.IsMovementLocked = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CerrarMenu()
    {
        if (jugadorActual != null)
        {
            jugadorActual.IsMovementLocked = false;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        panelCrafting.SetActive(false);
        jugadorActual = null;
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

        return true;
    }

    private ToolItem BuscarHerramienta(string nombre)
    {
        if (jugadorActual == null || jugadorActual.WeaponManager == null) return null;
        return jugadorActual.WeaponManager.unlockedWeapons.Find(t => t.toolName == nombre);
    }
}