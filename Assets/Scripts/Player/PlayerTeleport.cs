using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class PlayerTeleport : MonoBehaviour
{
    enum TeleportState
    {
        TELEPORT,
        SAVEPOS
    }

    private TeleportState _state;

    [SerializeField]
    private GameObject _copy;
    private GameObject _teleport;

    private Vector3 _savePosition;

    private PlayerController _playerController;

    private const float transitionTime = 0.1f;

    private int _teleportState = 0;

    public void SetUI()
    {
        _playerController = GetComponent<PlayerController>();
    }

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _state = TeleportState.SAVEPOS;

        _playerController.teleportAction.canceled += OnMyTeleportActionCanceled;
        _playerController.teleportAction.performed += OnMyTeleportActionPerformed;
	}

    void OnMyTeleportActionPerformed(InputAction.CallbackContext context) {
        if (!_playerController.CanTeleport)
            return;

        if (_state == TeleportState.SAVEPOS)
        {
            _savePosition = transform.position;
            _state = TeleportState.TELEPORT;
            _teleport = Instantiate(_copy, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation) as GameObject;
            if (_teleport != null)
            {
                _teleport = _teleport.transform.GetChild(0).gameObject;
            }
            else
            {
                Debug.LogError("Failed to instantiate teleport copy");
                _state = TeleportState.SAVEPOS;
            }
        }
        else if (_state == TeleportState.TELEPORT)
        {
            if (_teleport == null)
            {
                Debug.LogError("Teleport object is null. Cannot complete teleportation.");
                return;
            }

            Teleport(_savePosition);
            _playerController.CanMove = false;
            _state = TeleportState.SAVEPOS;
        }
    }

    void OnMyTeleportActionCanceled(InputAction.CallbackContext context) {

    }

    public void Teleport(Vector3 target)
    {
        _playerController.CanMove = false;
        DOTween.To(() => transform.position, x => transform.position = x, target, transitionTime).OnComplete(() => {
            transform.localScale = new Vector3(2, 2, 2);
            DOTween.To(() => transform.localScale, x => transform.localScale = x, new Vector3(1f, 1f, 1f), transitionTime).OnComplete(() => {
                transform.localScale = Vector3.one;
            });
            Destroy(_teleport.transform.parent.gameObject);
            _playerController.CanMove = true;
        });
    }

}
