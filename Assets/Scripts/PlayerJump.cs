using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpHeightIdle    = 1.0f;
    [SerializeField] private float jumpHeightWalking = 1.3f;
    [SerializeField] private float jumpHeightRunning = 1.8f;
    [SerializeField] private float gravity = 18f;
    [SerializeField] private float fallGravityMultiplier = 1.3f;
    [SerializeField] private float lowJumpGravityMultiplier = 1.8f;
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float fallDeathVelocity = 22f;

    private CharacterController _cc;
    private Player _player;
    private float _verticalVelocity;
    private bool  _isGrounded;
    private float _coyoteTimer;
    private float _jumpBufferTimer;
    private bool _isJumping;
    private bool _jumpKeyHeld;
    private float _peakFallVelocity = 0f;

    public bool IsGrounded => _isGrounded;
    public float VerticalVelocity => _verticalVelocity;
    public bool IsJumping => _isJumping;

    private void Awake()
    {
        _cc     = GetComponent<CharacterController>();
        _player = GetComponent<Player>();
        if (groundCheck == null) groundCheck = transform;
    }

    private void Start()
    {
        if (_player != null && _player.InputHandler != null)
        {
            _player.InputHandler.OnJumpEvent += TriggerJumpBuffer;
        }
    }

    private void OnDestroy()
    {
        if (_player != null && _player.InputHandler != null)
        {
            _player.InputHandler.OnJumpEvent -= TriggerJumpBuffer;
        }
    }

    private void TriggerJumpBuffer()
    {
        _jumpBufferTimer = jumpBufferTime;
    }

    private void Update()
    {
        if (GameManager.IsPaused || GameManager.IsDead) return;

        CheckGround();
        UpdateTimers();
        ReadJumpInput();
        TryJump();
        ApplyGravity();

        _cc.Move(Vector3.up * _verticalVelocity * Time.deltaTime);
    }

    private void CheckGround()
    {
        bool wasGrounded = _isGrounded;

        _isGrounded = _cc.isGrounded ||
                      Physics.CheckSphere(
                          groundCheck.position,
                          groundCheckRadius,
                          groundLayer,
                          QueryTriggerInteraction.Ignore);

        if (!_isGrounded && _verticalVelocity < 0f)
        {
            if (_verticalVelocity < _peakFallVelocity)
                _peakFallVelocity = _verticalVelocity;
        }

        if (_isGrounded)
        {
            if (!wasGrounded && _peakFallVelocity < -fallDeathVelocity)
            {
                HealthSystem hs = GetComponent<HealthSystem>();
                if (hs != null) hs.TakeDamage(hs.maxHealth + 1);
            }

            _peakFallVelocity = 0f;

            if (_verticalVelocity < 0f)
                _verticalVelocity = -4f;

            if (!wasGrounded)
                _isJumping = false;

            _coyoteTimer = coyoteTime;
        }
    }

    private void UpdateTimers()
    {
        if (!_isGrounded)
            _coyoteTimer -= Time.deltaTime;

        if (_jumpBufferTimer > 0f)
            _jumpBufferTimer -= Time.deltaTime;
    }

    private void ReadJumpInput()
    {
        if (_player != null && _player.InputHandler != null)
        {
            _jumpKeyHeld = _player.InputHandler.IsJumpHeld;
        }
        else 
        {
            if (Keyboard.current == null) return;
            _jumpKeyHeld = Keyboard.current.spaceKey.isPressed;
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                _jumpBufferTimer = jumpBufferTime;
        }
    }

    private void TryJump()
    {
        bool canJump = _jumpBufferTimer > 0f && (_isGrounded || _coyoteTimer > 0f);
        if (!canJump) return;

        _jumpBufferTimer = 0f;
        _coyoteTimer     = 0f;

        PerformJump();
    }

    private void PerformJump()
    {
        float height = GetJumpHeight();
        _verticalVelocity = Mathf.Sqrt(2f * gravity * height);
        _isJumping        = true;
    }

    private float GetJumpHeight()
    {
        if (Keyboard.current == null) return jumpHeightIdle;

        bool isMoving = Keyboard.current.wKey.isPressed ||
                        Keyboard.current.sKey.isPressed ||
                        Keyboard.current.aKey.isPressed ||
                        Keyboard.current.dKey.isPressed;

        if (!isMoving) return jumpHeightIdle;

        bool isRunning = _player != null && _player.InputHandler.IsRunning;
        return isRunning ? jumpHeightRunning : jumpHeightWalking;
    }

    private void ApplyGravity()
    {
        if (_isGrounded && _verticalVelocity <= 0f) return;

        float gravScale;

        if (_verticalVelocity < 0f)
        {
            gravScale  = fallGravityMultiplier;
            _isJumping = false;
        }
        else if (_isJumping && !_jumpKeyHeld)
        {
            gravScale = lowJumpGravityMultiplier;
        }
        else
        {
            gravScale = 1f;
        }

        _verticalVelocity -= gravity * gravScale * Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

#if UNITY_EDITOR
        UnityEditor.Handles.color = new Color(1f, 1f, 0f, 0.6f);
        UnityEditor.Handles.DrawWireDisc(
            transform.position + Vector3.up * jumpHeightIdle, Vector3.up, 0.25f);

        UnityEditor.Handles.color = new Color(0f, 1f, 1f, 0.6f);
        UnityEditor.Handles.DrawWireDisc(
            transform.position + Vector3.up * jumpHeightRunning, Vector3.up, 0.25f);
#endif
    }
}
