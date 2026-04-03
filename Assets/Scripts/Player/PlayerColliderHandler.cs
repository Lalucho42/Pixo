using UnityEngine;

public class PlayerColliderHandler
{
    private CharacterController controller;
    private float originalHeight;
    private Vector3 originalCenter;
    private float rollMultiplier;

    // Esta variable reemplaza al antiguo 'ApplyRollImpulse' del Player
    public bool IsRolling { get; private set; }

    public PlayerColliderHandler(CharacterController cc, float multiplier)
    {
        controller = cc;
        rollMultiplier = multiplier;

        // Guardamos los valores originales ni bien se crea el módulo
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
                // Achicamos la altura dinámicamente
                float newHeight = originalHeight * rollMultiplier;
                controller.height = newHeight;

                // Calculamos el centro para que los pies queden siempre al ras del suelo
                controller.center = new Vector3(
                    originalCenter.x,
                    newHeight / 2f,
                    originalCenter.z
                );
            }
            else
            {
                // Volvemos al estado original exacto
                controller.height = originalHeight;
                controller.center = originalCenter;
            }
        }
    }
}