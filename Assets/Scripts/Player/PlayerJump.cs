using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(PlayerPhysicsHandler))]
/// <summary>
/// Gère le saut du joueur en utilisant l'API de <see cref="PlayerPhysicsHandler"/>.
/// Abonne/désabonne proprement les callbacks d'InputAction et protège contre les références null.
/// </summary>
public class PlayerJump : MonoBehaviour
{
    [SerializeField] private PlayerGameplayConfig _config;
    [SerializeField] private float _maxJumpVelocity;
    [SerializeField] private float _minJumpVelocity;
    [SerializeField] private GameObject jumpFx;
    [SerializeField] private float feedbackJumpTime;

    private PlayerController _playerController;
    private PlayerPhysicsHandler _physicsHandler;
    private bool _hasJump = true;
    private bool _isJumping;
    private bool _showJumpFx;

    void Awake()
    {
        if (_config != null)
        {
            _maxJumpVelocity = _config.maxJumpVelocity;
            _minJumpVelocity = _config.minJumpVelocity;
            feedbackJumpTime = _config.jumpFeedbackDuration;
        }
    }

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _physicsHandler = GetComponent<PlayerPhysicsHandler>();

        if (_playerController == null)
        {
            Debug.LogError("PlayerJump requires a PlayerController on the same GameObject.");
            enabled = false;
            return;
        }

        if (_physicsHandler == null)
        {
            Debug.LogError("PlayerJump requires a PlayerPhysicsHandler on the same GameObject.");
            enabled = false;
            return;
        }

        if (_playerController.jumpAction != null)
        {
            _playerController.jumpAction.canceled += OnJumpCanceled;
            _playerController.jumpAction.performed += OnJumpPerformed;
        }
    }

    void OnDestroy()
    {
        if (_playerController?.jumpAction == null)
            return;

        _playerController.jumpAction.canceled -= OnJumpCanceled;
        _playerController.jumpAction.performed -= OnJumpPerformed;
    }

    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (!_hasJump || _isJumping || _physicsHandler == null || !CanJumpNow())
            return;

        if (!_showJumpFx)
            Invoke(nameof(ActivateFx), 0.1f);

        _hasJump = false;
        _isJumping = true;
        _physicsHandler.SetVerticalVelocity(_maxJumpVelocity);
        _physicsHandler.DetachFromPlatform(inheritVelocity: true);
        PlayJumpFeedback();
    }

    void OnJumpCanceled(InputAction.CallbackContext context)
    {
        if (_physicsHandler != null &&
            _physicsHandler.VerticalVelocity > _minJumpVelocity &&
            _playerController.CanJump)
        {
            _physicsHandler.SetVerticalVelocity(_minJumpVelocity);
        }

        _isJumping = false;
    }

    void FixedUpdate()
    {
        if (_physicsHandler == null)
            return;

        if (_physicsHandler.IsGrounded)
        {
            if (_showJumpFx)
            {
                _showJumpFx = false;
                GameObject fx = Instantiate(
                    jumpFx,
                    new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z),
                    Quaternion.identity);
                fx.transform.eulerAngles = new Vector3(90f, 0f, 0f);
            }

            if (!_isJumping)
                _hasJump = true;
        }
        else
        {
            _showJumpFx = true;
        }
    }

    bool CanJumpNow()
    {
        bool physicsGrounded = _physicsHandler != null && (_physicsHandler.IsGrounded || _physicsHandler.WasGroundedLastFrame);
        bool controllerGrounded = _playerController != null && _playerController.Controller != null && _playerController.Controller.isGrounded;
        return physicsGrounded || controllerGrounded;
    }

    void ActivateFx() => _showJumpFx = true;

    void PlayJumpFeedback()
    {
        if (transform.childCount == 0)
            return;

        Transform visual = transform.GetChild(0);
        DOTween.To(() => visual.localScale, x => visual.localScale = x, Vector3.one * 0.75f, feedbackJumpTime)
            .OnComplete(() =>
            {
                DOTween.To(() => visual.localScale, x => visual.localScale = x, Vector3.one, feedbackJumpTime);
            });
    }
}
