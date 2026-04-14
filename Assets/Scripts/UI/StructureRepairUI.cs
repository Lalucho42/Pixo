using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureRepairUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform contenedorSlots;
    public GameObject panelPrincipal;

    [Header("Animacion")]
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
        if (panelPrincipal.activeSelf && mainCam != null)
        {
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