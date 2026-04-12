using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceHealthBar : MonoBehaviour
{
    [Header("Referencias")]
    public Slider slider;
    public HealthSystem healthSystem;
    public GameObject visualContainer; // Arrastrá aquí el objeto que querés ocultar (el Canvas o el fondo de la barra)

    private Transform mainCamera;
    private bool isVisible = false;

    private void Start()
    {
        mainCamera = Camera.main.transform;

        // 1. Ocultamos la barra al iniciar
        if (visualContainer != null)
            visualContainer.SetActive(false);

        if (healthSystem != null && slider != null)
        {
            slider.maxValue = healthSystem.maxHealth;
            slider.value = healthSystem.currentHealth;

            // Nos suscribimos al evento de daño
            healthSystem.onTakeDamage.AddListener(UpdateBar);
        }
    }

    private void UpdateBar()
    {
        if (slider != null && healthSystem != null)
        {
            slider.value = healthSystem.currentHealth;

            // 2. Si el enemigo recibe daño y la barra está oculta, la mostramos
            if (!isVisible)
            {
                isVisible = true;
                if (visualContainer != null)
                    visualContainer.SetActive(true);
            }

            // 3. Si el enemigo muere, volvemos a ocultar la barra
            if (healthSystem.IsDead)
            {
                if (visualContainer != null)
                    visualContainer.SetActive(false);
            }
        }
    }

    private void LateUpdate()
    {
        // Solo rotamos si la barra es visible para ahorrar recursos
        if (mainCamera == null || !isVisible) return;

        // Efecto Billboard: Siempre mirando a cámara
        transform.LookAt(transform.position + mainCamera.forward);
    }
}