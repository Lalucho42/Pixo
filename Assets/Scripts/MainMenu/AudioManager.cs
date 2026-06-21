using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string nombre;
        public AudioClip[] clips;
        [Range(0f, 1f)] public float volumen = 1f;
        [Range(0.5f, 1.5f)] public float pitch = 1f;
        public bool usarPitchAleatorio = false;
    }

    public static AudioManager Instance;

    [Header("Referencia al Mixer")]
    public AudioMixer mainMixer;

    [Header("Ruteo a Grupos del Mixer")]
    public AudioMixerGroup grupoMusica;
    public AudioMixerGroup grupoSFX;
    public AudioMixerGroup grupoUI;

    [Header("Base de Datos de Sonidos")]
    public Sound[] musicaTracks;
    public Sound[] sfxClips;

    private AudioSource fuenteMusica;
    private AudioSource fuenteSFX2D;
    private AudioSource fuenteUI2D;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ConfigurarFuentesInternas();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ConfigurarFuentesInternas()
    {
        fuenteMusica = gameObject.AddComponent<AudioSource>();
        fuenteSFX2D = gameObject.AddComponent<AudioSource>();
        fuenteUI2D = gameObject.AddComponent<AudioSource>();

        if (grupoMusica != null) fuenteMusica.outputAudioMixerGroup = grupoMusica;
        if (grupoSFX != null) fuenteSFX2D.outputAudioMixerGroup = grupoSFX;
        if (grupoUI != null) fuenteUI2D.outputAudioMixerGroup = grupoUI;
    }

    // ========================================================
    // 🎛️ CANALES GLOBALES 2D (Música, Menús e Interfaz)
    // ========================================================

    public void PlayMusic(string nombre)
    {
        Sound s = Array.Find(musicaTracks, sound => sound.nombre == nombre);
        if (s == null || s.clips.Length == 0) return;

        if (fuenteMusica.clip == s.clips[0] && fuenteMusica.isPlaying) return;

        fuenteMusica.clip = s.clips[0];
        fuenteMusica.volume = s.volumen;
        fuenteMusica.pitch = s.pitch;
        fuenteMusica.loop = true;
        fuenteMusica.Play();
    }

    public void PlayUI(string nombre)
    {
        Sound s = Array.Find(sfxClips, sound => sound.nombre == nombre);
        if (s == null || s.clips.Length == 0) return;

        AudioClip clipAleatorio = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];
        fuenteUI2D.pitch = s.pitch;
        fuenteUI2D.PlayOneShot(clipAleatorio, s.volumen);
    }

    public void PlaySFX2D(string nombre)
    {
        Sound s = Array.Find(sfxClips, sound => sound.nombre == nombre);
        if (s == null || s.clips.Length == 0) return;

        AudioClip clipAleatorio = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];
        fuenteSFX2D.pitch = s.usarPitchAleatorio ? UnityEngine.Random.Range(0.9f, 1.1f) : s.pitch;
        fuenteSFX2D.PlayOneShot(clipAleatorio, s.volumen);

        Debug.Log($"<color=cyan><b>[UML 2D BYPASS]</b></color> Sonó: {nombre}");
    }

    // ========================================================
    // 🔊 MÉTODO MAESTRO 3D (Responsabilidad Única)
    // ========================================================
    /// <summary>
    /// Recibe un AudioSource del mundo físico (Player, Enemigo, etc.) y le inyecta la lógica de reproducción centralizada.
    /// </summary>
    public void PlaySFX3D(string nombre, AudioSource fuenteEmisora)
    {
        if (fuenteEmisora == null) return;

        Sound s = Array.Find(sfxClips, sound => sound.nombre == nombre);
        if (s == null || s.clips.Length == 0) return;

        // El Manager calcula el clip aleatorio y el pitch según su base de datos interna
        AudioClip clipElegido = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];
        fuenteEmisora.pitch = s.usarPitchAleatorio ? UnityEngine.Random.Range(0.88f, 1.12f) : s.pitch;

        // El Manager ejecuta la reproducción sobre el parlante corporal del objeto
        fuenteEmisora.PlayOneShot(clipElegido, s.volumen);

        Debug.Log($"<color=red><b>[UML 3D ESPACIAL]</b></color> Entidad: {fuenteEmisora.gameObject.name} -> Sonido: {nombre} ({clipElegido.name})");
    }

    // Controles del menú del Mixer
    public void CambiarVolumenMaster(float valorSlider) { mainMixer.SetFloat("MasterVol", Mathf.Log10(valorSlider) * 20); }
    public void CambiarVolumenMusica(float valorSlider) { mainMixer.SetFloat("MusicVol", Mathf.Log10(valorSlider) * 20); }
    public void CambiarVolumenSFX(float valorSlider) { mainMixer.SetFloat("SFXVol", Mathf.Log10(valorSlider) * 20); }
    public void CambiarVolumenUI(float valorSlider) { mainMixer.SetFloat("UIVol", Mathf.Log10(valorSlider) * 20); }
}