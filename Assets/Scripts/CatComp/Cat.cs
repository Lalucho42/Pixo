using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Cat : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public Transform[] waypoints;

    [Header("Configuracion del Guia")]
    public float distanciaMaxima = 8f;
    public float distanciaAlPunto = 1.5f;
    public float distanciaParaAvanzar = 3f;

    [Header("Configuracion de Huida")]
    public float radioDeteccion = 8f;
    public float distanciaHuida = 5f;

    [Header("Configuracion de Salto")]
    public float alturaSalto = 2f;
    public float duracionSalto = 0.6f;
    public bool estaSaltando = false;

    // --- NUEVO: CANDADO DE LA TRAMPA ---
    public bool isTrapped = false;

    public NavMeshAgent Agent { get; private set; }
    public Animator Anim { get; private set; }

    public CatJump Jump { get; private set; }
    public CatEvasion Evasion { get; private set; }
    public CatMovement Movement { get; private set; }
    public CatAnimations Animations { get; private set; }

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
        if (player == null) return;

        // --- EL ARREGLO ESTÁ ACÁ ---
        if (isTrapped)
        {
            // Si el Director tomó el control, el gato NO se frena a sí mismo.
            // Solo actualiza sus animaciones (para que mueva las patas si el Director lo hace correr).
            Animations.Tick();
            return; // Cortamos acá para que no use su lógica normal de guía o huida
        }

        // Su lógica normal cuando es libre
        if (Jump.Tick()) { Animations.Tick(); return; }
        if (Evasion.Tick()) { Animations.Tick(); return; }

        Movement.Tick();
        Animations.Tick();
    }
}