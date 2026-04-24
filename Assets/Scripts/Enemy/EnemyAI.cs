using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { Melee, Drone }

[RequireComponent(typeof(NavMeshAgent), typeof(HealthSystem))]
public class EnemyAI : MonoBehaviour
{
    public EnemyType type;

    [Header("Configuración de Movimiento")]
    public float detectionRange = 15f;
    public float loseTargetRange = 25f;
    public float attackRange = 1.6f;
    public float walkRange = 5f;
    public float runSpeed = 3.5f;
    public float walkSpeed = 1.5f;

    [Header("Configuración Dron (Vuelo)")]
    public float hoverHeight = 2f;
    public float hoverSmoothing = 3f;
    public float noiseAmplitude = 0.2f;
    public float noiseFrequency = 1.5f;

    [Header("Configuración Combate")]
    public float attackCooldown = 1.5f;
    public int damage = 10;
    public GameObject projectilePrefab;
    public Transform shootPoint;

    [Header("Referencias")]
    public EnemyHandDamage handDamageScript;
    public Animator animator;

    public NavMeshAgent Agent { get; private set; }
    public HealthSystem Health { get; private set; }
    public Rigidbody Rb { get; private set; } // Necesario para el dron
    public Transform PlayerTarget { get; private set; }

    private EnemyMovement movement;
    private IEnemyCombat combatModule;
    private float stunTimer = 0f;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Health = GetComponent<HealthSystem>();
        Rb = GetComponent<Rigidbody>();

        // Si es Dron, configuramos físicas para que no caiga
        if (type == EnemyType.Drone)
        {
            if (Rb == null) Rb = gameObject.AddComponent<Rigidbody>();
            Rb.useGravity = false;
            Rb.isKinematic = true; // El movimiento lo dicta el script
        }

        movement = new EnemyMovement(this);

        if (type == EnemyType.Melee)
            combatModule = new MeleeCombatModule(this);
        else
            combatModule = new DroneCombatModule(this);
    }

    private void Start()
    {
        Player p = FindFirstObjectByType<Player>();
        if (p != null) PlayerTarget = p.transform;

        if (Agent != null) Agent.stoppingDistance = attackRange - 0.2f;
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (PlayerTarget == null || Health.IsDead)
        {
            if (Health.IsDead && Agent != null && Agent.isOnNavMesh) Agent.isStopped = true;
            return;
        }

        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            if (Agent.isOnNavMesh) Agent.isStopped = true;
            return;
        }

        movement.Tick(Time.deltaTime);
        combatModule.UpdateCombat(Time.deltaTime);

        if (animator != null) animator.SetFloat("Speed", Agent.velocity.magnitude);
    }

    public void TriggerAttackAnimation()
    {
        if (animator != null) animator.SetTrigger("Punch"); // O la que uses para disparar
    }

    public void ApplyKnockback(Vector3 push, float duration)
    {
        if (Agent != null && Agent.isOnNavMesh) Agent.Move(push);
        stunTimer = duration;
    }
}