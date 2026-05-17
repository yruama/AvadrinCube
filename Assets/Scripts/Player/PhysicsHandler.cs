using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[DefaultExecutionOrder(-200)]
public class PlayerPhysicsHandler : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private PlayerGameplayConfig _config;

    [Header("Sol (override si pas de config)")]
    [SerializeField] private float _raycastDistance = 0.6f;
    [SerializeField] private float _raycastOriginHeight = 0.2f;

    [Header("Mouvement (override)")]
    [SerializeField] private float _groundAcceleration = 80f;
    [SerializeField] private float _groundDeceleration = 90f;
    [SerializeField] private float _airAcceleration = 40f;
    [SerializeField] private float _airDeceleration = 30f;
    [SerializeField] private float _maxAirSpeedMultiplier = 1.1f;

    [Header("Gravité (override)")]
    [SerializeField] private float _gravityMultiplier = 10f;
    [SerializeField] private float _groundStickForce = 2f;

    [Header("Plateformes (override)")]
    [SerializeField] private float _platformCheckDistance = 0.8f;
    [SerializeField] private bool _inheritPlatformVelocityOnJump = true;
    [SerializeField] private float _platformVelocityInheritance = 1f;

    private PlayerController _playerController;
    private Vector3 _horizontalVelocity;
    private Vector3 _desiredHorizontalVelocity;
    private float _verticalVelocity;
    private PlatformeController _ridingPlatform;
    private PlatformeController _registeredPlatform;
    private bool _isGrounded;
    private bool _wasGroundedLastFrame;

    void Awake()
    {
        if (_controller == null)
            _controller = GetComponent<CharacterController>();

        _playerController = GetComponent<PlayerController>();
        ApplyConfig();

        if (_groundLayer.value == 0)
            _groundLayer = GameConstants.GroundAndPlatformMask;
        else
            _groundLayer |= (1 << GameConstants.LAYER_GROUND);
    }

    void OnDisable() => UnregisterFromPlatform();

    void FixedUpdate() => ApplyPhysics(Time.fixedDeltaTime);

    public void SetDesiredHorizontalVelocity(Vector3 velocity) => _desiredHorizontalVelocity = velocity;
    public void SetVerticalVelocity(float velocity) => _verticalVelocity = velocity;

    public void DetachFromPlatform(bool inheritVelocity = true)
    {
        if (_registeredPlatform != null && inheritVelocity && _inheritPlatformVelocityOnJump)
        {
            Vector3 v = _registeredPlatform.Velocity * _platformVelocityInheritance;
            _horizontalVelocity += new Vector3(v.x, 0f, v.z);
        }
        UnregisterFromPlatform();
    }

    private void ApplyConfig()
    {
        if (_config == null)
            return;

        _groundAcceleration = _config.groundAcceleration;
        _groundDeceleration = _config.groundDeceleration;
        _airAcceleration = _config.airAcceleration;
        _airDeceleration = _config.airDeceleration;
        _gravityMultiplier = _config.gravityMultiplier;
        _groundStickForce = _config.groundStickForce;
        _platformCheckDistance = _config.platformCheckDistance;
    }

    private void UnregisterFromPlatform()
    {
        if (_registeredPlatform != null)
        {
            _registeredPlatform.SetPassenger(_controller, false);
            _registeredPlatform = null;
        }
        _ridingPlatform = null;
    }

    public void ApplyPhysics(float deltaTime)
    {
        _wasGroundedLastFrame = _isGrounded;
        UpdateGroundDetection();
        UpdatePlatformRegistration();
        ApplyGravity(deltaTime);
        UpdateHorizontalVelocity(deltaTime);
        ApplyMovement(deltaTime);
    }

    private void UpdateGroundDetection()
    {
        Vector3 origin = transform.position + Vector3.up * _raycastOriginHeight;
        float distance = _raycastDistance + _raycastOriginHeight;
        _isGrounded = false;

        Vector3[] offsets =
        {
            Vector3.zero,
            new Vector3(0.25f, 0f, 0.25f),
            new Vector3(-0.25f, 0f, 0.25f),
            new Vector3(0.25f, 0f, -0.25f),
            new Vector3(-0.25f, 0f, -0.25f),
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            if (Physics.Raycast(origin + offsets[i], Vector3.down, distance, _groundLayer))
            {
                _isGrounded = true;
                return;
            }
        }

        if (_controller.isGrounded)
            _isGrounded = true;
    }

    private void UpdatePlatformRegistration()
    {
        PlatformeController support = FindSupportingPlatform();
        _ridingPlatform = support;

        if (_registeredPlatform == support)
            return;

        if (_registeredPlatform != null)
            _registeredPlatform.SetPassenger(_controller, false);

        _registeredPlatform = support;

        if (_registeredPlatform != null)
            _registeredPlatform.SetPassenger(_controller, true);
    }

    private PlatformeController FindSupportingPlatform()
    {
        if (_verticalVelocity > 1f || (!_isGrounded && !_controller.isGrounded))
            return null;

        Vector3 origin = transform.position + Vector3.up * _raycastOriginHeight;
        if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, _platformCheckDistance, _groundLayer))
            return null;

        if (hit.normal.y < 0.5f)
            return null;

        MovingPlatformSurface surface = hit.collider.GetComponent<MovingPlatformSurface>();
        if (surface == null)
            surface = hit.collider.GetComponentInParent<MovingPlatformSurface>();

        return surface?.Platform;
    }

    private void ApplyGravity(float deltaTime)
    {
        if (_isGrounded && _verticalVelocity <= 0f)
            _verticalVelocity = -_groundStickForce;
        else
            _verticalVelocity -= GameConstants.GRAVITY_MULTIPLIER * _gravityMultiplier * deltaTime;
    }

    private void UpdateHorizontalVelocity(float deltaTime)
    {
        Vector3 current = new Vector3(_horizontalVelocity.x, 0f, _horizontalVelocity.z);
        Vector3 target = new Vector3(_desiredHorizontalVelocity.x, 0f, _desiredHorizontalVelocity.z);

        float rate = target.sqrMagnitude > 0.01f
            ? (_isGrounded ? _groundAcceleration : _airAcceleration)
            : (_isGrounded ? _groundDeceleration : _airDeceleration);

        Vector3 next = Vector3.MoveTowards(current, target.sqrMagnitude > 0.01f ? target : Vector3.zero, rate * deltaTime);

        if (target.sqrMagnitude > 0.01f)
        {
            float max = target.magnitude * (_isGrounded ? 1f : _maxAirSpeedMultiplier);
            if (next.sqrMagnitude > max * max)
                next = next.normalized * max;
        }

        _horizontalVelocity = next;
    }

    private void ApplyMovement(float deltaTime)
    {
        _controller.Move(_horizontalVelocity * deltaTime);
        CollisionFlags flags = _controller.Move(Vector3.up * _verticalVelocity * deltaTime);
        if ((flags & CollisionFlags.Above) != 0 && _verticalVelocity > 0f)
            _verticalVelocity = 0f;
    }

    public bool IsGrounded => _isGrounded;
    public bool WasGroundedLastFrame => _wasGroundedLastFrame;
    public float VerticalVelocity => _verticalVelocity;
    public bool IsOnPlatform => _ridingPlatform != null;
}
