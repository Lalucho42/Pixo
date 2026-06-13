using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Referencia al Mixer")]
    public AudioMixer mainMixer;

    // Estas funciones se conectan con los Sliders del menú.

    public void CambiarVolumenMaster(float valorSlider)
    {
        // Multiplica por 20 para convertir el valor logarítmico a escala de decibelios
        mainMixer.SetFloat("MasterVol", Mathf.Log10(valorSlider) * 20);
    }

    public void CambiarVolumenMusica(float valorSlider)
    {
        mainMixer.SetFloat("MusicVol", Mathf.Log10(valorSlider) * 20);
    }

    public void CambiarVolumenSFX(float valorSlider)
    {
        mainMixer.SetFloat("SFXVol", Mathf.Log10(valorSlider) * 20);
    }

    public void CambiarVolumenUI(float valorSlider)
    {
        mainMixer.SetFloat("UIVol", Mathf.Log10(valorSlider) * 20);
    }
}