using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class Cat : MonoBehaviour
{
    public enum CatState { Moving, Waiting, Sitting, StandingUp }

    [Header("Referencias")]
    public Transform player;
    public Transform[] waypoints;

    [Header("Configuracion del Guia")]
    public float distanciaAlPunto = 1.2f;
    public float distanciaParaAvanzar = 3.5f;
    public bool seguirJugador = false;

    [Header("Configuracion de Huida/Salto")]
    public float radioDeteccion = 8f;
    public float distanciaHuida = 5f;
    public float alturaSalto = 2f;
    public float duracionSalto = 0.6f;
    public bool estaSaltando = false;

    [Header("Estado Actual de la IA")]
    public CatState estadoActual = CatState.Moving;
    public bool isTrapped = false;

    [Header("Control de Cinematicas")]
    public bool isInCinematic = false;

    [Header("Control Externo (Orquestadores, ej: TutorialTrap)")]
    [Tooltip("Cuando es true, el gato ignora por completo su IA autonoma (CatMovement y CatEvasion: seguir/esperar al jugador, sentarse cerca de el, huir de enemigos). Un script externo controla el destino del NavMeshAgent directamente. Se activa/desactiva con IniciarControlExterno() / DetenerControlExterno().")]
    public bool invalidarSeguimiento = false;

    [Header("Ajuste de Audio Pasos")]
    public float cooldownPasosGato = 0.24f;
    private float ultimoTiempoPasoGato = 0f;

    public NavMeshAgent Agent { get; private set; }
    public Animator Anim { get; private set; }
    public AudioSource AudioSource { get; private set; }

    public CatJump Jump { get; private set; }
    public CatEvasion Evasion { get; private set; }
    public CatMovement Movement { get; private set; }
    public CatAnimations Animations { get; private set; }

    private float timerSeguridadBloqueo = 0f;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Agent.autoTraverseOffMeshLink = false;
        Agent.updateRotation = true;
        Anim = GetComponentInChildren<Animator>();
        AudioSource = GetComponent<AudioSource>();

        AudioSource.spatialBlend = 1f;
        AudioSource.dopplerLevel = 0f;
        AudioSource.minDistance = 2f;
        AudioSource.maxDistance = 25f;
        AudioSource.rolloffMode = AudioRolloffMode.Linear;

        Jump = new CatJump(this);
        Evasion = new CatEvasion(this);
        Movement = new CatMovement(this);
        Animations = new CatAnimations(this);
    }

    private void Start()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.grupoSFX != null)
        {
            AudioSource.outputAudioMixerGroup = AudioManager.Instance.grupoSFX;
        }
    }

    private void Update()
    {
        if (player == null) return;

        if (isTrapped)
        {
            Animations.Tick();
            return;
        }

        if (invalidarSeguimiento)
        {
            // Un orquestador externo (ej: TutorialTrap) tiene el control total de la
            // navegacion. Ignoramos CatMovement (seguir/esperar al jugador) y CatEvasion
            // (huida de enemigos) para que no "secuestren" el destino frame a frame.
            // Conservamos CatJump para que un OffMeshLink en el camino al destino externo
            // siga animandose como un salto en vez de romperse, y CatAnimations para que
            // el blend tree de locomocion siga recibiendo la velocidad real del Agent.
            if (Jump.Tick())
            {
                Animations.Tick();
                return;
            }

            Animations.Tick();
            return;
        }

        if (estadoActual == CatState.StandingUp)
        {
            timerSeguridadBloqueo += Time.deltaTime;
            if (timerSeguridadBloqueo > 2.0f)
            {
                EventoFinalizarLevantado();
            }
        }
        else
        {
            timerSeguridadBloqueo = 0f;
        }

        if (Jump.Tick()) { Animations.Tick(); return; }
        if (Evasion.Tick()) { Animations.Tick(); return; }

        Movement.Tick();
        Animations.Tick();
    }

    public void PlayCatFootstep()
    {
        if (isTrapped) return;
        if (AudioManager.Instance == null || AudioSource == null) return;
        if (Time.time < ultimoTiempoPasoGato + cooldownPasosGato) return;

        bool enCinematica = isInCinematic || !Agent.enabled || !Agent.isOnNavMesh;

        if (!enCinematica)
        {
            if (estaSaltando || estadoActual != CatState.Moving || Agent.velocity.magnitude < 0.15f) return;
        }

        ultimoTiempoPasoGato = Time.time;
        AudioManager.Instance.PlaySFX3D("Gato_Paso", AudioSource);
    }

    public void EventoFinalizarLevantado()
    {
        if (estadoActual == CatState.StandingUp)
        {
            estadoActual = CatState.Moving;
            if (Agent != null) Agent.isStopped = false;
        }
    }

    /// <summary>
    /// Entrega el control de la navegacion a un script externo (orquestador).
    /// Desactiva la IA autonoma (CatMovement / CatEvasion) y manda al gato directo
    /// al destino indicado usando el NavMeshAgent, con animacion de carrera normal.
    /// </summary>
    public void IniciarControlExterno(Vector3 destino)
    {
        enabled = true;
        isTrapped = false;
        invalidarSeguimiento = true;
        estadoActual = CatState.Moving;
        estaSaltando = false;

        if (Anim != null)
        {
            // Igual que en CatMovement: para salir de la animacion de Sentado hay
            // que disparar el Trigger, no alcanza con apagar el bool IsSitting.
            Anim.SetTrigger("StandUp");
            Anim.SetBool("IsSitting", false);
        }

        if (Agent != null && Agent.isActiveAndEnabled && Agent.isOnNavMesh)
        {
            Agent.isStopped = false;
            Agent.updateRotation = true;
            Agent.SetDestination(destino);
        }
    }

    /// <summary>
    /// Frena al gato en el lugar. Sigue bajo control externo (no se reactiva la IA
    /// autonoma): pensado para el final de una secuencia guiada por un orquestador.
    /// </summary>
    public void DetenerControlExterno(bool sentarse)
    {
        if (Agent != null && Agent.isActiveAndEnabled && Agent.isOnNavMesh)
        {
            Agent.isStopped = true;
            Agent.velocity = Vector3.zero;
            Agent.ResetPath();
        }

        estadoActual = sentarse ? CatState.Waiting : CatState.Moving;

        if (Anim != null)
        {
            Anim.SetBool("IsSitting", sentarse);
        }
    }
}