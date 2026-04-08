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
        // Buscamos colisiones en el rango
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, interactRange);

        foreach (Collider col in colliders)
        {
            // Magia pura: Preguntamos si el objeto tocado tiene CUALQUIER script que use la interfaz
            IInteractable interactable = col.GetComponent<IInteractable>();

            if (interactable != null)
            {
                // Si lo tiene, le decimos "hacé lo tuyo" (el puente se repara, el gato maúlla, el arma se agarra)
                interactable.Interact(player);
                break; // Solo interactuamos con uno por vez
            }
        }
    }
}