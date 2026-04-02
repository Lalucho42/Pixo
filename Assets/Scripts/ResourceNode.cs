using UnityEngine;

public enum ResourceType { Madera, Piedra }

public class ResourceNode : MonoBehaviour
{
    public string resourceName;
    public ResourceType type;
    public float health = 100f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log(resourceName + " vida: " + health);
        if (health <= 0f) Destroy(gameObject);
    }
}