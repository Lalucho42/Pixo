using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    public string weaponHandObjectName = "PipeWeapon_Hand";
    public AudioClip pickupSound;

    public void Interact(Player player)
    {
        if (player.WeaponManager != null)
        {
            player.WeaponManager.ActivateWeaponByName(weaponHandObjectName);

            if (pickupSound != null && AudioManager.instance != null)
            {
                AudioManager.instance.Play3DSFX(pickupSound, transform.position);
            }

            Destroy(gameObject);
        }
    }
}