using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { Melee, Drone }

[RequireComponent(typeof(NavMeshAgent), typeof(HealthSystem))]
public class EnemyAI : MonoBehaviour
{
    [Header("Configuraci�n del Chip")]
    public EnemyType type;

    [Header("Estad�sticas")]
    public float detectionRange = 15f;
    public float loseTargetRange = 25f;
    public float attackRange = 1.5f; // Acercado para evitar golpes a distancia
    public float attackCooldown = 1.5f;
    public int damage = 10;

    [Header("Referencias (Dron)")]
    public GameObject projectilePrefab;
    public Transform shootPoint;

    [Header("Animación")]
    public Animator animator;

    // Propiedades para los m�dulos
    public NavMeshAgent Agent { get; private set; }
    public HealthSystem Health { get; private set; }
    public Transform PlayerTarget { get; private set; }

    // M�dulos
    private EnemyMovement movement;
    private IEnemyCombat combatModule;
    private float stunTimer = 0f;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Health = GetComponent<HealthSystem>();

        // Inyectamos los m�dulos
        movement = new EnemyMovement(this);

        // Selecci�n de "Chip" de combate
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

        if (Agent != null) 
        {
            Agent.speed = 1.8f; // Velocidad que concuerda con la animación de caminar (evita deslizamiento)
            Agent.stoppingDistance = attackRange - 0.2f;
            
            // Intenta asegurarse de que el agente esta pegado al NavMesh
            if (!Agent.isOnNavMesh)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
                {
                    Agent.Warp(hit.position);
                }
                else
                {
                    Debug.LogWarning($"El enemigo {gameObject.name} no está en un NavMesh y no puede moverse. ¡Asegúrate de haber horneado el NavMesh en esta superficie plana!");
                }
            }
        }
        
        if (animator == null) animator = GetComponentInChildren<Animator>();
        
        // Desactivar Root Motion para evitar que las animaciones desplacen al personaje independientemente del NavMeshAgent
        if (animator != null) animator.applyRootMotion = false;
    }

    private void Update()
    {
        if (PlayerTarget == null) return;
        
        if (Health.IsDead)
        {
            if (animator != null) animator.SetBool("Dead", true);
            if (Agent != null && Agent.isOnNavMesh) Agent.isStopped = true;
            return;
        }

        float dt = Time.deltaTime;

        if (stunTimer > 0)
        {
            stunTimer -= dt;
            Agent.isStopped = true;
            UpdateAnimations();
            return;
        }

        movement.Tick(dt);
        combatModule.UpdateCombat(dt);
        
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        if (animator != null && Agent != null)
        {
            animator.SetFloat("Speed", Agent.velocity.magnitude);
        }
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
}