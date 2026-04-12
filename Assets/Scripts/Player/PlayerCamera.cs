using UnityEngine;

public class PlayerCamera
{
    private Player player;
    private Transform target;
    private float sens, min, max;
    private float yaw, pitch;

    public Vector3 CameraForward { get { return Camera.main.transform.forward; } }
    public Vector3 CameraRight { get { return Camera.main.transform.right; } }

    public PlayerCamera(Player brain, Transform t, float s, float mi, float ma)
    {
        player = brain; target = t; sens = s; min = mi; max = ma;
    }

    public void Tick(float dt)
    {
        Vector2 look = player.InputHandler.LookInput;
        yaw += look.x * sens;
        pitch -= look.y * sens;
        pitch = Mathf.Clamp(pitch, min, max);
        target.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}