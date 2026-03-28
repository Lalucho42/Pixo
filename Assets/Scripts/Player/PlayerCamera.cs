using UnityEngine;

public class PlayerCamera
{
    private Player player;

    // Variables internas que guardarán los límites
    private float clampAngleMin;
    private float clampAngleMax;

    private float rotacionX = 0f;
    private float rotacionY = 0f;

    private Transform cameraFollowTarget;

    // --- ACTUALIZACIÓN DEL CONSTRUCTOR ---
    // Ahora pedimos minAngle y maxAngle al crearlo
    public PlayerCamera(Player playerBrain, Transform followTarget, float minAngle, float maxAngle)
    {
        player = playerBrain;
        cameraFollowTarget = followTarget;

        // Guardamos los límites
        clampAngleMin = minAngle;
        clampAngleMax = maxAngle;

        Vector3 rotacionActual = cameraFollowTarget.localRotation.eulerAngles;
        // Normalizamos los ángulos si son raros
        rotacionX = rotacionActual.x > 180 ? rotacionActual.x - 360 : rotacionActual.x;
        rotacionY = rotacionActual.y;
    }

    public void Tick(float deltaTime)
    {
        Vector2 mouseInput = player.InputHandler.LookInput;
        float sensibilidad = player.cameraSensitivity;

        // --- CORRECCIÓN ACÁ ---
        // Le sacamos el deltaTime y el multiplicador * 50f. 
        // El movimiento del mouse se aplica directo con tu sensibilidad.
        rotacionY = rotacionY + (mouseInput.x * sensibilidad);
        rotacionX = rotacionX - (mouseInput.y * sensibilidad);

        rotacionX = Mathf.Clamp(rotacionX, clampAngleMin, clampAngleMax);

        cameraFollowTarget.rotation = Quaternion.Euler(rotacionX, rotacionY, 0f);
    }
}