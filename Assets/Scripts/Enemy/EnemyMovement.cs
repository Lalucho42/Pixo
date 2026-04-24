using UnityEngine;

public class EnemyMovement
{
    private EnemyAI enemy;
    private bool isChasing = false;
    private float noiseOffset;

    public EnemyMovement(EnemyAI brain)
    {
        enemy = brain;
        noiseOffset = Random.Range(0f, 100f);
    }

    public void Tick(float dt)
    {
        if (enemy.PlayerTarget == null) return;

        float dist = Vector3.Distance(enemy.transform.position, enemy.PlayerTarget.position);

        // Lógica de persecución básica
        UpdateChasingState(dist);

        if (!isChasing)
        {
            if (enemy.Agent.isOnNavMesh) enemy.Agent.isStopped = true;
            return;
        }

        // Ajustar velocidad
        enemy.Agent.speed = (dist <= enemy.walkRange) ? enemy.walkSpeed : enemy.runSpeed;

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

        // --- LÓGICA ESPECIAL DE DRON (ALTURA Y NOISE) ---
        if (enemy.type == EnemyType.Drone)
        {
            ApplyDroneFlight(dt);
        }
    }

    private void UpdateChasingState(float dist)
    {
        if (!isChasing && dist <= enemy.detectionRange) isChasing = true;
        else if (isChasing && dist > enemy.loseTargetRange) isChasing = false;
    }

    private void ApplyDroneFlight(float dt)
    {
        // 1. Calculamos la altura objetivo sobre el NavMesh
        float noise = Mathf.PerlinNoise(Time.time * enemy.noiseFrequency, noiseOffset) * enemy.noiseAmplitude;
        float targetY = enemy.Agent.nextPosition.y + enemy.hoverHeight + noise;

        // 2. Aplicamos la altura suavemente al transform
        Vector3 pos = enemy.transform.position;
        pos.y = Mathf.Lerp(pos.y, targetY, dt * enemy.hoverSmoothing);
        enemy.transform.position = pos;

        // 3. Inclinación estética (Tilt)
        float tilt = Vector3.Dot(enemy.Agent.velocity, enemy.transform.right);
        enemy.transform.rotation *= Quaternion.Euler(0, 0, -tilt * 2f);
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