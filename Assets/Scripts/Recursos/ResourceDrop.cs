using UnityEngine;

public class ResourceDrop : MonoBehaviour
{
    public ResourceType type;
    public int amount = 1;

    private void OnTriggerEnter(Collider other)
    {
        // 1. Buscamos el "Cerebro" del objeto que nos pisó
        Player player = other.GetComponent<Player>();

        // 2. Si es el jugador y tiene su módulo de inventario activo...
        if (player != null && player.Inventory != null)
        {
            // 3. Le mandamos los recursos a su módulo
            player.Inventory.AddResource(type, amount);

            Destroy(gameObject);
        }
    }
}