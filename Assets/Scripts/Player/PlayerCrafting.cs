using System.Collections.Generic;
using UnityEngine;

public class PlayerCrafting
{
    private Player player;

    public PlayerCrafting(Player playerBrain)
    {
        player = playerBrain;
    }

    public bool TryCraft(List<ResourceCost> costos)
    {
        foreach (ResourceCost costo in costos)
        {
            if (!player.Inventory.HasResource(costo.tipoRecurso, costo.cantidad)) return false;
        }

        foreach (ResourceCost costo in costos)
        {
            player.Inventory.ConsumeResource(costo.tipoRecurso, costo.cantidad);
        }
        return true;
    }

    public void AplicarEfectoMedikit()
    {
        HealthSystem health = player.GetComponent<HealthSystem>();
        if (health != null) health.Heal(50);
    }

    // --- NUEVO: Busca el arma por el nombre que le pongas en el Inspector ---
    public ToolItem ObtenerArma(string nombreArma)
    {
        return player.WeaponManager.unlockedWeapons.Find(w => w.toolName == nombreArma);
    }

    public void AplicarMejoraArma(ToolItem armaObjetivo)
    {
        if (armaObjetivo != null) armaObjetivo.AplicarMejora();
    }

    public void AplicarReparacionArma(ToolItem armaObjetivo, int usosRestaurados)
    {
        if (armaObjetivo != null) armaObjetivo.RepararArma(usosRestaurados);
    }

    public List<ResourceCost> CalcularCostoReparacion(List<ResourceCost> costoBase, ToolItem armaObjetivo)
    {
        if (armaObjetivo == null) return costoBase;

        int usosFaltantes = armaObjetivo.usosMaximos - armaObjetivo.usosActuales;
        float porcentaje = (float)usosFaltantes / armaObjetivo.usosMaximos;

        List<ResourceCost> costoProporcional = new List<ResourceCost>();

        foreach (var c in costoBase)
        {
            int cantidadReal = Mathf.CeilToInt(c.cantidad * porcentaje);
            if (c.cantidad > 0 && usosFaltantes > 0 && cantidadReal == 0) cantidadReal = 1;
            costoProporcional.Add(new ResourceCost { tipoRecurso = c.tipoRecurso, cantidad = cantidadReal, icono = c.icono });
        }
        return costoProporcional;
    }
}