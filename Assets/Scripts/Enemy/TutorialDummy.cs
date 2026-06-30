using UnityEngine;
using System.Collections;

public class TutorialDummy : MonoBehaviour
{
    [Header("Configuración del Temblor (Feedback)")]
    [Tooltip("Arrastrá acá el modelo 3D del robot (el objeto hijo).")]
    public Transform mallaVisual;
    public float duracionTemblor = 0.15f;
    public float intensidadTemblor = 0.08f;

    [Header("Efectos Visuales (Opcional)")]
    [Tooltip("Arrastrá un sistema de partículas de chispas o musgo si tenés.")]
    public ParticleSystem particulasImpacto;

    private Vector3 posicionOriginalMalla;
    private bool estaTemblando = false;

    private void Start()
    {
        if (mallaVisual == null && transform.childCount > 0)
        {
            mallaVisual = transform.GetChild(0);
        }

        if (mallaVisual != null)
        {
            posicionOriginalMalla = mallaVisual.localPosition;
        }
    }

    
    public void TakeDamage(int damageAmount)
    {
        RecibirGolpe();
    }

   
    public void RecibirGolpe()
    {
        if (mallaVisual == null) return;

        if (particulasImpacto != null)
        {
            particulasImpacto.Play();
        }

        if (estaTemblando)
        {
            StopAllCoroutines();
            mallaVisual.localPosition = posicionOriginalMalla;
        }

        StartCoroutine(CorrutinaTemblor());
    }

    private IEnumerator CorrutinaTemblor()
    {
        estaTemblando = true;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracionTemblor)
        {
           
            float offsetX = Random.Range(-1f, 1f) * intensidadTemblor;
            float offsetY = Random.Range(-1f, 1f) * intensidadTemblor;

            mallaVisual.localPosition = new Vector3(
                posicionOriginalMalla.x + offsetX,
                posicionOriginalMalla.y + offsetY,
                posicionOriginalMalla.z
            );

            tiempoTranscurrido += Time.deltaTime;
            yield return null; 
        }

        mallaVisual.localPosition = posicionOriginalMalla;
        estaTemblando = false;
    }
}