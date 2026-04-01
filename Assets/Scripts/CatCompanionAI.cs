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
    
    [Header("Animation Tuning")]
    public float speedSmoothTime = 0.1f;
    public float walkAnimationBaseSpeed = 3f;
    
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Vector3 lastPlayerPosition;
    private CharacterController playerController;
    private float smoothedSpeed;
    private float speedVelocity;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        
        SetupNavMeshAgent();
        FindPlayer();
        SetupCollider();
        StartCoroutine(InitializeAgent());
    }

    private void SetupNavMeshAgent()
    {
        if (navMeshAgent == null) return;
        navMeshAgent.enabled = false;
        navMeshAgent.stoppingDistance = followDistance;
        navMeshAgent.updateRotation = true;
        navMeshAgent.angularSpeed = 600f;
        navMeshAgent.acceleration = 25f;
        navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        navMeshAgent.radius = 0.25f;
    }

    private void FindPlayer()
    {
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
    }

    private void SetupCollider()
    {
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
        
        float targetSpeed = GetAdjustedSpeed();
        
        smoothedSpeed = Mathf.SmoothDamp(smoothedSpeed, targetSpeed, ref speedVelocity, speedSmoothTime);
        UpdateAnimator(smoothedSpeed);
        
        Transform nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            FleeFromEnemy(nearestEnemy);
        }
        else
        {
            FollowPlayer(smoothedSpeed);
        }
    }

    private float GetAdjustedSpeed()
    {
        float speed = navMeshAgent.velocity.magnitude;
        bool isPlayerMoving = playerController != null && 
            new Vector3(playerController.velocity.x, 0, playerController.velocity.z).magnitude > 0.1f;
        
        if (!isPlayerMoving && Vector3.Distance(transform.position, playerTarget.position) <= followDistance + 0.2f)
        {
            speed = 0f;
        }
        return speed;
    }

    private void UpdateAnimator(float speed)
    {
        if (animator == null) return;

        animator.SetFloat("Speed", speed);

        if (speed > 0.05f)
        {
            animator.speed = speed / walkAnimationBaseSpeed;
        }
        else
        {
            animator.speed = 1f;
        }
    }

    private Transform FindNearestEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, fleeRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy")) return hitCollider.transform;
        }
        return null;
    }

    private void FleeFromEnemy(Transform enemy)
    {
        if (!navMeshAgent.isActiveAndEnabled || !navMeshAgent.isOnNavMesh) return;
        navMeshAgent.stoppingDistance = 0f;
        Vector3 fleeDirection = (transform.position - enemy.position).normalized;
        Vector3 fleeTarget = transform.position + (fleeDirection * fleeDistance);
        navMeshAgent.SetDestination(fleeTarget);
    }

    private void FollowPlayer(float currentSpeed)
    {
        if (!navMeshAgent.isActiveAndEnabled || !navMeshAgent.isOnNavMesh) return;
        
        navMeshAgent.stoppingDistance = followDistance;
        if (Vector3.Distance(playerTarget.position, lastPlayerPosition) > movementBuffer)
        {
            lastPlayerPosition = playerTarget.position;
            navMeshAgent.SetDestination(playerTarget.position);
        }

        float remainingDistance = navMeshAgent.remainingDistance;
        if (!navMeshAgent.pathPending && remainingDistance <= navMeshAgent.stoppingDistance + 0.1f && currentSpeed < 0.1f)
        {
            Vector3 lookPos = playerTarget.position - transform.position;
            lookPos.y = 0;
            if (lookPos != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }
}
