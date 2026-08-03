using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class StructureRepairUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform contenedorSlots;
    public GameObject panelPrincipal;

    [Header("Seguimiento 3D a Screen Space (Opcional)")]
    [Tooltip("Asigná la PC o estructura si querés que el cartel flotador la siga por la pantalla")]
    public Transform objetivoEnMundo;
    public Vector3 offsetMundo = new Vector3(0f, 1.8f, 0f);

    [Header("Animación")]
    public float animSpeed = 15f;

    private Camera mainCam;
    private Vector3 escalaOriginal;
    private Coroutine animCoroutine;

    private void Awake()
    {
        mainCam = Camera.main;

        if (panelPrincipal != null)
        {
            escalaOriginal = panelPrincipal.transform.localScale;
        }
    }

    private void LateUpdate()
    {
        if (objetivoEnMundo != null && mainCam != null && panelPrincipal != null && panelPrincipal.activeSelf)
        {
            Vector3 screenPos = mainCam.WorldToScreenPoint(objetivoEnMundo.position + offsetMundo);

            if (screenPos.z > 0)
            {
                transform.position = screenPos;
            }
        }
    }

    public void ConfigurarCartel(List<ResourceCost> costos)
    {
        if (contenedorSlots == null) return;

        foreach (Transform child in contenedorSlots)
        {
            Destroy(child.gameObject);
        }

        foreach (ResourceCost costo in costos)
        {
            GameObject nuevoSlot = Instantiate(slotPrefab, contenedorSlots);
            ResourceSlotUI slotScript = nuevoSlot.GetComponent<ResourceSlotUI>();
            if (slotScript != null)
            {
                slotScript.Configurar(costo.icono, costo.cantidad);
            }
        }

        StartCoroutine(RecalcularLayoutScreenSpace());
    }

    private IEnumerator RecalcularLayoutScreenSpace()
    {
        yield return new WaitForEndOfFrame();

        Canvas.ForceUpdateCanvases();

        if (contenedorSlots is RectTransform rectSlots)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectSlots);
        }

        if (panelPrincipal != null && panelPrincipal.transform is RectTransform rectPanel)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectPanel);
        }
    }

    public void Mostrar()
    {
        if (panelPrincipal == null || panelPrincipal.activeSelf) return;

        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimarAparicion());
    }

    public void Ocultar()
    {
        if (panelPrincipal == null || !panelPrincipal.activeSelf) return;

        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimarDesaparicion());
    }

    private IEnumerator AnimarAparicion()
    {
        panelPrincipal.transform.localScale = Vector3.zero;
        panelPrincipal.SetActive(true);

        while (Vector3.Distance(panelPrincipal.transform.localScale, escalaOriginal) > 0.01f)
        {
            panelPrincipal.transform.localScale = Vector3.Lerp(panelPrincipal.transform.localScale, escalaOriginal, Time.deltaTime * animSpeed);
            yield return null;
        }

        panelPrincipal.transform.localScale = escalaOriginal;
    }

    private IEnumerator AnimarDesaparicion()
    {
        while (Vector3.Distance(panelPrincipal.transform.localScale, Vector3.zero) > 0.01f)
        {
            panelPrincipal.transform.localScale = Vector3.Lerp(panelPrincipal.transform.localScale, Vector3.zero, Time.deltaTime * animSpeed);
            yield return null;
        }

        panelPrincipal.transform.localScale = Vector3.zero;
        panelPrincipal.SetActive(false);
    }
}