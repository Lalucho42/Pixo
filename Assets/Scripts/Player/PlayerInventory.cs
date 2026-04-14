using UnityEngine;
using System;

public class PlayerInventory
{
    private Player player;
    public int Madera { get; private set; }
    public int Piedra { get; private set; }

    public event Action<int, int> OnInventoryChanged;

    public PlayerInventory(Player playerBrain)
    {
        player = playerBrain;
        Madera = 0;
        Piedra = 0;
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (type == ResourceType.Madera) Madera += amount;
        else if (type == ResourceType.Piedra) Piedra += amount;

        if (OnInventoryChanged != null) OnInventoryChanged.Invoke(Madera, Piedra);
    }

    public bool HasResource(ResourceType type, int amount)
    {
        if (type == ResourceType.Madera) return Madera >= amount;
        if (type == ResourceType.Piedra) return Piedra >= amount;
        return false;
    }

    public void ConsumeResource(ResourceType type, int amount)
    {
        if (type == ResourceType.Madera) Madera -= amount;
        if (type == ResourceType.Piedra) Piedra -= amount;

        if (OnInventoryChanged != null) OnInventoryChanged.Invoke(Madera, Piedra);
    }
}