using UnityEngine;
using Unity.Cinemachine;

public class SetCameraTarget : MonoBehaviour
{
    [SerializeField] private string playerName = "Player Variant";

    void Start()
    {
        var vcam = GetComponent<CinemachineCamera>();
        if (vcam == null) return;

        var player = GameObject.Find(playerName);
        if (player == null) return;

        var cameraTarget = player.transform.Find("CameraTarget");
        if (cameraTarget != null)
        {
            vcam.Follow = cameraTarget;
            vcam.LookAt = cameraTarget;
        }
        else
        {
            vcam.Follow = player.transform;
            vcam.LookAt = player.transform;
        }
    }
}
