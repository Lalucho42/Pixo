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
        // Auto-detección: Si te olvidás de arrastrar la malla, busca al primer hijo
        if (mallaVisual == null && transform.childCount > 0)
        {
            mallaVisual = transform.GetChild(0);
        }

        if (mallaVisual != null)
        {
            posicionOriginalMalla = mallaVisual.localPosition;
        }
    }

    // --- INTERFAZ UNIVERSAL DE DAÑO ---
    // Tu espada (ToolItem), al activarse por los eventos del Player, va a buscar un método 
    // llamado 'TakeDamage' en lo que golpee. Este método captura ese llamado.
    public void TakeDamage(int damageAmount)
    {
        RecibirGolpe();
    }

    // --- MÉTODO DE EJECUCIÓN ---
    public void RecibirGolpe()
    {
        if (mallaVisual == null) return;

        // Si tenés partículas asignadas, las reproduce en el lugar del impacto
        if (particulasImpacto != null)
        {
            particulasImpacto.Play();
        }

        // Si el jugador le pega muy rápido (combo), reiniciamos el temblor para que no se desfase
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
            // Generamos una vibración rápida en los ejes X e Y
            float offsetX = Random.Range(-1f, 1f) * intensidadTemblor;
            float offsetY = Random.Range(-1f, 1f) * intensidadTemblor;

            mallaVisual.localPosition = new Vector3(
                posicionOriginalMalla.x + offsetX,
                posicionOriginalMalla.y + offsetY,
                posicionOriginalMalla.z
            );

            tiempoTranscurrido += Time.deltaTime;
            yield return null; // Espera al siguiente frame
        }

        // Al finalizar, devolvemos la malla a su posición original exacta
        mallaVisual.localPosition = posicionOriginalMalla;
        estaTemblando = false;
    }
}