using UnityEngine;

public class CraftingWorkbench : MonoBehaviour, IInteractable
{
    public void Interact(Player player)
    {
        if (CraftingUI.Instance != null)
        {
            CraftingUI.Instance.AbrirMenu(player);
        }
    }
}