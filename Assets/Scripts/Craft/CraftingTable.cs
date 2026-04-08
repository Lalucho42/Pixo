using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    [Header("Referencia a la Interfaz")]
    public CraftingUI menuCrafteo; // El script que manejará la pantalla

    public void Interact(Player player)
    {
        if (menuCrafteo != null)
        {
            // Le pasamos el jugador a la UI para que sepa de qué inventario cobrar
            menuCrafteo.AbrirMesa(player);
        }
        else
        {
            Debug.LogWarning("¡Falta asignar el menú de crafteo en la mesa!");
        }
    }
}