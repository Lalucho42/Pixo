using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject obj)
    {
        if (obj.CompareTag("Player"))
        {
            HealthSystem health = obj.GetComponent<HealthSystem>();
            if (health == null) health = obj.GetComponentInParent<HealthSystem>();
            if (health == null) health = obj.GetComponentInChildren<HealthSystem>();

            if (health != null && !health.IsDead)
            {
                health.TakeDamage(9999);
            }
            else
            {
                Player.IsDead = true;
                if (GameManager.instance != null) GameManager.instance.ShowDeathMenu();
            }
        }
    }
}
