using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Tooltip("El prefab del enemigo que va a nacer aca")]
    public GameObject enemigoPrefab;

    public GameObject Spawnear()
    {
        if (enemigoPrefab != null)
        {
            return Instantiate(enemigoPrefab, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogWarning($"El objeto {gameObject.name} no tiene un prefab asignado.");
            return null;
        }
    }
}