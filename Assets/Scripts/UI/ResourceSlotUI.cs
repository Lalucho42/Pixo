using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceSlotUI : MonoBehaviour
{
    public Image icono;
    public TextMeshProUGUI textoCantidad;

    public void Configurar(Sprite spriteIcono, int cantidad, bool tieneSuficiente = true)
    {
        if (icono != null && spriteIcono != null)
        {
            icono.sprite = spriteIcono;
        }

        if (textoCantidad != null)
        {
            textoCantidad.text = "X" + cantidad.ToString();
            textoCantidad.color = tieneSuficiente ? Color.white : new Color(1f, 0.3f, 0.3f);
        }
    }
}