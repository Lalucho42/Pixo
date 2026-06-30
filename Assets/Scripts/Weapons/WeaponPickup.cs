using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    public string weaponHandObjectName = "PipeWeapon_Hand";

    public void Interact(Player player)
    {
        if (player.WeaponManager != null)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX2D("Arma_Agarrar");
            }

            player.WeaponManager.ActivateWeaponByName(weaponHandObjectName);

            Destroy(gameObject);
        }
    }
}