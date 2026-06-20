using UnityEngine;
using UnityEngine.Audio; // Necesario para el Mixer
using System;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string nombre;
        public AudioClip clip;
        [Range(0f, 1f)] public float volumen = 1f;
        [Range(0.5f, 1.5f)] public float pitch = 1f;
        public bool usarPitchAleatorio = false; // Da variedad a pasos/golpes
    }

    public static AudioManager Instance;

    [Header("Referencia al Mixer")]
    public AudioMixer mainMixer; // Tu referencia original del Mixer

    [Header("Ruteo a Grupos del Mixer")]
    public AudioMixerGroup grupoMusica;
    public AudioMixerGroup grupoSFX;
    public AudioMixerGroup grupoUI; // Nuevo: Para conectar tus sonidos de interfaz

    [Header("Base de Datos de Sonidos")]
    public Sound[] musicaTracks;
    public Sound[] sfxClips;

    private AudioSource fuenteMusica;
    private AudioSource fuenteSFX2D;
    private AudioSource fuenteUI2D;

    private void Awake()
    {
        // --- PATRÓN SINGLETON ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // No se destruye al cambiar de escena
            ConfigurarFuentesInternas();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ConfigurarFuentesInternas()
    {
        // Creamos los altavoces internos globales
        fuenteMusica = gameObject.AddComponent<AudioSource>();
        fuenteSFX2D = gameObject.AddComponent<AudioSource>();
        fuenteUI2D = gameObject.AddComponent<AudioSource>();

        // Los ruteamos de forma inteligente a los canales del Mixer que mostrás en tu captura
        if (grupoMusica != null) fuenteMusica.outputAudioMixerGroup = grupoMusica;
        if (grupoSFX != null) fuenteSFX2D.outputAudioMixerGroup = grupoSFX;
        if (grupoUI != null) fuenteUI2D.outputAudioMixerGroup = grupoUI;
    }

    // ==========================================
    // REPRODUCCIÓN DE AUDIO
    // ==========================================

    public void PlayMusic(string nombre)
    {
        Sound s = Array.Find(musicaTracks, sound => sound.nombre == nombre);
        if (s == null) return;

        if (fuenteMusica.clip == s.clip && fuenteMusica.isPlaying) return;

        fuenteMusica.clip = s.clip;
        fuenteMusica.volume = s.volumen;
        fuenteMusica.pitch = s.pitch;
        fuenteMusica.loop = true;
        fuenteMusica.Play();
    }

    public void PlaySFX2D(string nombre)
    {
        Sound s = Array.Find(sfxClips, sound => sound.nombre == nombre);
        if (s == null) return;

        fuenteSFX2D.pitch = s.usarPitchAleatorio ? UnityEngine.Random.Range(0.9f, 1.1f) : s.pitch;
        fuenteSFX2D.PlayOneShot(s.clip, s.volumen);
    }

    // Nuevo método: Reproduce sonidos que se verán afectados por el Slider de la UI
    public void PlayUI(string nombre)
    {
        Sound s = Array.Find(sfxClips, sound => sound.nombre == nombre);
        if (s == null) return;

        fuenteUI2D.pitch = s.pitch;
        fuenteUI2D.PlayOneShot(s.clip, s.volumen);
    }

    // Auxiliar para que los enemigos/gato obtengan sonidos 3D físicos en el mapa
    public AudioClip GetClip(string nombre, out float vol, out float pit, out bool aleatorio)
    {
        Sound s = Array.Find(sfxClips, sound => sound.nombre == nombre);
        if (s != null)
        {
            vol = s.volumen;
            pit = s.pitch;
            aleatorio = s.usarPitchAleatorio;
            return s.clip;
        }
        vol = 1f; pit = 1f; aleatorio = false;
        return null;
    }

    // ==========================================
    // TUS FUNCIONES ORIGINALES DEL MENÚ (CONSERVADAS)
    // ==========================================

    public void CambiarVolumenMaster(float valorSlider)
    {
        mainMixer.SetFloat("MasterVol", Mathf.Log10(valorSlider) * 20); //
    }

    public void CambiarVolumenMusica(float valorSlider)
    {
        mainMixer.SetFloat("MusicVol", Mathf.Log10(valorSlider) * 20); //
    }

    public void CambiarVolumenSFX(float valorSlider)
    {
        mainMixer.SetFloat("SFXVol", Mathf.Log10(valorSlider) * 20); //
    }

    public void CambiarVolumenUI(float valorSlider)
    {
        mainMixer.SetFloat("UIVol", Mathf.Log10(valorSlider) * 20); //
    }
}