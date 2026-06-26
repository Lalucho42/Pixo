using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GestorDeEscenas : MonoBehaviour
{
    [Header("Configuración de Transición")]
    public CanvasGroup panelFade;
    public float duracionFade = 1f;

    [Header("UI de Carga Asincrónica (Opcional para el Parcial)")]
    public GameObject contenedorLoading;
    public Slider barraProgreso;
    public TextMeshProUGUI textoPorcentaje;

    private void Start()
    {
        if (contenedorLoading != null)
        {
            contenedorLoading.SetActive(false);
        }

        if (panelFade != null)
        {
            panelFade.alpha = 1f;
            panelFade.blocksRaycasts = true;
            StartCoroutine(FadeDeEntrada());
        }
    }

    public void CargarJuego(string nombreDeLaEscena)
    {
        StartCoroutine(TransicionYCarga(nombreDeLaEscena));
    }

    private IEnumerator TransicionYCarga(string nombreDeLaEscena)
    {
        if (panelFade != null) panelFade.blocksRaycasts = true;

        float tiempo = 0f;
        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            if (panelFade != null) panelFade.alpha = tiempo / duracionFade;
            yield return null;
        }

        if (panelFade != null) panelFade.alpha = 1f;

        if (contenedorLoading != null)
        {
            contenedorLoading.SetActive(true);
        }

        AsyncOperation operacionCarga = SceneManager.LoadSceneAsync(nombreDeLaEscena);

        operacionCarga.allowSceneActivation = false;

        while (!operacionCarga.isDone)
        {
            float progresoReal = Mathf.Clamp01(operacionCarga.progress / 0.9f);

            if (barraProgreso != null) barraProgreso.value = progresoReal;
            if (textoPorcentaje != null) textoPorcentaje.text = Mathf.RoundToInt(progresoReal * 100f) + "%";

            if (operacionCarga.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f);
                operacionCarga.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private IEnumerator FadeDeEntrada()
    {
        float tiempo = 0f;
        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            if (panelFade != null) panelFade.alpha = 1f - (tiempo / duracionFade);
            yield return null;
        }

        if (panelFade != null)
        {
            panelFade.alpha = 0f;
            panelFade.blocksRaycasts = false;
        }
    }
}