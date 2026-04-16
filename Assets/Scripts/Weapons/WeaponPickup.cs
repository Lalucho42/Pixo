using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    public string weaponHandObjectName = "PipeWeapon_Hand";

    public void Interact(Player player)
    {
        if (player.WeaponManager != null)
        {
            player.WeaponManager.ActivateWeaponByName(weaponHandObjectName);


            Destroy(gameObject);
        }
    }
}