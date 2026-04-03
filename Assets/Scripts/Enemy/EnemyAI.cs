using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { Melee, Drone }

[RequireComponent(typeof(NavMeshAgent), typeof(HealthSystem))]
public class EnemyAI : MonoBehaviour
{
    [Header("Configuración del Chip")]
    public EnemyType type;

    [Header("Estadísticas")]
    public float attackRange = 2.5f;
    public float attackCooldown = 1.5f;
    public int damage = 10;

    [Header("Referencias (Dron)")]
    public GameObject projectilePrefab;
    public Transform shootPoint;

    // Propiedades para los módulos
    public NavMeshAgent Agent { get; private set; }
    public HealthSystem Health { get; private set; }
    public Transform PlayerTarget { get; private set; }

    // Módulos
    private EnemyMovement movement;
    private IEnemyCombat combatModule;
    private float stunTimer = 0f;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Health = GetComponent<HealthSystem>();

        // Inyectamos los módulos
        movement = new EnemyMovement(this);

        // Selección de "Chip" de combate
        if (type == EnemyType.Melee)
            combatModule = new MeleeCombatModule(this);
        else
            combatModule = new DroneCombatModule(this);
    }

    private void Start()
    {
        // Buscamos al jugador una sola vez
        Player p = FindFirstObjectByType<Player>();
        if (p != null) PlayerTarget = p.transform;

        if (Agent != null) Agent.stoppingDistance = attackRange - 0.5f;
    }

    private void Update()
    {
        if (PlayerTarget == null || Health.IsDead) return;

        float dt = Time.deltaTime;

        if (stunTimer > 0)
        {
            stunTimer -= dt;
            Agent.isStopped = true;
            return;
        }

        movement.Tick(dt);
        combatModule.UpdateCombat(dt);
    }

    public void ApplyKnockback(Vector3 push, float duration)
    {
        if (Agent != null && Agent.isOnNavMesh) Agent.Move(push);
        stunTimer = duration;
    }
}