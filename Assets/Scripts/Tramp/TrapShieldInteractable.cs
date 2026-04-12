using UnityEngine;

public class TrapShieldInteractable : MonoBehaviour, IInteractable
{
    public TutorialTrap director;

    public void Interact(Player player)
    {
        if (director != null) director.EscudoInteractuado();
    }
}