using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Tooltip("El prefab del enemigo (Melee o Dron) que va a nacer acá")]
    public GameObject enemigoPrefab;

    // El Director va a llamar a esta función cuando sea el momento
    public GameObject Spawnear()
    {
        if (enemigoPrefab != null)
        {
            // Crea al enemigo exactamente en la posición y rotación de este objeto vacío
            return Instantiate(enemigoPrefab, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogWarning($"¡Cuidado! El objeto {gameObject.name} no tiene un prefab asignado.");
            return null;
        }
    }
}