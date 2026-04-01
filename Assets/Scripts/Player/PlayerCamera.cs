using UnityEngine;

public class PlayerCamera
{
    private Player player;
    private float clampAngleMin;
    private float clampAngleMax;
    private float rotacionX = 0f;
    private float rotacionY = 0f;
    private Transform cameraFollowTarget;

    public PlayerCamera(Player playerBrain, Transform followTarget, float minAngle, float maxAngle)
    {
        player = playerBrain;
        cameraFollowTarget = followTarget;
        clampAngleMin = minAngle;
        clampAngleMax = maxAngle;

        Vector3 rotacionActual = cameraFollowTarget.localRotation.eulerAngles;
        rotacionX = rotacionActual.x > 180 ? rotacionActual.x - 360 : rotacionActual.x;
        rotacionY = rotacionActual.y;
    }

    public void Tick(float deltaTime)
    {
        if (GameManager.IsPaused || GameManager.IsDead) return;

        Vector2 mouseInput = player.InputHandler.LookInput;
        float sensibilidad = player.cameraSensitivity;

        rotacionY = rotacionY + (mouseInput.x * sensibilidad);
        rotacionX = rotacionX - (mouseInput.y * sensibilidad);
        rotacionX = Mathf.Clamp(rotacionX, clampAngleMin, clampAngleMax);

        cameraFollowTarget.rotation = Quaternion.Euler(rotacionX, rotacionY, 0f);
    }
}
