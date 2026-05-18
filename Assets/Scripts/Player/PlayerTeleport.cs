using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Permet d'enregistrer une position et de s'y téléporter via un marqueur instancié.
/// Utilise <see cref="PlayerController"/> pour vérifier les permissions d'action.
/// </summary>
public class PlayerTeleport : MonoBehaviour
{
    private enum TeleportState { SavePos, Teleport }

    [SerializeField] private GameObject _copy;
    [SerializeField] private float _transitionDuration = 0.1f;

    private TeleportState _state = TeleportState.SavePos;
    private GameObject _teleportMarkerRoot;
    private Vector3 _savePosition;
    private PlayerController _playerController;

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _playerController.teleportAction.performed += OnTeleportPerformed;
        _playerController.teleportAction.canceled += OnTeleportCanceled;
    }

    void OnDestroy()
    {
        if (_playerController?.teleportAction == null)
            return;

        _playerController.teleportAction.performed -= OnTeleportPerformed;
        _playerController.teleportAction.canceled -= OnTeleportCanceled;
    }

    void OnTeleportPerformed(InputAction.CallbackContext context)
    {
        if (!_playerController.CanTeleport)
            return;

        if (_state == TeleportState.SavePos)
        {
            _savePosition = transform.position;
            _state = TeleportState.Teleport;

            _teleportMarkerRoot = Instantiate(_copy, transform.position, transform.rotation);
            if (_teleportMarkerRoot == null)
            {
                _state = TeleportState.SavePos;
                return;
            }
        }
        else
        {
            if (_teleportMarkerRoot == null)
                return;

            _playerController.CanMove = false;
            PlayerTeleportEffects.AnimateTo(transform, _savePosition, _transitionDuration, () =>
            {
                Destroy(_teleportMarkerRoot.transform.parent != null
                    ? _teleportMarkerRoot.transform.parent.gameObject
                    : _teleportMarkerRoot);
                _teleportMarkerRoot = null;
                _playerController.CanMove = true;
                _state = TeleportState.SavePos;
            });
        }
    }

    void OnTeleportCanceled(InputAction.CallbackContext context) { }
}
