using UnityEngine;

// La "I" al principio es por convención, significa Interface.
public interface IInteractable
{
    // Este es el contrato. Cualquier script que firme este contrato 
    // ESTÁ OBLIGADO a tener una función llamada Interact que reciba al Player.
    void Interact(Player player);
}