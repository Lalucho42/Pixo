using UnityEngine;

public class MissionTriggerZone : MonoBehaviour
{
    public string textoDeEstaZona;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (MissionUI.Instance != null)
            {
                MissionUI.Instance.ActualizarMision(textoDeEstaZona);
            }
            Destroy(gameObject);
        }
    }
}