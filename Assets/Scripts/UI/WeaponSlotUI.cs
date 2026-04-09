using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    public Image icono;
    public Image barraDurabilidad;
    public GameObject marcoSeleccion;

    private ToolItem herramientaVinculada;

    public void Configurar(ToolItem tool)
    {
        herramientaVinculada = tool;
        ActualizarVisuales();
    }

    public void SetSeleccionado(bool esSeleccionado)
    {
        if (marcoSeleccion != null) marcoSeleccion.SetActive(esSeleccionado);
    }

    public void ActualizarVisuales()
    {
        if (herramientaVinculada == null) return;

        icono.sprite = herramientaVinculada.estaMejorada ? herramientaVinculada.iconoMejorado : herramientaVinculada.iconoUI;

        // Modificamos la escala según lo que pida el arma específica
        icono.rectTransform.localScale = Vector3.one * herramientaVinculada.escalaIcono;

        float fill = (float)herramientaVinculada.usosActuales / herramientaVinculada.usosMaximos;
        barraDurabilidad.fillAmount = fill;

        if (fill <= 0) barraDurabilidad.color = Color.red;
        else if (fill < 0.3f) barraDurabilidad.color = Color.yellow;
        else barraDurabilidad.color = Color.green;
    }

    private void Update()
    {
        ActualizarVisuales();
    }
}