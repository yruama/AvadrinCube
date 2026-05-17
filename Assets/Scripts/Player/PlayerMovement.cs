using UnityEngine;

[RequireComponent(typeof(PlayerPhysicsHandler))]
public class PlayerMovement : MonoBehaviour, IResettable
{
    [SerializeField] private PlayerGameplayConfig _config;
    [SerializeField] private float _moveSpeed = 5f;

    private PlayerController _playerController;
    private PlayerPhysicsHandler _physicsHandler;
    private Vector3 _desiredVelocity;
    private Vector3 _startPosition;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _physicsHandler = GetComponent<PlayerPhysicsHandler>();
        _startPosition = transform.position;

        if (_config != null)
            _moveSpeed = _config.moveSpeed;
    }

    private void Update()
    {
        if (_playerController == null)
            return;

        Vector2 axisInput = _playerController.moveAction.ReadValue<Vector2>();
        Vector3 direction = new Vector3(axisInput.x, 0f, axisInput.y);

        if (direction.sqrMagnitude > 1f)
            direction.Normalize();

        _desiredVelocity = _playerController.CanMove ? direction * _moveSpeed : Vector3.zero;
        _physicsHandler?.SetDesiredHorizontalVelocity(_desiredVelocity);
    }

    public void ResetState()
    {
        transform.position = _startPosition;
        _desiredVelocity = Vector3.zero;
        _physicsHandler?.SetDesiredHorizontalVelocity(Vector3.zero);
        _physicsHandler?.DetachFromPlatform(false);
    }
}
