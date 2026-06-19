using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(HealthSystem))]
public class EnemyAI : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float detectionRange = 15f;
    public float loseTargetRange = 25f;
    public float attackRange = 1.6f;
    public float walkSpeed = 1.5f;

    [Header("Configuración Combate")]
    public float attackCooldown = 1.5f;
    public int damage = 10;

    [Header("Referencias Auto-detectables")]
    public EnemyHandDamage handDamageScript;
    public Animator animator;

    public NavMeshAgent Agent { get; private set; }
    public HealthSystem Health { get; private set; }
    public Transform PlayerTarget { get; private set; }

    private EnemyMovement movement;
    private MeleeCombatModule combatModule;
    private float stunTimer = 0f;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Health = GetComponent<HealthSystem>();

        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (handDamageScript == null) handDamageScript = GetComponentInChildren<EnemyHandDamage>();

        movement = new EnemyMovement(this);
        combatModule = new MeleeCombatModule(this);
    }

    private void Start()
    {
        Player p = FindFirstObjectByType<Player>();
        if (p != null) PlayerTarget = p.transform;

        if (Agent != null) Agent.stoppingDistance = attackRange - 0.2f;

        if (handDamageScript != null)
        {
            handDamageScript.Setup(damage);
        }
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

        float velocidadParaAnim = (Agent.isStopped || !Agent.isOnNavMesh) ? 0f : Agent.desiredVelocity.magnitude;
        if (animator != null) animator.SetFloat("Speed", velocidadParaAnim);
    }

    public void TriggerAttackAnimation()
    {
        if (animator != null) animator.SetTrigger("Punch");
    }

    public void ApplyKnockback(Vector3 push, float duration)
    {
        if (Agent != null && Agent.isOnNavMesh) Agent.Move(push);
        stunTimer = duration;
    }

    public void ActivarDañoMano()
    {
        if (handDamageScript != null) handDamageScript.SetDamageState(true);
    }

    public void DesactivarDañoMano()
    {
        if (handDamageScript != null) handDamageScript.SetDamageState(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, loseTargetRange);
    }
}