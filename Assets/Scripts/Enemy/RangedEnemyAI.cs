using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class RangedEnemyAI : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 3.5f;
    public float hoverHeight = 1.5f;
    public float hoverSmoothing = 3f;
    public float groundCheckDistance = 10f;

    [Header("Estetica Dron")]
    public float noiseAmplitude = 0.15f;
    public float noiseFrequency = 0.5f;
    public float tiltAmount = 10f;
    public float tiltSmoothing = 4f;

    [Header("Combate")]
    public float attackRange = 12f;
    public float stopDistance = 6f;
    public float circleRadius = 5f;
    public float circleSpeed = 30f;
    public float fireCooldown = 2.5f;

    public float projectileSpeed = 18f;
    [Range(0f, 3f)] public float shotSpread = 1.5f;

    [Header("Referencias")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    private Rigidbody rb;
    private HealthSystem health;
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
        health = GetComponent<HealthSystem>();

        rb.useGravity = false;
        rb.linearDamping = 4f;
        rb.angularDamping = 8f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        noiseOffsetX = Random.Range(0f, 100f);
        noiseOffsetZ = Random.Range(0f, 100f);
        circleAngle = Random.Range(0f, 360f);
    }

    private void Start()
    {
        Player p = FindFirstObjectByType<Player>();
        if (p != null) playerTarget = p.transform;
    }

    private void FixedUpdate()
    {
        if (health != null && health.IsDead)
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            return;
        }

        if (playerTarget == null) return;

        fireTimer -= Time.fixedDeltaTime;

        if (stunTimer > 0)
        {
            stunTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * 4f);
            return;
        }

        float targetY = ComputeTargetY();
        Vector3 desiredVelocity;
        float distToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distToPlayer <= attackRange)
        {
            desiredVelocity = ComputeCombatVelocity(distToPlayer, targetY);
            if (fireTimer <= 0f) Shoot();
        }
        else
        {
            desiredVelocity = ComputePursuitVelocity(targetY);
        }

        desiredVelocity += ComputeDroneNoise();
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, desiredVelocity, Time.fixedDeltaTime * 5f);

        ApplyDroneTilt();
        FacePlayer();
    }

    private float ComputeTargetY()
    {
        float groundY = transform.position.y - hoverHeight;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance, ~LayerMask.GetMask("Enemy", "Player")))
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
        float speedMult = distToPlayer < stopDistance ? 0.5f : 1f;
        Vector3 xzVelocity = flat.normalized * moveSpeed * speedMult;
        float yVelocity = (targetY - transform.position.y) * hoverSmoothing;
        return new Vector3(xzVelocity.x, yVelocity, xzVelocity.z);
    }

    private void FacePlayer()
    {
        Vector3 dir = playerTarget.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.01f) return;
        targetRotation = Quaternion.Slerp(targetRotation, Quaternion.LookRotation(dir), Time.fixedDeltaTime * 6f);
    }

    private void ApplyDroneTilt()
    {
        Vector3 vel = rb.linearVelocity;
        float rightTilt = -Vector3.Dot(vel, transform.right) * (tiltAmount / moveSpeed);
        float forwardTilt = Vector3.Dot(vel, transform.forward) * (tiltAmount / moveSpeed);
        Quaternion tiltRot = targetRotation * Quaternion.Euler(forwardTilt, 0f, rightTilt);
        transform.rotation = Quaternion.Slerp(transform.rotation, tiltRot, Time.fixedDeltaTime * tiltSmoothing);
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb != null)
        {
            Vector3 targetPos = playerTarget.position + Vector3.up * 1f;
            Vector3 randomOffset = Random.insideUnitSphere * shotSpread;
            Vector3 aimDir = (targetPos + randomOffset - firePoint.position).normalized;
            bulletRb.linearVelocity = aimDir * projectileSpeed;
        }
        fireTimer = fireCooldown;
    }

    public void ApplyKnockback(Vector3 pushVector, float duration)
    {
        if (rb != null)
        {
            rb.AddForce(pushVector * 2.5f, ForceMode.Impulse);
            rb.AddForce(Vector3.down * 3f, ForceMode.Impulse);
        }
        stunTimer = duration;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (playerTarget != null)
        {
            Gizmos.color = Color.green;
            DrawWireCircle(playerTarget.position, circleRadius);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * groundCheckDistance);
    }

    private void DrawWireCircle(Vector3 center, float radius)
    {
        float angle = 0f;
        Vector3 lastPoint = center + new Vector3(Mathf.Cos(0) * radius, 0, Mathf.Sin(0) * radius);
        for (int i = 1; i <= 32; i++)
        {
            angle = i / 32f * Mathf.PI * 2f;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }
}