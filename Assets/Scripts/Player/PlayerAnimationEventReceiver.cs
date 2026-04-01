using UnityEngine;

public class PlayerAnimationEventReceiver : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public void StartRollImpulse()
    {
        if (player != null)
        {
            player.SetRollImpulse(true);
        }
    }

    public void StopRollImpulse()
    {
        if (player != null)
        {
            player.SetRollImpulse(false);
        }
    }
}
