using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("--- Datos de Movimiento ---")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rollImpulseSpeed = 8f;
    public float rotationSpeed = 12f;

    [Header("--- Datos de Salto y Fisica ---")]
    public float jumpHeightIdle = 1.5f;
    public float gravity = 15f;

    [Header("--- Datos de Camara ---")]
    public Transform cameraFollowTarget;
    public float cameraSensitivity = 1.5f;
    public float cameraClampMin = -30f;
    public float cameraClampMax = 40f;

    [Header("--- Datos de Combate ---")]
    public Transform attackPoint;
    public float stunDuration = 0.5f;

    [Header("--- Datos de Rodar (Collider) ---")]
    [Range(0.1f, 0.9f)] public float rollHeightMultiplier = 0.5f; // 0.5 = se achica a la mitad

    // Propiedades de Componentes
    public CharacterController Controller { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerWeaponManager WeaponManager { get; private set; }

    // Módulos (Nuestra arquitectura limpia)
    public PlayerInputHandler InputHandler { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerJump Jump { get; private set; }
    public PlayerCombat Combat { get; private set; }
    public PlayerInteract Interact { get; private set; }
    public PlayerAnimations Animations { get; private set; }
    public PlayerCamera PlayerCamera { get; private set; }
    public PlayerColliderHandler ColliderHandler { get; private set; } // <-- Nuevo módulo

    // Estados
    public static bool IsDead = false;
    public bool IsMovementLocked { get; set; }

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
        Animator = GetComponentInChildren<Animator>();
        WeaponManager = GetComponent<PlayerWeaponManager>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Inicializamos todos los módulos
        InputHandler = new PlayerInputHandler();
        ColliderHandler = new PlayerColliderHandler(Controller, rollHeightMultiplier); // <-- Instanciado
        Movement = new PlayerMovement(this, walkSpeed, runSpeed, rollImpulseSpeed);
        Jump = new PlayerJump(this);
        Combat = new PlayerCombat(this);
        Interact = new PlayerInteract(this);
        Animations = new PlayerAnimations(this);
        PlayerCamera = new PlayerCamera(this, cameraFollowTarget, cameraSensitivity, cameraClampMin, cameraClampMax);

        InputHandler.OnScrollEvent += HandleWeaponScroll;
        IsDead = false;
    }

    private void OnEnable() { if (InputHandler != null) InputHandler.Enable(); }
    private void OnDisable() { if (InputHandler != null) InputHandler.Disable(); }

    private void Update()
    {
        if (IsDead || GameManager.IsPaused || GameManager.IsDead) return;

        float dt = Time.deltaTime;
        Movement.Tick(dt);
        Jump.Tick(dt);
        Combat.Tick(dt);
        Interact.Tick(dt);
        Animations.Tick(dt);
        if (PlayerCamera != null) PlayerCamera.Tick(dt);
    }

    private void HandleWeaponScroll(float scrollValue)
    {
        if (WeaponManager != null) WeaponManager.CycleWeapon(scrollValue);
    }
}