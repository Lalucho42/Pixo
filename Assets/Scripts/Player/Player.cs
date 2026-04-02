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
    public float rollColliderHeight = 1f;
    public Vector3 rollColliderCenter = new Vector3(0f, 0.5f, 0f);
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;

    // Propiedades de Componentes
    public CharacterController Controller { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerWeaponManager WeaponManager { get; private set; }

    // Módulos
    public PlayerInputHandler InputHandler { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerJump Jump { get; private set; }
    public PlayerCombat Combat { get; private set; }
    public PlayerInteract Interact { get; private set; }
    public PlayerAnimations Animations { get; private set; }
    public PlayerCamera PlayerCamera { get; private set; }

    // Estados
    public static bool IsDead = false;
    public bool ApplyRollImpulse { get; private set; }
    public bool IsMovementLocked { get; set; } // Nueva llave para bloquear movimiento

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
        Animator = GetComponentInChildren<Animator>();
        WeaponManager = GetComponent<PlayerWeaponManager>();

        originalColliderHeight = Controller.height;
        originalColliderCenter = Controller.center;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InputHandler = new PlayerInputHandler();
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

    public void SetRollImpulse(bool active)
    {
        ApplyRollImpulse = active;
        if (Controller != null)
        {
            Controller.height = active ? rollColliderHeight : originalColliderHeight;
            Controller.center = active ? rollColliderCenter : originalColliderCenter;
        }
    }
}