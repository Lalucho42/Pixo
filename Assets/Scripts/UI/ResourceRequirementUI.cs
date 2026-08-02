using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceRequirementUI : MonoBehaviour
{
    [Header("Referencias de UI")]
    public RectTransform mainPanel;
    public Transform slotsContainer;
    public GameObject slotPrefab;
    public TextMeshProUGUI actionText;
    public CanvasGroup canvasGroup;

    [Header("Tamaño del Panel")]
    [Tooltip("Escala general del cartel (0.5 o 0.6 para que sea un HUD compacto)")]
    public float escalaDelPanel = 0.5f;

    [Header("Ajustes de Animación")]
    public float animSpeed = 15f;

    [Header("Seguimiento 3D en Pantalla (Opcional)")]
    public Vector3 offsetMundo = new Vector3(0f, 1.8f, 0f);

    private Camera mainCam;
    private Transform target3D;
    private Coroutine activeRoutine;

    private void Awake()
    {
        mainCam = Camera.main;
        if (canvasGroup == null && mainPanel != null)
        {
            canvasGroup = mainPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = mainPanel.gameObject.AddComponent<CanvasGroup>();
        }
        OcultarInmediato();
    }

    private void LateUpdate()
    {
        // Seguir la posición 3D del objeto en pantalla
        if (target3D != null && mainPanel != null && mainPanel.gameObject.activeSelf && mainCam != null)
        {
            Vector3 screenPos = mainCam.WorldToScreenPoint(target3D.position + offsetMundo);
            if (screenPos.z > 0)
            {
                transform.position = screenPos;
            }
        }
    }

    public void ConfigurarYMostrar(List<ResourceCost> costos, Transform objetivo3D, string textoAccion = "Press [E] to Repair", Vector3? customOffset = null)
    {
        target3D = objetivo3D;
        if (customOffset.HasValue) offsetMundo = customOffset.Value;

        if (actionText != null)
        {
            actionText.text = textoAccion;
        }

        // 1. Limpiar slots anteriores
        foreach (Transform child in slotsContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. Instanciar slots
        foreach (ResourceCost costo in costos)
        {
            GameObject nuevoSlot = Instantiate(slotPrefab, slotsContainer);
            ResourceSlotUI slotScript = nuevoSlot.GetComponent<ResourceSlotUI>();

            if (slotScript != null)
            {
                // Evita que la imagen/ícono estire e infle el panel gigante
                if (slotScript.icono != null)
                {
                    slotScript.icono.preserveAspect = true;
                }

                slotScript.Configurar(costo.icono, costo.cantidad);
            }
        }

        // 3. Recalcular espacio
        StartCoroutine(RecalcularLayout());

        // 4. Mostrar
        Mostrar();
    }

    private IEnumerator RecalcularLayout()
    {
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();

        if (slotsContainer is RectTransform rectSlots) LayoutRebuilder.ForceRebuildLayoutImmediate(rectSlots);
        if (mainPanel != null) LayoutRebuilder.ForceRebuildLayoutImmediate(mainPanel);
    }

    public void Mostrar()
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(FadeRoutine(1f, Vector3.one * escalaDelPanel));
    }

    public void Ocultar()
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(FadeRoutine(0f, Vector3.one * (escalaDelPanel * 0.8f), true));
    }

    private void OcultarInmediato()
    {
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        if (mainPanel != null)
        {
            mainPanel.localScale = Vector3.one * (escalaDelPanel * 0.8f);
            mainPanel.gameObject.SetActive(false);
        }
    }

    private IEnumerator FadeRoutine(float targetAlpha, Vector3 targetScale, bool desactivarAlFinal = false)
    {
        if (mainPanel != null) mainPanel.gameObject.SetActive(true);

        float startAlpha = canvasGroup != null ? canvasGroup.alpha : 0f;
        Vector3 startScale = mainPanel != null ? mainPanel.localScale : Vector3.zero;
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * animSpeed;
            if (canvasGroup != null) canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time);
            if (mainPanel != null) mainPanel.localScale = Vector3.Lerp(startScale, targetScale, time);
            yield return null;
        }

        if (canvasGroup != null) canvasGroup.alpha = targetAlpha;
        if (mainPanel != null) mainPanel.localScale = targetScale;

        if (desactivarAlFinal && mainPanel != null)
        {
            mainPanel.gameObject.SetActive(false);
        }
    }
}