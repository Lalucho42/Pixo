using UnityEngine;
using UnityEngine.AI;

public class Cat : MonoBehaviour
{
    // El único origen de la verdad para la IA
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

    public NavMeshAgent Agent { get; private set; }
    public Animator Anim { get; private set; }

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

        Jump = new CatJump(this);
        Evasion = new CatEvasion(this);
        Movement = new CatMovement(this);
        Animations = new CatAnimations(this);
    }

    private void Update()
    {
        if (player == null) return;

        if (isTrapped)
        {
            Animations.Tick();
            return;
        }

        // FAIL-SAFE PROFESIONAL: Solo corre si el estado se quedó trabado en "StandingUp"
        if (estadoActual == CatState.StandingUp)
        {
            timerSeguridadBloqueo += Time.deltaTime;
            if (timerSeguridadBloqueo > 2.0f) // 2 segundos es un margen seguro para cualquier transición
            {
                Debug.LogWarning("[IA Gato] Fail-safe activado: Forzando estado Moving.");
                EventoFinalizarLevantado();
            }
        }
        else
        {
            timerSeguridadBloqueo = 0f;
        }

        // Módulos de acción prioritaria
        if (Jump.Tick()) { Animations.Tick(); return; }
        if (Evasion.Tick()) { Animations.Tick(); return; }

        Movement.Tick();
        Animations.Tick();
    }

    // Tu Animation Event de confianza
    public void EventoFinalizarLevantado()
    {
        if (estadoActual == CatState.StandingUp)
        {
            estadoActual = CatState.Moving;
            if (Agent != null) Agent.isStopped = false;
        }
    }
}