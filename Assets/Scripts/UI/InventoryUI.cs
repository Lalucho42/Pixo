using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Textos de la UI")]
    public TextMeshProUGUI textoMadera;
    public TextMeshProUGUI textoPiedra;
    public TextMeshProUGUI textoParteComputadora; // <--- Nuevo texto para la parte

    private void Start()
    {
        Player player = FindFirstObjectByType<Player>();

        if (player != null && player.Inventory != null)
        {
            // Nos conectamos al evento que ahora manda 3 cosas
            player.Inventory.OnInventoryChanged += ActualizarPantalla;

            // Al empezar, mostramos los 3 valores
            ActualizarPantalla(player.Inventory.Madera, player.Inventory.Piedra, player.Inventory.ParteComputadora);
        }
    }

    // Ahora recibe los 3 numeros
    private void ActualizarPantalla(int cantidadMadera, int cantidadPiedra, int cantidadParte)
    {
        if (textoMadera != null) textoMadera.text = cantidadMadera.ToString();
        if (textoPiedra != null) textoPiedra.text = cantidadPiedra.ToString();
        // AGREGAMOS ESTO:
        if (textoParteComputadora != null) textoParteComputadora.text = cantidadParte.ToString();
    }
}