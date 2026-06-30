using UnityEngine;

public class FinalZoneTrigger : MonoBehaviour
{
    [SerializeField] private GestorFinNivel gestorFin;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gestorFin != null)
        {
            gestorFin.ActivarFinNivel();
        }
    }
}