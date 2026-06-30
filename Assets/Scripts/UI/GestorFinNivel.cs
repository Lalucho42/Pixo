using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorFinNivel : MonoBehaviour
{
    public CanvasGroup canvasGroupFinNivel;
    public float duracionFade = 1.5f;
    public string escenaMenu = "MainMenu";

    private bool yaTermino = false;

    private void Start()
    {
        if (canvasGroupFinNivel != null)
        {
            canvasGroupFinNivel.alpha = 0f;
            canvasGroupFinNivel.interactable = false;
            canvasGroupFinNivel.blocksRaycasts = false;
        }
    }

    public void ActivarFinNivel()
    {
        if (yaTermino) return;
        yaTermino = true;

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(FadeFinNivel());
    }

    private IEnumerator FadeFinNivel()
    {
        if (canvasGroupFinNivel != null)
        {
            canvasGroupFinNivel.blocksRaycasts = true;
        }

        float tiempo = 0f;
        while (tiempo < duracionFade)
        {
            tiempo += Time.unscaledDeltaTime;
            if (canvasGroupFinNivel != null)
            {
                canvasGroupFinNivel.alpha = tiempo / duracionFade;
            }
            yield return null;
        }

        if (canvasGroupFinNivel != null)
        {
            canvasGroupFinNivel.alpha = 1f;
            canvasGroupFinNivel.interactable = true;
        }
    }

    public void BotonVolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaMenu);
    }

    public void BotonSalirDelJuego()
    {
        Application.Quit();
    }
}