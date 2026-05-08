using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Cat : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public Transform[] waypoints;

    [Header("Configuracion")]
    public float distanciaAlPunto = 1.2f;
    public float distanciaParaAvanzar = 3.5f;
    public bool seguirJugador = false;

    [Header("Estados")]
    public bool esperandoAlJugador = false;
    public bool estaSentado = false;
    public bool bloqueadoPorAnimacion = false;
    public bool estaGirando = false;
    public bool isTrapped = false;

    [Header("Config Salto")]
    public float alturaSalto = 2f;
    public float duracionSalto = 0.6f;
    public bool estaSaltando = false;
    public float radioDeteccion = 8f;
    public float distanciaHuida = 5f;

    public NavMeshAgent Agent { get; private set; }
    public Animator Anim { get; private set; }

    public CatMovement Movement { get; private set; }
    public CatAnimations Animations { get; private set; }
    public CatJump Jump { get; private set; }
    public CatEvasion Evasion { get; private set; }

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Agent.autoTraverseOffMeshLink = false;
        Anim = GetComponentInChildren<Animator>();

        Jump = new CatJump(this);
        Evasion = new CatEvasion(this);
        Movement = new CatMovement(this);
        Animations = new CatAnimations(this);
    }

    private void Update()
    {
        if (player == null || isTrapped) return;
        if (Jump.Tick()) { Animations.Tick(); return; }
        if (Evasion.Tick()) { Animations.Tick(); return; }
        Movement.Tick();
        Animations.Tick();
    }

    public void EventoFinalizarLevantado()
    {
        bloqueadoPorAnimacion = false;
        estaSentado = false;
        if (Agent != null) Agent.isStopped = false;
    }

    public void EventoFinalizarGiro()
    {
        estaGirando = false;
        if (Agent != null) Agent.isStopped = false;
    }
}