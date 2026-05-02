using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Textos de la UI")]
    public TextMeshProUGUI textoMadera;
    public TextMeshProUGUI textoPiedra;
    public TextMeshProUGUI textoParteComputadora; 
    private void Start()
    {
        Player player = FindFirstObjectByType<Player>();

        if (player != null && player.Inventory != null)
        {
            player.Inventory.OnInventoryChanged += ActualizarPantalla;

            ActualizarPantalla(player.Inventory.Madera, player.Inventory.Piedra, player.Inventory.ParteComputadora);
        }
    }

    private void ActualizarPantalla(int cantidadMadera, int cantidadPiedra, int cantidadParte)
    {
        if (textoMadera != null) textoMadera.text = cantidadMadera.ToString();
        if (textoPiedra != null) textoPiedra.text = cantidadPiedra.ToString();
        if (textoParteComputadora != null) textoParteComputadora.text = cantidadParte.ToString();
    }
}