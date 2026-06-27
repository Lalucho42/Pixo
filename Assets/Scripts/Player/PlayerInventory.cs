using UnityEngine;
using System;

public class PlayerInventory
{
    private Player player;
    public int Madera { get; private set; }
    public int Piedra { get; private set; }
    public int ParteComputadora { get; private set; } 
    public int Hierro { get; private set; }

    public event Action<int, int, int, int> OnInventoryChanged;

    public PlayerInventory(Player playerBrain)
    {
        player = playerBrain;
        Madera = 0;
        Piedra = 0;
        ParteComputadora = 0;
        Hierro = 0;
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (type == ResourceType.Madera) Madera = Madera + amount;
        else if (type == ResourceType.Piedra) Piedra = Piedra + amount;
        else if (type == ResourceType.ParteComputadora) ParteComputadora = ParteComputadora + amount;
        else if (type == ResourceType.Hierro) Hierro = Hierro + amount;

        if (OnInventoryChanged != null) OnInventoryChanged.Invoke(Madera, Piedra, ParteComputadora, Hierro);
    }

    public bool HasResource(ResourceType type, int amount)
    {
        if (type == ResourceType.Madera) return Madera >= amount;
        if (type == ResourceType.Piedra) return Piedra >= amount;
        if (type == ResourceType.ParteComputadora) return ParteComputadora >= amount;
        if (type == ResourceType.Hierro) return Hierro >= amount;
        return false;
    }

    public void ConsumeResource(ResourceType type, int amount)
    {
        if (type == ResourceType.Madera) Madera = Madera - amount;
        if (type == ResourceType.Piedra) Piedra = Piedra - amount;
        if (type == ResourceType.ParteComputadora) ParteComputadora = ParteComputadora - amount;
        if (type == ResourceType.Hierro) Hierro = Hierro - amount;

        if (OnInventoryChanged != null) OnInventoryChanged.Invoke(Madera, Piedra, ParteComputadora, Hierro);
    }
}