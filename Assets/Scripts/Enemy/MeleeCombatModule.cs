using UnityEngine;

public class MeleeCombatModule : IEnemyCombat
{
    private EnemyAI enemy;
    private float timer;

    public MeleeCombatModule(EnemyAI brain) { enemy = brain; }

    public void UpdateCombat(float dt)
    {
        if (timer > 0) timer -= dt;

        float dist = Vector3.Distance(enemy.transform.position, enemy.PlayerTarget.position);

        if (dist <= enemy.attackRange && timer <= 0)
        {
            // Atacamos directamente al HealthSystem del jugador
            HealthSystem pHealth = enemy.PlayerTarget.GetComponent<HealthSystem>();
            if (pHealth != null)
            {
                enemy.TriggerAttackAnimation();
                pHealth.TakeDamage(enemy.damage);
                timer = enemy.attackCooldown;
                Debug.Log("¡Enemigo Melee golpeó!");
            }
        }
    }
}