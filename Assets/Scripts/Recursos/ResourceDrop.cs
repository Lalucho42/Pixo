using UnityEngine;

public class ResourceDrop : MonoBehaviour
{
    public ResourceType type;
    public int amount = 1;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null && player.Inventory != null)
        {
            player.Inventory.AddResource(type, amount);
            Destroy(gameObject);
        }
    }
}
