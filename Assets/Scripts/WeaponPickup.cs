using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponPickup : MonoBehaviour
{
    public string weaponHandObjectName = "PipeWeapon_Hand";
    public float interactionRadius = 2.5f;
    public AudioClip pickupSound;

    private void Update()
    {
        if (GameManager.IsPaused || GameManager.IsDead) return;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null || playerObj == gameObject) return;

        if (transform.root == playerObj.transform) return;

        float distance = Vector3.Distance(transform.position, playerObj.transform.position);
        
        bool isPressingInteract = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
        
        if (distance <= interactionRadius && isPressingInteract)
        {
            PlayerWeaponManager manager = playerObj.GetComponent<PlayerWeaponManager>();
            if (manager != null)
            {
                manager.ActivateWeaponByName(weaponHandObjectName);
                
                if (pickupSound != null && AudioManager.instance != null)
                {
                    AudioManager.instance.Play3DSFX(pickupSound, transform.position);
                }
                
                Destroy(gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
