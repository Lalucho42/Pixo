using UnityEngine;

public class RangedEnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4.5f;
    public float hoverHeight = 3.5f;
    public float hoverSmoothing = 6f;
    public float groundCheckDistance = 20f;

    [Header("Drone Feel")]
    public float noiseAmplitude = 0.18f;
    public float noiseFrequency = 0.6f;
    public float tiltAmount = 12f;
    public float tiltSmoothing = 5f;

    [Header("Combat")]
    public float attackRange = 14f;
    public float stopDistance = 7f;
    public float circleRadius = 7f;
    public float circleSpeed = 35f;
    public float fireCooldown = 2.2f;

    [Header("References")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    private Rigidbody rb;
    private Transform playerTarget;
    private float fireTimer = 0f;
    private float stunTimer = 0f;
    private float noiseOffsetX;
    private float noiseOffsetZ;
    private float circleAngle = 0f;
    private Quaternion targetRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.linearDamping = 4f;
        rb.angularDamping = 8f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        noiseOffsetX = Random.Range(0f, 100f);
        noiseOffsetZ = Random.Range(0f, 100f);
        circleAngle = Random.Range(0f, 360f);
        targetRotation = transform.rotation;
    }

    private void Start()
    {
        Player p = FindFirstObjectByType<Player>();
        if (p != null) playerTarget = p.transform;

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = false;
    }

    private void FixedUpdate()
    {
        if (playerTarget == null)
        {
            Player p = FindFirstObjectByType<Player>();
            if (p != null) playerTarget = p.transform;
            if (playerTarget == null) return;
        }

        fireTimer -= Time.fixedDeltaTime;

        if (stunTimer > 0)
        {
            stunTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * 8f);
            return;
        }

        float targetY = ComputeTargetY();
        Vector3 droneNoise = ComputeDroneNoise();
        float distToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        Vector3 desiredVelocity;

        if (distToPlayer <= attackRange)
        {
            desiredVelocity = ComputeCombatVelocity(distToPlayer, targetY);

            if (fireTimer <= 0f)
            {
                Shoot();
            }
        }
        else
        {
            desiredVelocity = ComputePursuitVelocity(targetY);
        }

        desiredVelocity += droneNoise;
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, desiredVelocity, Time.fixedDeltaTime * 5f);

        ApplyDroneTilt();
        FacePlayer();
    }

    private float ComputeTargetY()
    {
        float groundY = transform.position.y - hoverHeight;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance, ~LayerMask.GetMask("Enemy", "RangedGuard", "Player")))
        {
            groundY = hit.point.y;
        }

        return Mathf.Lerp(transform.position.y, groundY + hoverHeight, Time.fixedDeltaTime * hoverSmoothing);
    }

    private Vector3 ComputeDroneNoise()
    {
        float t = Time.time * noiseFrequency;
        float nx = (Mathf.PerlinNoise(t + noiseOffsetX, 0f) - 0.5f) * 2f * noiseAmplitude;
        float ny = (Mathf.PerlinNoise(0f, t + noiseOffsetX) - 0.5f) * 2f * noiseAmplitude * 0.5f;
        float nz = (Mathf.PerlinNoise(t + noiseOffsetZ, 1f) - 0.5f) * 2f * noiseAmplitude;
        return new Vector3(nx, ny, nz);
    }

    private Vector3 ComputePursuitVelocity(float targetY)
    {
        Vector3 flat = playerTarget.position - transform.position;
        flat.y = 0;
        Vector3 xzVelocity = flat.normalized * moveSpeed;
        float yVelocity = (targetY - transform.position.y) * hoverSmoothing;
        return new Vector3(xzVelocity.x, yVelocity, xzVelocity.z);
    }

    private Vector3 ComputeCombatVelocity(float distToPlayer, float targetY)
    {
        circleAngle += circleSpeed * Time.fixedDeltaTime;
        float rad = circleAngle * Mathf.Deg2Rad;

        Vector3 circleOffset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * circleRadius;
        Vector3 orbitalTarget = playerTarget.position + circleOffset;

        Vector3 flat = orbitalTarget - transform.position;
        flat.y = 0;

        float speedMult = distToPlayer < stopDistance ? 0.4f : 1f;
        Vector3 xzVelocity = flat.normalized * moveSpeed * speedMult;
        float yVelocity = (targetY - transform.position.y) * hoverSmoothing;

        return new Vector3(xzVelocity.x, yVelocity, xzVelocity.z);
    }

    private void FacePlayer()
    {
        if (playerTarget == null) return;
        Vector3 dir = playerTarget.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.01f) return;
        Quaternion look = Quaternion.LookRotation(dir);
        targetRotation = Quaternion.Slerp(targetRotation, look, Time.fixedDeltaTime * 6f);
    }

    private void ApplyDroneTilt()
    {
        Vector3 vel = rb.linearVelocity;
        float rightTilt = -Vector3.Dot(vel, transform.right) * (tiltAmount / moveSpeed);
        float forwardTilt = Vector3.Dot(vel, transform.forward) * (tiltAmount / moveSpeed);

        rightTilt = Mathf.Clamp(rightTilt, -tiltAmount, tiltAmount);
        forwardTilt = Mathf.Clamp(forwardTilt, -tiltAmount, tiltAmount);

        Quaternion tiltRot = targetRotation * Quaternion.Euler(forwardTilt, 0f, rightTilt);
        transform.rotation = Quaternion.Slerp(transform.rotation, tiltRot, Time.fixedDeltaTime * tiltSmoothing);
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Collider[] shooterColliders = GetComponentsInChildren<Collider>();
        Collider bulletCollider = bullet.GetComponent<Collider>();
        if (bulletCollider != null)
        {
            foreach (var c in shooterColliders) Physics.IgnoreCollision(bulletCollider, c);
        }

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            Vector3 aimDir = (playerTarget.position + Vector3.up * 1f - firePoint.position).normalized;
            bulletRb.linearVelocity = aimDir * 22f;
        }

        fireTimer = fireCooldown;
    }

    public void ApplyKnockback(Vector3 pushVector, float stunDuration)
    {
        if (rb != null)
        {
            rb.AddForce(pushVector * 2f, ForceMode.Impulse);
        }
        stunTimer = stunDuration;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        if (playerTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerTarget.position, circleRadius);
        }
    }
}
