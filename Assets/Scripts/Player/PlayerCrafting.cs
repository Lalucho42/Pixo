using System.Collections.Generic;
using UnityEngine;

public class PlayerCrafting
{
    private Player player;

    public PlayerCrafting(Player playerBrain)
    {
        player = playerBrain;
    }

    // --- NUEVO: Ahora recibe una lista dinámica de costos ---
    public bool TryCraft(List<ResourceCost> costos)
    {
        // 1. Verificamos si tiene TODOS los materiales
        foreach (ResourceCost costo in costos)
        {
            if (!player.Inventory.HasResource(costo.tipoRecurso, costo.cantidad))
            {
                Debug.LogWarning("Módulo Crafting: Faltan recursos.");
                return false;
            }
        }

        // 2. Si tiene todo, cobramos
        foreach (ResourceCost costo in costos)
        {
            player.Inventory.ConsumeResource(costo.tipoRecurso, costo.cantidad);
        }

        return true;
    }

    public void AplicarEfectoMedikit() { Debug.Log("¡Medikit aplicado!"); }
    public void AplicarMejoraArma() { Debug.Log("¡Arma mejorada!"); }
    public void AplicarReparacionArma() { Debug.Log("¡Arma reparada!"); }
}