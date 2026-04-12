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
            // Configuramos los valores iniciales de la barra
            slider.maxValue = playerHealth.maxHealth;
            slider.value = playerHealth.currentHealth;

            // --- OPTIMIZACIÓN PRO ---
            // Nos suscribimos al evento 'onTakeDamage' del HealthSystem.
            // Así la barra solo se actualiza cuando recibís un golpe, ahorrando recursos.
            playerHealth.onTakeDamage.AddListener(ActualizarBarra);
        }
    }

    // Esta función se llama sola gracias al evento de arriba
    public void ActualizarBarra()
    {
        if (slider != null && playerHealth != null)
        {
            slider.value = playerHealth.currentHealth;
        }
    }

    // También actualizamos en el Update por si hay curaciones
    private void Update()
    {
        if (slider != null && playerHealth != null)
        {
            // Usamos un Lerp suave para que la barra no "salte", sino que baje fluido
            slider.value = Mathf.Lerp(slider.value, playerHealth.currentHealth, Time.deltaTime * 5f);
        }
    }
}