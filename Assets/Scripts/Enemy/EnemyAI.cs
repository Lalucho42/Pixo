using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { Melee, Drone }

[RequireComponent(typeof(NavMeshAgent), typeof(HealthSystem))]
public class EnemyAI : MonoBehaviour
{
    public EnemyType type;

    public float detectionRange = 15f;
    public float loseTargetRange = 25f;
    public float attackRange = 1.6f;
    public float walkRange = 5f;
    public float runSpeed = 3.5f;
    public float walkSpeed = 1.5f;
    public float attackCooldown = 1.5f;
    public int damage = 10;

    public GameObject projectilePrefab;
    public Transform shootPoint;

    public EnemyHandDamage handDamageScript;

    public Animator animator;
    public NavMeshAgent Agent { get; private set; }
    public HealthSystem Health { get; private set; }
    public Transform PlayerTarget { get; private set; }

    private EnemyMovement movement;
    private IEnemyCombat combatModule;
    private float stunTimer = 0f;

    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int walkHash = Animator.StringToHash("WalkForward");
    private readonly int runHash = Animator.StringToHash("Run Forward");
    private readonly int idleHash = Animator.StringToHash("Idle");
    private readonly int combatIdleHash = Animator.StringToHash("Combat Idle");
    private readonly int punchHash = Animator.StringToHash("Punch");
    private readonly int attack1Hash = Animator.StringToHash("Attack1");

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Health = GetComponent<HealthSystem>();
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

        if (Agent != null)
        {
            Agent.stoppingDistance = attackRange - 0.2f;
        }

        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator != null) animator.applyRootMotion = false;
    }

    private void Update()
    {
        if (PlayerTarget == null || Health.IsDead)
        {
            if (Health.IsDead)
            {
                if (Agent != null && Agent.isActiveAndEnabled && Agent.isOnNavMesh) Agent.isStopped = true;
                UpdateAnimation(0);
            }
            return;
        }

        float dt = Time.deltaTime;

        if (stunTimer > 0)
        {
            stunTimer -= dt;
            if (Agent.isActiveAndEnabled && Agent.isOnNavMesh) Agent.isStopped = true;
            UpdateAnimation(0);
            return;
        }

        movement.Tick(dt);
        combatModule.UpdateCombat(dt);

        if (Agent != null)
        {
            UpdateAnimation(Agent.velocity.magnitude);
        }
    }

    private void UpdateAnimation(float speed)
    {
        if (animator == null) return;

        animator.SetFloat(speedHash, speed);

        bool isMoving = speed > 0.1f;
        bool isRunning = speed > 2.5f;

        animator.SetBool(walkHash, isMoving && !isRunning);
        animator.SetBool(runHash, isRunning);

        bool hasTarget = PlayerTarget != null && Vector3.Distance(transform.position, PlayerTarget.position) < detectionRange;
        animator.SetBool(idleHash, !isMoving && !hasTarget);
        animator.SetBool(combatIdleHash, !isMoving && hasTarget);
    }

    public void TriggerAttackAnimation()
    {
        if (animator == null) return;

        animator.SetTrigger(punchHash);
        animator.SetTrigger(attack1Hash);
    }

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
        if (Agent != null && Agent.isActiveAndEnabled && Agent.isOnNavMesh) Agent.Move(push);
        stunTimer = duration;
    }
}
