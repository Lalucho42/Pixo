using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceHealthBar : MonoBehaviour
{
    [Header("Referencias")]
    public Slider slider;
    public HealthSystem healthSystem;
    public GameObject visualContainer;

    private Transform mainCamera;
    private bool isVisible = false;

    private void Start()
    {
        mainCamera = Camera.main.transform;

        if (visualContainer != null)
            visualContainer.SetActive(false);

        if (healthSystem != null && slider != null)
        {
            slider.maxValue = healthSystem.maxHealth;
            slider.value = healthSystem.currentHealth;

            healthSystem.onTakeDamage.AddListener(UpdateBar);
        }
    }

    private void UpdateBar()
    {
        if (slider != null && healthSystem != null)
        {
            slider.value = healthSystem.currentHealth;

            if (!isVisible)
            {
                isVisible = true;
                if (visualContainer != null)
                    visualContainer.SetActive(true);
            }

            if (healthSystem.IsDead)
            {
                if (visualContainer != null)
                    visualContainer.SetActive(false);
            }
        }
    }

    private void LateUpdate()
    {
        if (mainCamera == null || !isVisible) return;

        transform.LookAt(transform.position + mainCamera.forward);
    }
}