using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerJump : MonoBehaviour
{
    private PlayerController _playerController;

    [SerializeField]
    private float _maxJumpVelocity;
    [SerializeField]
    private float _minJumpVelocity;
    [SerializeField]
    private float _gravityMultiplier;
    private bool _hasJump;
    private bool _isJumping;

    private float _verticalVelocity;


    [SerializeField]
    private GameObject jumpFx;
    private bool _showJumpFx;
    private float _positionY;

    private float _time;

    private int jumpState = 0;

    [SerializeField] float feedbackJumpTime;

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _time = Time.time;

        _playerController.jumpAction.canceled += OnMyJumpActionCanceled;
        _playerController.jumpAction.performed += OnMyJumpActionPerformed;
    }

    void OnMyJumpActionPerformed(InputAction.CallbackContext context) {
        // Si le joueur appuie sur le bouton, qu'il a son Jump de dispo et qu'il n'est pas en cours de saut. Avec l'action de dispo
        if (_hasJump && !_isJumping)
        {
            if (!_showJumpFx) Invoke("activeFx", 0.1f);
            _hasJump = false;
            _isJumping = true;
            _verticalVelocity = _maxJumpVelocity;

            FeedBackJumping();
        }
    }

    void OnMyJumpActionCanceled(InputAction.CallbackContext context) {
        if (_verticalVelocity > _minJumpVelocity  && _playerController.CanJump)
            _verticalVelocity = _minJumpVelocity; 

        _isJumping = false;
    }

    void Update()
    {
        // Si le joueur n'est pas au sol
        if (!_playerController.IsGrounded)
        {
            _showJumpFx = true;
            _verticalVelocity -= GameConstants.GRAVITY_MULTIPLIER * _gravityMultiplier * Time.deltaTime;
        }
        // Si le joueur n'est pas en cours de saut et qu'il est au sol
        else if (_playerController.IsGrounded)
        {
            if (_showJumpFx)
            {
                _showJumpFx = false;
                GameObject fx = Instantiate(jumpFx, new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z), Quaternion.identity) as GameObject;
                fx.transform.eulerAngles = new Vector3(90f, 0f, 0);
            }

            if (!_isJumping)
            {
                _verticalVelocity = 0;
                _hasJump = true;
            }
        }

        _playerController.JumpVelocity = _verticalVelocity;
    }

    void activeFx() {
        _showJumpFx = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), transform.localScale / 2);
    }

    void FeedBackJumping() {
        DOTween.To(() => gameObject.transform.GetChild(0).localScale, x => gameObject.transform.GetChild(0).localScale = x, Vector3.one * 0.75f, feedbackJumpTime).OnComplete(()=>{
            DOTween.To(() => gameObject.transform.GetChild(0).localScale, x => gameObject.transform.GetChild(0).localScale = x, Vector3.one, feedbackJumpTime);
        });

    }
}
