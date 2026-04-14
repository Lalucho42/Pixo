using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    [Header("UI Reference")]
    public CraftingUI menuCrafteo;

    public void Interact(Player player)
    {
        if (menuCrafteo != null)
        {
            menuCrafteo.AbrirMesa(player);
        }
    }
}
