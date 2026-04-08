using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceSlotUI : MonoBehaviour
{
    public Image icono;
    public TextMeshProUGUI textoCantidad;

    public void Configurar(Sprite spriteIcono, int cantidad)
    {
        if (icono != null) icono.sprite = spriteIcono;
        if (textoCantidad != null) textoCantidad.text = "X " + cantidad.ToString();
    }
}