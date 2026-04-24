using UnityEngine;
using Unity.Cinemachine;

public class SetCameraTarget : MonoBehaviour
{
    [Header("Rendimiento: Arrastra el PlayerTarget aqui directamente")]
    [SerializeField] private Transform manualTarget;
    [SerializeField] private string playerName = "Player Variant";

    void Start()
    {
        var vcam = GetComponent<CinemachineCamera>();
        if (vcam == null) return;

        // Si no asignaste nada manual, buscamos una vez y listo
        if (manualTarget == null)
        {
            var player = GameObject.Find(playerName);
            if (player != null) manualTarget = player.transform.Find("CameraTarget");

            // Si aun asi no lo encuentra, usamos el transform del player
            if (manualTarget == null && player != null) manualTarget = player.transform;
        }

        if (manualTarget != null)
        {
            vcam.Follow = manualTarget;
            vcam.LookAt = manualTarget;
        }
    }
}