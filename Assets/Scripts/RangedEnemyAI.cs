using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RangedEnemyAI : MonoBehaviour
{
    public Transform[] waypoints;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackRange = 12f;
    public float stopDistance = 8f; // Trata de mantener este espacio
    public float fireCooldown = 2f;
    private int currentWaypointIndex = 0;
    private float fireTimer = 0f;
    private NavMeshAgent navMeshAgent;
    private Transform playerTarget;
    private float stunTimer = 0f;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
            navMeshAgent.stoppingDistance = stopDistance;
            navMeshAgent.autoBraking = true;
            navMeshAgent.acceleration = 10f;
        }

        Player p = FindFirstObjectByType<Player>();
        if (p != null) playerTarget = p.transform;
        
        StartCoroutine(InitializeAgent());
    }

    private System.Collections.IEnumerator InitializeAgent()
    {
        yield return new WaitForSeconds(0.5f);
        if (navMeshAgent == null) yield break;
        
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            navMeshAgent.enabled = true;
            if (waypoints != null && waypoints.Length > 0) navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private void Update()
    {
        if (navMeshAgent == null || !navMeshAgent.enabled) return;
        
        if (playerTarget == null)
        {
            Player p = FindFirstObjectByType<Player>();
            if (p != null) playerTarget = p.transform;
            if (playerTarget == null) return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        fireTimer -= Time.deltaTime;

        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            navMeshAgent.isStopped = true;
            return;
        }

        // Si esta en rango de disparo pero no demasiado cerca
        if (distanceToPlayer <= attackRange)
        {
            Combat(distanceToPlayer);
        }
        else
        {
            PursuePlayer();
        }
    }

    private void PursuePlayer()
    {
        if (navMeshAgent == null || !navMeshAgent.enabled) return;
        
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(playerTarget.position);
    }

    private void Combat(float distance)
    {
        // Se detiene si ya esta en una buena posicion
        navMeshAgent.isStopped = (distance >= stopDistance - 0.5f);
        if (!navMeshAgent.isStopped)
        {
            navMeshAgent.SetDestination(playerTarget.position);
        }

        RotateTowardsPlayer();
        
        if (fireTimer <= 0)
        {
            Shoot();
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            
            Collider[] shooterColliders = GetComponentsInChildren<Collider>();
            Collider bulletCollider = bullet.GetComponent<Collider>();
            if (bulletCollider != null)
            {
                foreach (var c in shooterColliders) Physics.IgnoreCollision(bulletCollider, c);
            }

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = firePoint.forward * 20f;
            }
            fireTimer = fireCooldown;
        }
    }

    public void ApplyKnockback(Vector3 pushVector, float stunDuration)
    {
        if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.Move(pushVector);
        }
        stunTimer = stunDuration;
    }
}
