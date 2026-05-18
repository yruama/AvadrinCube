using UnityEngine;

[RequireComponent(typeof(PlayerPhysicsHandler))]
/// <summary>
/// Lit l'entrée du joueur et transmet la vitesse désirée à la <see cref="PlayerPhysicsHandler"/>.
/// Sépare la lecture d'entrée (Update) de l'application physique (FixedUpdate dans le handler).
/// </summary>
public class PlayerMovement : MonoBehaviour, IResettable
{
    [SerializeField] private PlayerGameplayConfig _config;
    [SerializeField] private float _moveSpeed = 8f;

    private PlayerController _playerController;
    private PlayerPhysicsHandler _physicsHandler;
    private Vector3 _desiredVelocity;
    private Vector3 _startPosition;

    private float MoveSpeed => _config != null ? _config.moveSpeed : _moveSpeed;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _physicsHandler = GetComponent<PlayerPhysicsHandler>();
        _startPosition = transform.position;

        if (_config == null)
            Debug.LogWarning("PlayerMovement: no PlayerGameplayConfig assigned, using local moveSpeed.");

        if (_physicsHandler == null)
        {
            Debug.LogError("PlayerMovement requires a PlayerPhysicsHandler sur le GameObject.");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (_playerController == null)
            return;

        Vector2 axisInput = _playerController.MoveInput;
        Vector3 direction = new Vector3(axisInput.x, 0f, axisInput.y);

        if (direction.sqrMagnitude > 1f)
            direction.Normalize();

        _desiredVelocity = _playerController.CanMove ? direction * MoveSpeed : Vector3.zero;
        _physicsHandler?.SetDesiredHorizontalVelocity(_desiredVelocity);
    }

    /// <summary>
    /// Réinitialise l'état du joueur (position de départ, velocities) utilisé par les systèmes de reset.
    /// Détache le joueur de toute plateforme si nécessaire.
    /// </summary>
    public PlayerGameplayConfig GameplayConfig => _config;

    public void ResetState()
    {
        transform.position = _startPosition;
        _desiredVelocity = Vector3.zero;
        _physicsHandler?.SetDesiredHorizontalVelocity(Vector3.zero);
        _physicsHandler?.DetachFromPlatform(false);
    }
}
