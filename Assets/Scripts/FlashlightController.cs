using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light[] lights;
    public GameObject[] extraObjects;
    private bool isLightOn = false;
    private Player player;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        if (player != null && player.InputHandler != null)
        {
            player.InputHandler.OnFlashlightEvent += ToggleFlashlight;
        }

        if (lights == null || lights.Length == 0)
        {
            lights = GetComponentsInChildren<Light>(true);
        }
        
        UpdateLights();
    }

    private void OnDestroy()
    {
        if (player != null && player.InputHandler != null)
        {
            player.InputHandler.OnFlashlightEvent -= ToggleFlashlight;
        }
    }

    private void ToggleFlashlight()
    {
        if (GameManager.IsPaused || GameManager.IsDead) return;

        isLightOn = !isLightOn;
        UpdateLights();
        
        if (AudioManager.instance != null && AudioManager.instance.buttonClick != null) 
            AudioManager.instance.PlaySFX(AudioManager.instance.buttonClick);
    }

    private void UpdateLights()
    {
        if (lights != null)
        {
            foreach (var l in lights)
            {
                if (l != null) l.enabled = isLightOn;
            }
        }

        if (extraObjects != null)
        {
            foreach (var obj in extraObjects)
            {
                if (obj != null) obj.SetActive(isLightOn);
            }
        }
    }
}
