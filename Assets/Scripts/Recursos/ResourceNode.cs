using UnityEngine;

public enum ResourceType { Madera, Piedra }

public class ResourceNode : MonoBehaviour
{
    public string resourceName;
    public ResourceType type;
    public float health = 100f;

    [Header("Drop Settings")]
    public GameObject dropPrefab;
    public int dropAmount = 3;

    [Header("Spawn Settings")]
    public Transform spawnPoint;

    public void TakeDamage(float amount)
    {
        if (health <= 0) return;

        health -= amount;

        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (dropPrefab != null)
        {
            Vector3 spawnOrigin = spawnPoint != null ? spawnPoint.position : transform.position;

            for (int i = 0; i < dropAmount; i++)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, Random.Range(-0.5f, 0.5f));
                Instantiate(dropPrefab, spawnOrigin + randomOffset, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }
}
