using UnityEngine;

public class PlayerAnimationEventReceiver : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        // Como este script va en el modelo (hijo), buscamos al cerebro en el padre
        player = GetComponentInParent<Player>();
    }

    // Esta función la llamará la animación cuando empiece el impulso
    public void StartRollImpulse()
    {
        if (player != null)
        {
            player.SetRollImpulse(true);
        }
    }

    // Esta función la llamará la animación cuando termine el impulso
    public void StopRollImpulse()
    {
        if (player != null)
        {
            player.SetRollImpulse(false);
        }
    }
}