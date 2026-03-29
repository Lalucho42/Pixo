using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambienceSource;
    
    public AudioClip bgMusic;
    public AudioClip ambienceClip;
    public AudioClip buttonClick;
    public AudioClip enemyHit;
    public AudioClip doorOpen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (bgMusic != null && musicSource != null)
        {
            musicSource.clip = bgMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        if (ambienceClip != null && ambienceSource != null)
        {
            ambienceSource.clip = ambienceClip;
            ambienceSource.loop = true;
            ambienceSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayAmbience(AudioClip clip)
    {
        if (clip != null && ambienceSource != null)
        {
            ambienceSource.Stop();
            ambienceSource.clip = clip;
            ambienceSource.Play();
        }
    }
}
