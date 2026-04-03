using UnityEngine;

public class EnemyMovement
{
    private EnemyAI enemy;

    public EnemyMovement(EnemyAI brain) { enemy = brain; }

    public void Tick(float dt)
    {
        float dist = Vector3.Distance(enemy.transform.position, enemy.PlayerTarget.position);

        if (dist <= enemy.attackRange)
        {
            enemy.Agent.isStopped = true;
            RotateTowardsPlayer(dt);
        }
        else
        {
            enemy.Agent.isStopped = false;
            enemy.Agent.SetDestination(enemy.PlayerTarget.position);
        }
    }

    private void RotateTowardsPlayer(float dt)
    {
        Vector3 dir = (enemy.PlayerTarget.position - enemy.transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRot, dt * 5f);
        }
    }
}