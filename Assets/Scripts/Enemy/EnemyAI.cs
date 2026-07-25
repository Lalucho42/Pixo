using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(HealthSystem))]
[RequireComponent(typeof(AudioSource))]
public class EnemyAI : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    public float detectionRange = 15f;
    public float loseTargetRange = 25f;
    public float attackRange = 1.6f;
    public float walkSpeed = 1.5f;

    [Header("Configuracion Combate")]
    public float attackCooldown = 1.5f;
    public int damage = 10;
    public float tiempoAnimacionMuerte = 60f;

    [Header("Referencias Auto-detectables")]
    public EnemyHandDamage handDamageScript;
    public Animator animator;

    [Header("Control de Cinematicas")]
    public bool isInCinematic = false;

    [Header("Configuracion de Drops Colectables")]
    public GameObject dropPrefab;
    public int dropAmount = 1;
    public Transform spawnPoint;

    public NavMeshAgent Agent { get; private set; }
    public HealthSystem Health { get; private set; }
    public Transform PlayerTarget { get; private set; }
    public AudioSource AudioSource { get; private set; }

    private EnemyMovement movement;
    private MeleeCombatModule combatModule;
    private float stunTimer = 0f;
    private bool yaDropeo = false;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Health = GetComponent<HealthSystem>();
        AudioSource = GetComponent<AudioSource>();

        AudioSource.spatialBlend = 1f;
        AudioSource.dopplerLevel = 0f;
        AudioSource.minDistance = 2f;
        AudioSource.maxDistance = 30f;
        AudioSource.rolloffMode = AudioRolloffMode.Linear;

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

        if (AudioManager.Instance != null && AudioManager.Instance.grupoSFX != null)
        {
            AudioSource.outputAudioMixerGroup = AudioManager.Instance.grupoSFX;
        }
    }

    private void Update()
    {
        if (PlayerTarget == null || Health.IsDead)
        {
            if (Health.IsDead)
            {
                ManejarMuerteYDrop();
            }
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

    private void ManejarMuerteYDrop()
    {
        if (yaDropeo) return;
        yaDropeo = true;

        DesactivarDañoMano();

        if (Agent != null)
        {
            if (Agent.isOnNavMesh) Agent.isStopped = true;
            Agent.enabled = false;
        }

        Collider[] todosLosColliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in todosLosColliders)
        {
            c.enabled = false;
        }

        if (animator != null)
        {
            animator.ResetTrigger("Punch");
            animator.ResetTrigger("Hit");
            animator.SetBool("Die", true);
        }

        if (dropPrefab != null)
        {
            Vector3 spawnOrigin = spawnPoint != null ? spawnPoint.position : transform.position;
            for (int i = 0; i < dropAmount; i++)
            {
                Instantiate(dropPrefab, spawnOrigin + randomOrigin(), Quaternion.identity);
            }
        }

        Destroy(gameObject, tiempoAnimacionMuerte);
    }

    private Vector3 randomOrigin()
    {
        return new Vector3(Random.Range(-0.3f, 0.3f), 0.2f, Random.Range(-0.3f, 0.3f));
    }

    public void EjecutarEfectosRecibirDaño(Vector3 posicionImpacto)
    {
        if (Health.IsDead) return;

        DesactivarDañoMano();

        if (Agent != null && Agent.isOnNavMesh)
        {
            Agent.isStopped = true;
        }

        if (animator != null)
        {
            animator.ResetTrigger("Punch");
            animator.SetTrigger("Hit");
        }

        if (AudioManager.Instance != null && AudioSource != null)
        {
            AudioManager.Instance.PlaySFX3D("Impacto_Enemigo", AudioSource);
        }
    }

    public void PlayEnemyFootstep()
    {
        if (Health.IsDead || AudioManager.Instance == null || AudioSource == null) return;

        bool enCinematica = isInCinematic || !Agent.enabled || !Agent.isOnNavMesh;

        if (!enCinematica)
        {
            if (Agent.isStopped || Agent.velocity.magnitude < 0.15f) return;
        }

        AudioManager.Instance.PlaySFX3D("Enemigo_Paso", AudioSource);
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

        if (AudioManager.Instance != null && AudioSource != null && !Health.IsDead)
        {
            AudioManager.Instance.PlaySFX3D("Enemigo_Ataque", AudioSource);
        }
    }

    public void DesactivarDañoMano()
    {
        if (handDamageScript != null) handDamageScript.SetDamageState(false);
    }
}