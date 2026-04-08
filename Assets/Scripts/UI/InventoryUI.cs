using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Textos de la UI")]
    public TextMeshProUGUI textoMadera;
    public TextMeshProUGUI textoPiedra;

    private void Start()
    {
        Player player = FindFirstObjectByType<Player>();

        if (player != null && player.Inventory != null)
        {
            player.Inventory.OnInventoryChanged += ActualizarPantalla;

            ActualizarPantalla(player.Inventory.Madera, player.Inventory.Piedra);
        }
    }

    private void ActualizarPantalla(int cantidadMadera, int cantidadPiedra)
    {
        if (textoMadera != null) textoMadera.text = cantidadMadera.ToString();
        if (textoPiedra != null) textoPiedra.text = cantidadPiedra.ToString();
    }
}