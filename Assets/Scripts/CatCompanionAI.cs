using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public class CatCompanionAI : MonoBehaviour
{
    public Transform playerTarget;
    public float followDistance = 3.5f;
    public float movementBuffer = 1.0f;
    public float rotationSpeed = 2.0f;
    public float fleeRadius = 8f;
    public float fleeDistance = 5f;
    
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Vector3 lastPlayerPosition;
    private CharacterController playerController;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
            navMeshAgent.stoppingDistance = followDistance;
            navMeshAgent.updateRotation = true;
            navMeshAgent.acceleration = 8f;
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            navMeshAgent.radius = 0.3f;
        }
        
        Player player = FindFirstObjectByType<Player>();
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.GetComponent<Player>();
                playerTarget = playerObject.transform;
                lastPlayerPosition = playerTarget.position;
                playerController = playerObject.GetComponent<CharacterController>();
            }
        }

        if (player != null)
        {
            playerTarget = player.transform;
            lastPlayerPosition = playerTarget.position;
            playerController = player.Controller;
        }

        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col == null)
        {
            col = gameObject.AddComponent<CapsuleCollider>();
            col.center = new Vector3(0, 0.25f, 0);
            col.radius = 0.3f;
            col.height = 0.6f;
        }
        col.isTrigger = false;
        gameObject.layer = 0;

        StartCoroutine(InitializeAgent());
    }

    private System.Collections.IEnumerator InitializeAgent()
    {
        yield return new WaitForEndOfFrame();
        if (navMeshAgent != null)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10.0f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
            }
            navMeshAgent.enabled = true;
        }
    }

    private void Update()
    {
        if (playerTarget == null || navMeshAgent == null) return;
        
        float currentSpeed = navMeshAgent.velocity.magnitude;
        
        bool isPlayerMoving = false;
        if (playerController != null)
        {
            isPlayerMoving = new Vector3(playerController.velocity.x, 0, playerController.velocity.z).magnitude > 0.1f;
        }

        if (!isPlayerMoving && Vector3.Distance(transform.position, playerTarget.position) <= followDistance + 0.2f)
        {
            currentSpeed = 0f;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", currentSpeed);
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, fleeRadius);
        bool isFleeing = false;
        Transform nearestEnemy = null;
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                isFleeing = true;
                nearestEnemy = hitCollider.transform;
                break;
            }
        }

        if (isFleeing && nearestEnemy != null)
        {
            if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.stoppingDistance = 0f;
                Vector3 fleeDirection = (transform.position - nearestEnemy.position).normalized;
                Vector3 fleeTarget = transform.position + (fleeDirection * fleeDistance);
                navMeshAgent.SetDestination(fleeTarget);
            }
        }
        else
        {
            if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.stoppingDistance = followDistance;
                
                if (Vector3.Distance(playerTarget.position, lastPlayerPosition) > movementBuffer)
                {
                    lastPlayerPosition = playerTarget.position;
                    navMeshAgent.SetDestination(playerTarget.position);
                }
            }

            float remainingDistance = (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh) ? navMeshAgent.remainingDistance : 0f;
            if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh && !navMeshAgent.pathPending && remainingDistance <= navMeshAgent.stoppingDistance)
            {
                Vector3 lookPos = playerTarget.position - transform.position;
                lookPos.y = 0;
                if (lookPos != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }
            }
        }
    }
}
