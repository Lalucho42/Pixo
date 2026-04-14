using UnityEngine;

public class MeleeCombatModule : IEnemyCombat
{
    private EnemyAI enemy;
    private float timer;

    public MeleeCombatModule(EnemyAI brain) { enemy = brain; }

    public void UpdateCombat(float dt)
    {
        if (timer > 0) timer -= dt;

        if (enemy.PlayerTarget == null) return;

        float dist = Vector3.Distance(enemy.transform.position, enemy.PlayerTarget.position);

        if (dist <= enemy.attackRange && timer <= 0)
        {
            enemy.TriggerAttackAnimation();
            timer = enemy.attackCooldown;
        }
    }
}