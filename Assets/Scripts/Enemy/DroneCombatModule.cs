using UnityEngine;

public class DroneCombatModule : IEnemyCombat
{
    private EnemyAI enemy;
    private float timer;

    public DroneCombatModule(EnemyAI brain) { enemy = brain; }

    public void UpdateCombat(float dt)
    {
        if (timer > 0) timer -= dt;

        float dist = Vector3.Distance(enemy.transform.position, enemy.PlayerTarget.position);

        if (dist <= enemy.attackRange && timer <= 0)
        {
            enemy.TriggerAttackAnimation();
            Shoot();
            timer = enemy.attackCooldown;
        }
    }

    private void Shoot()
    {
        if (enemy.projectilePrefab == null || enemy.shootPoint == null) return;

        GameObject projectile = Object.Instantiate(enemy.projectilePrefab, enemy.shootPoint.position, enemy.shootPoint.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = (enemy.PlayerTarget.position + Vector3.up - enemy.shootPoint.position).normalized;
            rb.AddForce(dir * 20f, ForceMode.Impulse);
        }
    }
}