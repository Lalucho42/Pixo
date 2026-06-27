using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class IgnoreAudioPause : MonoBehaviour
{
    void Awake()
    {
        
        GetComponent<AudioSource>().ignoreListenerPause = true;
    }
}