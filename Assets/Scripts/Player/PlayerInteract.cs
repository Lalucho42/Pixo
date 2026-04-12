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
        if (player.IsMovementLocked) return;

        Collider[] colliders = Physics.OverlapSphere(player.transform.position, interactRange);

        foreach (Collider col in colliders)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(player);
                break;
            }
        }
    }
}