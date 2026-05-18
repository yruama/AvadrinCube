using UnityEngine;

[RequireComponent(typeof(CharacterController))]
/// <summary>
/// Gère la physique du joueur : détection du sol, gravité, mouvement appliqué au CharacterController
/// et attachement/détachement aux plateformes mobiles.
/// Lit la vitesse désirée via <see cref="SetDesiredHorizontalVelocity"/> et applique le mouvement en FixedUpdate().
/// </summary>
public class PlayerPhysicsHandler : MonoBehaviour
{
    private CharacterController _controller;
    private PlayerController _playerController;
    [SerializeField] private PlayerGameplayConfig _gameplayConfig;
    [SerializeField] private float _gravityMultiplier = GameConstants.GRAVITY_MULTIPLIER;

    // Movement
    private Vector3 _desiredHorizontalVelocity = Vector3.zero;
    private float _verticalVelocity = 0f;

    // Ground detection
    private bool _isGrounded = false;
    private bool _wasGroundedLastFrame = false;

    // Platform attachment
    private PlatformeController _currentPlatform;

    [SerializeField] private float _groundCheckDistance = 0.15f;
    [SerializeField] private LayerMask _groundLayer;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _playerController = GetComponent<PlayerController>();

        if (_playerController != null)
            _groundLayer = _playerController.GroundLayer;

        if (_groundLayer == 0)
            _groundLayer = GameConstants.GroundAndPlatformMask;

        if (_controller == null)
        {
            Debug.LogError("PlayerPhysicsHandler requiert un CharacterController sur le GameObject.");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        if (_gameplayConfig == null)
            _gameplayConfig = GetComponent<PlayerMovement>()?.GameplayConfig;

        if (_gameplayConfig != null)
            _gravityMultiplier = Mathf.Max(0.1f, _gameplayConfig.gravityMultiplier);
        else
            Debug.LogWarning("PlayerPhysicsHandler: no PlayerGameplayConfig found, using default gravity multiplier.");
    }

    void OnEnable()
    {
        // nothing for now
    }

    void OnDisable()
    {
        DetachFromPlatform(false);
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        _wasGroundedLastFrame = _isGrounded;
        UpdateGroundDetection();
        ApplyGravity(dt);
        ApplyMovement(dt);
    }

    private void UpdateGroundDetection()
    {
        _wasGroundedLastFrame = _isGrounded;

        bool hitGround = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _controller.height * 0.5f + _groundCheckDistance, _groundLayer, QueryTriggerInteraction.Ignore);
        bool platformSurface = DetectPlatformSurface();
        _isGrounded = _controller.isGrounded || hitGround || platformSurface;

        if (!_isGrounded || (!platformSurface && _currentPlatform != null))
        {
            DetachFromPlatform(false);
        }
    }

    private bool DetectPlatformSurface()
    {
        if (_controller == null)
            return false;

        float checkDistance = Mathf.Max(_groundCheckDistance, 0.05f);
        float sphereRadius = Mathf.Clamp(_controller.radius * 0.9f, 0.05f, _controller.radius);
        Vector3 sphereCenter = transform.position + Vector3.down * (_controller.height * 0.5f - sphereRadius);
        Vector3 overlapCenter = sphereCenter + Vector3.down * checkDistance;

        Collider[] hits = Physics.OverlapSphere(overlapCenter, sphereRadius, _groundLayer, QueryTriggerInteraction.Ignore);
        foreach (Collider hit in hits)
        {
            MovingPlatformSurface surface = hit.GetComponent<MovingPlatformSurface>() ?? hit.GetComponentInParent<MovingPlatformSurface>();
            if (surface != null && surface.Platform != null)
            {
                AttachPlatformSurface(surface);
                return true;
            }
        }

        return false;
    }

    private void AttachPlatformSurface(MovingPlatformSurface surface)
    {
        if (surface == null || surface.Platform == null)
            return;

        if (_currentPlatform != surface.Platform)
        {
            _currentPlatform?.SetPassenger(_controller, false);
            _currentPlatform = surface.Platform;
            _currentPlatform.SetPassenger(_controller, true);
        }
    }

    private float GravityMultiplier => _gameplayConfig != null ? Mathf.Max(0.1f, _gameplayConfig.gravityMultiplier) : Mathf.Max(0.1f, _gravityMultiplier);

    private void ApplyGravity(float dt)
    {
        float gravity = GravityMultiplier;
        if (_isGrounded && _verticalVelocity < 0f)
            _verticalVelocity = 0f;
        else
            _verticalVelocity -= gravity * dt;
    }

    private void ApplyMovement(float dt)
    {
        Vector3 move = _desiredHorizontalVelocity;
        move.y = _verticalVelocity;
        if (_controller != null)
            _controller.Move(move * dt);
    }

    // Public API used by other components
    /// <summary>
    /// Définit la vitesse horizontale désirée (X,Z) que la physique appliquera au prochain FixedUpdate.
    /// </summary>
    public void SetDesiredHorizontalVelocity(Vector3 v) => _desiredHorizontalVelocity = v;

    /// <summary>
    /// Définit la composante verticale de la vélocité (utilisé par les sauts).
    /// </summary>
    public void SetVerticalVelocity(float v) => _verticalVelocity = v;

    /// <summary>
    /// Retourne true si le joueur est considéré comme au sol.
    /// </summary>
    public bool IsGrounded => _isGrounded;

    /// <summary>
    /// Retourne l'état d'ancrage sol du frame précédent.
    /// </summary>
    public bool WasGroundedLastFrame => _wasGroundedLastFrame;

    /// <summary>
    /// Vélocité verticale courante appliquée par la physique.
    /// </summary>
    public float VerticalVelocity => _verticalVelocity;

    /// <summary>
    /// Détache le joueur de la plateforme courante. Si <paramref name="inheritVelocity"/> est vrai,
    /// hérite de la vélocité horizontale de la plateforme pour éviter les téléportations brutales.
    /// </summary>
    public void DetachFromPlatform(bool inheritVelocity)
    {
        if (_currentPlatform == null)
            return;

        if (inheritVelocity)
        {
            Vector3 vel = _currentPlatform.Velocity;
            _desiredHorizontalVelocity += new Vector3(vel.x, 0f, vel.z);
        }

        _currentPlatform?.SetPassenger(_controller, false);
        _currentPlatform = null;
    }
}
