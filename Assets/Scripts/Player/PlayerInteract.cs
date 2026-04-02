using UnityEngine;

public class PlayerInteract
{
    private Player player;
    private float interactRange = 2.5f;

    public PlayerInteract(Player playerBrain)
    {
        player = playerBrain;
        player.InputHandler.OnInteractEvent += TryInteract;
    }

    public void Tick(float deltaTime) { }

    private void TryInteract()
    {
        // Buscamos objetos interactuables en un radio
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, interactRange);
        foreach (Collider col in colliders)
        {
            WeaponPickup pickup = col.GetComponent<WeaponPickup>();
            if (pickup != null)
            {
                pickup.Interact(player);
                break; // Solo agarramos uno por vez
            }
        }
    }
}