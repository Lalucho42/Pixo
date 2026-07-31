using System.Collections;
using UnityEngine;

public enum ResourceType { Madera, Piedra, ParteComputadora, Hierro }

public class ResourceNode : MonoBehaviour
{
    public string resourceName;
    public ResourceType type;
    public float health = 100f;

    [Header("Efecto de Temblor")]
    public float shakeDuration = 0.15f;
    public float shakeStrength = 0.1f;

    [Header("Drop Settings")]
    public GameObject dropPrefab;
    public int dropAmount = 3;

    [Header("Spawn Settings")]
    public Transform spawnPoint;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void TakeDamage(float amount)
    {
        if (health <= 0) return;

        health -= amount;

        // Inicia el temblor al recibir impacto
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeCoroutine());

        if (health <= 0f)
        {
            Die();
        }
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            Vector3 randomOffset = Random.insideUnitSphere * shakeStrength;
            transform.localPosition = originalPosition + randomOffset;
            yield return null;
        }

        transform.localPosition = originalPosition;
        shakeCoroutine = null;
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