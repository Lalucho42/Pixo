using System.Collections; // Necesario para las Corrutinas (animaciones por código)
using System.Collections.Generic;
using UnityEngine;

public class StructureRepairUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform contenedorSlots;
    public GameObject panelPrincipal;

    [Header("Animación")]
    public float animSpeed = 15f; // Qué tan rápido hace el efecto resorte

    private Camera mainCam;
    private Vector3 escalaOriginal;
    private Coroutine animCoroutine;

    private void Awake()
    {
        mainCam = Camera.main; // Guardamos la cámara para el Billboard

        if (panelPrincipal != null)
        {
            // Guardamos el tamaño normal del cartel para saber hasta dónde inflarlo
            escalaOriginal = panelPrincipal.transform.localScale;
        }
    }

    private void LateUpdate()
    {
        // EFECTO BILLBOARD: Hacemos que todo el Canvas mire a la cámara siempre.
        // Se usa LateUpdate para asegurarnos de que la cámara ya se movió este frame.
        if (panelPrincipal.activeSelf && mainCam != null)
        {
            // Truco: Igualamos la dirección hacia adelante del Canvas con la de la cámara.
            // Si usamos LookAt, el texto se lee al revés. Así queda perfecto.
            transform.forward = mainCam.transform.forward;
        }
    }

    public void ConfigurarCartel(List<ResourceCost> costos)
    {
        foreach (Transform child in contenedorSlots) Destroy(child.gameObject);

        foreach (ResourceCost costo in costos)
        {
            GameObject nuevoSlot = Instantiate(slotPrefab, contenedorSlots);
            ResourceSlotUI slotScript = nuevoSlot.GetComponent<ResourceSlotUI>();
            if (slotScript != null) slotScript.Configurar(costo.icono, costo.cantidad);
        }
    }

    public void Mostrar()
    {
        if (panelPrincipal == null || panelPrincipal.activeSelf) return;

        // Si había otra animación corriendo (ej: se estaba cerrando y volviste a entrar), la frenamos
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimarAparicion());
    }

    public void Ocultar()
    {
        if (panelPrincipal == null || !panelPrincipal.activeSelf) return;

        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimarDesaparicion());
    }

    // --- ANIMACIONES POR CÓDIGO (Suaves y sin usar Animator) ---

    private IEnumerator AnimarAparicion()
    {
        // Empezamos con tamaño 0 (invisible)
        panelPrincipal.transform.localScale = Vector3.zero;
        panelPrincipal.SetActive(true);

        // Inflamos como un globo hasta llegar a la escala original
        while (Vector3.Distance(panelPrincipal.transform.localScale, escalaOriginal) > 0.01f)
        {
            panelPrincipal.transform.localScale = Vector3.Lerp(panelPrincipal.transform.localScale, escalaOriginal, Time.deltaTime * animSpeed);
            yield return null; // Esperamos al siguiente frame
        }

        // Aseguramos que termine en el tamaño exacto
        panelPrincipal.transform.localScale = escalaOriginal;
    }

    private IEnumerator AnimarDesaparicion()
    {
        // Desinflamos hasta llegar a 0
        while (Vector3.Distance(panelPrincipal.transform.localScale, Vector3.zero) > 0.01f)
        {
            panelPrincipal.transform.localScale = Vector3.Lerp(panelPrincipal.transform.localScale, Vector3.zero, Time.deltaTime * animSpeed);
            yield return null;
        }

        panelPrincipal.transform.localScale = Vector3.zero;
        panelPrincipal.SetActive(false); // Recién acá lo apagamos de verdad
    }
}