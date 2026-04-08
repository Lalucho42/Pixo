using UnityEngine;

public enum ResourceType { Madera, Piedra }

public class ResourceNode : MonoBehaviour
{
    public string resourceName;
    public ResourceType type;
    public float health = 100f;

    [Header("--- Configuracion de Drop ---")]
    public GameObject dropPrefab;
    public int dropAmount = 3;

    [Header("--- Punto de Spawn Seguro ---")]
    [Tooltip("Punto donde aparecerán los recursos. Si está vacío, usa el centro del árbol.")]
    public Transform spawnPoint; // <-- NUEVO TARGET

    public void TakeDamage(float amount)
    {
        if (health <= 0) return;

        health -= amount;
        Debug.Log(resourceName + " vida: " + health);

        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (dropPrefab != null)
        {
            // 1. Elegimos el origen. Si le pusiste un Target, usa ese. Si te olvidaste, usa el centro del árbol.
            Vector3 origenDeSpawn = spawnPoint != null ? spawnPoint.position : transform.position;

            for (int i = 0; i < dropAmount; i++)
            {
                // 2. Achiqué el radio de aleatoriedad (de 1.5f a 0.5f) para que se queden bien pegaditos al Target
                Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, Random.Range(-0.5f, 0.5f));

                Instantiate(dropPrefab, origenDeSpawn + randomOffset, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }
}