using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTransform;
    private float stunTimer = 0f;
    public float attackRange = 2f;
    public int attackDamage = 10;
    public float attackCooldown = 1.5f;
    private float attackTimer = 0f;
    private CinemachineImpulseSource impulseSource;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        
        if (agent != null)
        {
            agent.enabled = false;
            agent.stoppingDistance = attackRange - 0.2f;
            agent.autoBraking = true;
            agent.acceleration = 12f;
        }

        Player p = FindFirstObjectByType<Player>();
        if (p != null) playerTransform = p.transform;
        
        StartCoroutine(InitializeAgent());
    }

    private System.Collections.IEnumerator InitializeAgent()
    {
        yield return new WaitForSeconds(0.5f);
        if (agent == null) yield break;
        
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            agent.enabled = true;
        }
    }

    private void Update()
    {
        if (agent == null || !agent.enabled) return;
        
        attackTimer -= Time.deltaTime;
        
        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            agent.isStopped = true;
            return;
        }

        if (playerTransform == null)
        {
            Player p = FindFirstObjectByType<Player>();
            if (p != null) playerTransform = p.transform;
            if (playerTransform == null) return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange)
        {
            agent.isStopped = true;
            RotateTowardsPlayer();
            
            if (attackTimer <= 0f)
            {
                Attack();
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void Attack()
    {
        HealthSystem playerHealth = playerTransform.GetComponent<HealthSystem>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            attackTimer = attackCooldown;
            if (impulseSource != null) impulseSource.GenerateImpulse(0.5f);
        }
    }

    public void ApplyKnockback(Vector3 pushVector, float stunDuration)
    {
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.Move(pushVector);
        }
        stunTimer = stunDuration;
    }
}
