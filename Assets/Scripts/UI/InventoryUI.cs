using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Textos de la UI")]
    public TextMeshProUGUI textoMadera;
    public TextMeshProUGUI textoPiedra;
    public TextMeshProUGUI textoParteComputadora;
    public TextMeshProUGUI textoHierro;

    private void Start()
    {
        Player player = FindFirstObjectByType<Player>();

        if (player != null && player.Inventory != null)
        {
            player.Inventory.OnInventoryChanged += ActualizarPantalla;

            ActualizarPantalla(player.Inventory.Madera, player.Inventory.Piedra, player.Inventory.ParteComputadora, player.Inventory.Hierro);
        }
    }

    private void ActualizarPantalla(int cantidadMadera, int cantidadPiedra, int cantidadParte, int cantidadHierro)
    {
        if (textoMadera != null) textoMadera.text = cantidadMadera.ToString();
        if (textoPiedra != null) textoPiedra.text = cantidadPiedra.ToString();
        if (textoParteComputadora != null) textoParteComputadora.text = cantidadParte.ToString();
        if (textoHierro != null) textoHierro.text = cantidadHierro.ToString();
    }
}