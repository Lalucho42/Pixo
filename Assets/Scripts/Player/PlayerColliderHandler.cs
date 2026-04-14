using UnityEngine;

public class PlayerColliderHandler
{
    private CharacterController controller;
    private float originalHeight;
    private Vector3 originalCenter;
    private float rollMultiplier;

    public bool IsRolling { get; private set; }

    public PlayerColliderHandler(CharacterController cc, float multiplier)
    {
        controller = cc;
        rollMultiplier = multiplier;

        originalHeight = cc.height;
        originalCenter = cc.center;
    }

    public void SetRollState(bool active)
    {
        IsRolling = active;

        if (controller != null)
        {
            if (active)
            {
                float newHeight = originalHeight * rollMultiplier;
                controller.height = newHeight;

                controller.center = new Vector3(
                    originalCenter.x,
                    newHeight / 2f,
                    originalCenter.z
                );
            }
            else
            {
                controller.height = originalHeight;
                controller.center = originalCenter;
            }
        }
    }
}
