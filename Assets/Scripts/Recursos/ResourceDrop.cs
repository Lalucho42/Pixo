using UnityEngine;


public class ResourceDrop : MonoBehaviour, IInteractable
{
    public ResourceType tipoDeRecurso;
    public int cantidad = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (tipoDeRecurso != ResourceType.ParteComputadora)
        {
            Player jugador = other.GetComponent<Player>();

            if (jugador != null && jugador.Inventory != null)
            {
                AgarrarObjeto(jugador);
            }
        }
    }

    public void Interact(Player jugador)
    {
        if (tipoDeRecurso == ResourceType.ParteComputadora)
        {
            AgarrarObjeto(jugador);
        }
    }

    private void AgarrarObjeto(Player jugador)
    {
        jugador.Inventory.AddResource(tipoDeRecurso, cantidad);
        Destroy(gameObject);
    }
}