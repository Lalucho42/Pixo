using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 12f;

    [Header("Configuracion de Habilidades")]
    public float rollCooldown = 2f;
    public float rollImpulseSpeed = 8f; // Velocidad del impulso al rodar

    [Header("Configuracion de Camara")]
    public Transform cameraFollowTarget;
    public float cameraSensitivity = 1.5f;
    public float cameraClampMin = -30f;
    public float cameraClampMax = 40f;

    public CharacterController Controller { get; private set; }
    public Animator Animator { get; private set; }

    public PlayerInputHandler InputHandler { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerAnimations Animations { get; private set; }
    public PlayerCamera PlayerCamera { get; private set; }

    // --- NUEVA VARIABLE CONTROLADA POR LA ANIMACI�N ---
    public bool ApplyRollImpulse { get; private set; }

    private void Awake()
    {
        if (!CompareTag("Player")) gameObject.tag = "Player";
        Controller = GetComponent<CharacterController>();
        Animator = GetComponentInChildren<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InputHandler = new PlayerInputHandler();
        Movement = new PlayerMovement(this);
        Animations = new PlayerAnimations(this);
        PlayerCamera = new PlayerCamera(this, cameraFollowTarget, cameraClampMin, cameraClampMax);
    }

    private void OnEnable()
    {
        InputHandler.Enable();
    }

    private void OnDisable()
    {
        InputHandler.Disable();
    }

    private void Update()
    {
        // 1. Primero movemos y rotamos al personaje
        Movement.Tick(Time.deltaTime);

        // 2. Despu�s animamos
        Animations.Tick(Time.deltaTime);

        // 3. Y por �LTIMO en este mismo frame, rotamos el Target de la c�mara
        if (PlayerCamera != null)
        {
            PlayerCamera.Tick(Time.deltaTime);
        }
    }

    // �BORRAMOS la funci�n LateUpdate por completo, ya no la necesitamos!

    private void LateUpdate()
    {
        if (PlayerCamera != null)
        {
            PlayerCamera.Tick(Time.deltaTime);
        }
    }

    // --- NUEVA FUNCI�N QUE LLAMA EL SCRIPT PUENTE ---
    public void SetRollImpulse(bool active)
    {
        ApplyRollImpulse = active;
    }
}
