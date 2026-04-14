using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public Slider slider;
    public HealthSystem playerHealth;

    private void Start()
    {
        if (playerHealth != null && slider != null)
        {
            slider.maxValue = playerHealth.maxHealth;
            slider.value = playerHealth.currentHealth;

            playerHealth.onTakeDamage.AddListener(ActualizarBarra);
        }
    }

    public void ActualizarBarra()
    {
        if (slider != null && playerHealth != null)
        {
            slider.value = playerHealth.currentHealth;
        }
    }

    private void Update()
    {
        if (slider != null && playerHealth != null)
        {
            slider.value = Mathf.Lerp(slider.value, playerHealth.currentHealth, Time.deltaTime * 5f);
        }
    }
}