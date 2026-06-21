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
}