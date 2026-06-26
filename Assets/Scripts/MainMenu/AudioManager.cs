using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (escena.name == "MenuPrincipal")
        {
            PlayMusic("Musica_Menu");
        }
        else if (escena.name == "Boceto" || escena.name == "Juego")
        {
            PlayMusic("Ambiente_Bosque");
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

    public void StopMusic()
    {
        if (fuenteMusica != null)
        {
            fuenteMusica.Stop();
        }
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
    }

    public void PlaySFX3D(string nombre, AudioSource fuenteEmisora)
    {
        if (fuenteEmisora == null) return;

        Sound s = Array.Find(sfxClips, sound => sound.nombre == nombre);
        if (s == null || s.clips.Length == 0) return;

        AudioClip clipElegido = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];
        fuenteEmisora.pitch = s.usarPitchAleatorio ? UnityEngine.Random.Range(0.88f, 1.12f) : s.pitch;
        fuenteEmisora.PlayOneShot(clipElegido, s.volumen);
    }

    public void CambiarVolumenMaster(float valorSlider) { mainMixer.SetFloat("MasterVol", Mathf.Log10(valorSlider) * 20); }
    public void CambiarVolumenMusica(float valorSlider) { mainMixer.SetFloat("MusicVol", Mathf.Log10(valorSlider) * 20); }
    public void CambiarVolumenSFX(float valorSlider) { mainMixer.SetFloat("SFXVol", Mathf.Log10(valorSlider) * 20); }
    public void CambiarVolumenUI(float valorSlider) { mainMixer.SetFloat("UIVol", Mathf.Log10(valorSlider) * 20); }
}