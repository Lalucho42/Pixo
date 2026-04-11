using UnityEngine;

public class EnemyMovement
{
    private EnemyAI enemy;

    private bool isChasing = false;

    public EnemyMovement(EnemyAI brain) { enemy = brain; }

    public void Tick(float dt)
    {
        if (enemy.PlayerTarget == null) return;

        float dist = Vector3.Distance(enemy.transform.position, enemy.PlayerTarget.position);

        // Logica de deteccion
        if (!isChasing && dist <= enemy.detectionRange)
        {
            isChasing = true;
        }
        else if (isChasing && dist > enemy.loseTargetRange)
        {
            isChasing = false;
            enemy.Agent.isStopped = true;
            return;
        }

        if (!isChasing)
        {
            // Idle si no está persiguiendo
            if (enemy.Agent.isOnNavMesh) enemy.Agent.isStopped = true;
            return;
        }

        // Si está persiguiendo
        if (dist <= enemy.attackRange)
        {
            if (enemy.Agent.isOnNavMesh) enemy.Agent.isStopped = true;
            RotateTowardsPlayer(dt);
        }
        else
        {
            if (enemy.Agent.isOnNavMesh)
            {
                enemy.Agent.isStopped = false;
                enemy.Agent.SetDestination(enemy.PlayerTarget.position);
            }
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