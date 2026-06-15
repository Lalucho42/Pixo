using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorDeEscenas : MonoBehaviour
{
    [Header("Configuración de Transición")]
    public CanvasGroup panelFade;
    public float duracionFade = 1f;

    private void Start()
    {
        
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
        
        panelFade.blocksRaycasts = true;

        
        float tiempo = 0f;
        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            panelFade.alpha = tiempo / duracionFade; 
            yield return null; 
        }

        panelFade.alpha = 1f; 

        
        AsyncOperation operacionCarga = SceneManager.LoadSceneAsync(nombreDeLaEscena);

        
        operacionCarga.allowSceneActivation = true;

        while (!operacionCarga.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator FadeDeEntrada()
    {
        float tiempo = 0f;
        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            panelFade.alpha = 1f - (tiempo / duracionFade); 
            yield return null;
        }

        panelFade.alpha = 0f; 
        panelFade.blocksRaycasts = false; 
    }
}