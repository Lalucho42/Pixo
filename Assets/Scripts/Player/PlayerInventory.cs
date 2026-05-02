using UnityEngine;
using System;

public class PlayerInventory
{
    private Player player;
    public int Madera { get; private set; }
    public int Piedra { get; private set; }
    public int ParteComputadora { get; private set; } 
    public event Action<int, int, int> OnInventoryChanged;

    public PlayerInventory(Player playerBrain)
    {
        player = playerBrain;
        Madera = 0;
        Piedra = 0;
        ParteComputadora = 0;
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (type == ResourceType.Madera) Madera = Madera + amount;
        else if (type == ResourceType.Piedra) Piedra = Piedra + amount;
        else if (type == ResourceType.ParteComputadora) ParteComputadora = ParteComputadora + amount;

        if (OnInventoryChanged != null) OnInventoryChanged.Invoke(Madera, Piedra, ParteComputadora);
    }

    public bool HasResource(ResourceType type, int amount)
    {
        if (type == ResourceType.Madera) return Madera >= amount;
        if (type == ResourceType.Piedra) return Piedra >= amount;
        if (type == ResourceType.ParteComputadora) return ParteComputadora >= amount;
        return false;
    }

    public void ConsumeResource(ResourceType type, int amount)
    {
        if (type == ResourceType.Madera) Madera = Madera - amount;
        if (type == ResourceType.Piedra) Piedra = Piedra - amount;
        if (type == ResourceType.ParteComputadora) ParteComputadora = ParteComputadora - amount;

        if (OnInventoryChanged != null) OnInventoryChanged.Invoke(Madera, Piedra, ParteComputadora);
    }
}