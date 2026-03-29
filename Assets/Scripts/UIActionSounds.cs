using UnityEngine;

public class UIActionSounds : MonoBehaviour
{
    public void PlayClickSound()
    {
        if (AudioManager.instance != null && AudioManager.instance.buttonClick != null) AudioManager.instance.PlaySFX(AudioManager.instance.buttonClick);
    }
}
