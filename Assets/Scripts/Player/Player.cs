using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 12f;
    public float rollCooldown = 2f;
    public float rollImpulseSpeed = 8f;
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
    public static bool IsDead = false;
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

        IsDead = false;
        CheckpointManager.Register(transform.position, transform.rotation);

        if (GameManager.instance == null)
        {
            GameObject gmObj = new GameObject("GameManager");
            gmObj.AddComponent<GameManager>();
            DontDestroyOnLoad(gmObj);
        }
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
        if (IsDead || GameManager.IsPaused || GameManager.IsDead) return;

        Movement.Tick(Time.deltaTime);
        Animations.Tick(Time.deltaTime);

        if (PlayerCamera != null)
            PlayerCamera.Tick(Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (IsDead || GameManager.IsPaused || GameManager.IsDead) return;
        if (PlayerCamera != null)
            PlayerCamera.Tick(Time.deltaTime);
    }

    public void SetRollImpulse(bool active)
    {
        ApplyRollImpulse = active;
    }
}
