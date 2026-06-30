using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AudioSliderLink : MonoBehaviour
{
    public enum TipoVolumen { Master, Musica, SFX, UI }
    public TipoVolumen tipo;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (slider == null || AudioManager.Instance == null) return;

        slider.onValueChanged.RemoveAllListeners();

        switch (tipo)
        {
            case TipoVolumen.Master:
                slider.onValueChanged.AddListener(ValorCambiadoMaster);
                slider.value = AudioManager.Instance.volumenMaster;
                break;
            case TipoVolumen.Musica:
                slider.onValueChanged.AddListener(ValorCambiadoMusica);
                slider.value = AudioManager.Instance.volumenMusica;
                break;
            case TipoVolumen.SFX:
                slider.onValueChanged.AddListener(ValorCambiadoSFX);
                slider.value = AudioManager.Instance.volumenSFX;
                break;
            case TipoVolumen.UI:
                slider.onValueChanged.AddListener(ValorCambiadoUI);
                slider.value = AudioManager.Instance.volumenUI;
                break;
        }
    }

    private void ValorCambiadoMaster(float v)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.CambiarVolumenMaster(v);
    }

    private void ValorCambiadoMusica(float v)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.CambiarVolumenMusica(v);
    }

    private void ValorCambiadoSFX(float v)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.CambiarVolumenSFX(v);
    }

    private void ValorCambiadoUI(float v)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.CambiarVolumenUI(v);
    }
}