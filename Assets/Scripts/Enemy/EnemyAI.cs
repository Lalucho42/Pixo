using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { Melee, Drone }

[RequireComponent(typeof(NavMeshAgent), typeof(HealthSystem))]
public class EnemyAI : MonoBehaviour
{
    [Header("Configuración del Chip")]
    public EnemyType type;

    [Header("Estadísticas")]
    public float detectionRange = 15f;
    public float loseTargetRange = 25f;
    public float attackRange = 1.6f;
    public float attackCooldown = 1.5f;
    public int damage = 10;

    [Header("Referencias de Disparo (Futuro/Dron)")]
    public GameObject projectilePrefab; // <-- Restaurado
    public Transform shootPoint;       // <-- Restaurado

    [Header("Combate por Animación (Melee)")]
    public EnemyHandDamage handDamageScript;

    [Header("Referencias Generales")]
    public Animator animator;
    public NavMeshAgent Agent { get; private set; }
    public HealthSystem Health { get; private set; }
    public Transform PlayerTarget { get; private set; }

    private EnemyMovement movement;
    private IEnemyCombat combatModule;
    private float stunTimer = 0f;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Health = GetComponent<HealthSystem>();
        movement = new EnemyMovement(this);

        // Selección de módulo basada en el tipo
        if (type == EnemyType.Melee)
            combatModule = new MeleeCombatModule(this);
        else
            combatModule = new DroneCombatModule(this);
    }

    private void Start()
    {
        Player p = FindFirstObjectByType<Player>();
        if (p != null) PlayerTarget = p.transform;

        if (Agent != null)
        {
            Agent.speed = 1.8f;
            Agent.stoppingDistance = attackRange - 0.2f;
        }

        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator != null) animator.applyRootMotion = false;
    }

    private void Update()
    {
        if (PlayerTarget == null || Health.IsDead)
        {
            if (Health.IsDead && Agent != null && Agent.isOnNavMesh) Agent.isStopped = true;
            return;
        }

        float dt = Time.deltaTime;

        if (stunTimer > 0)
        {
            stunTimer -= dt;
            if (Agent.isOnNavMesh) Agent.isStopped = true;
            return;
        }

        movement.Tick(dt);
        combatModule.UpdateCombat(dt);

        if (animator != null && Agent != null)
            animator.SetFloat("Speed", Agent.velocity.magnitude);
    }

    public void TriggerAttackAnimation()
    {
        if (animator != null) animator.SetTrigger("Punch");
    }

    // --- EVENTOS PARA EL MELEE ---
    public void EnemyStartHit()
    {
        if (handDamageScript != null)
        {
            handDamageScript.Setup(damage);
            handDamageScript.SetDamageState(true);
        }
    }

    public void EnemyEndHit()
    {
        if (handDamageScript != null) handDamageScript.SetDamageState(false);
    }

    public void ApplyKnockback(Vector3 push, float duration)
    {
        if (Agent != null && Agent.isOnNavMesh) Agent.Move(push);
        stunTimer = duration;
    }
}