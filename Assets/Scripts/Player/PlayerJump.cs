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
    private bool _jumpAvailable = true;
    private bool _isJumping;
    private bool _showJumpFx;

    private float MaxJumpVelocity => _config != null ? _config.maxJumpVelocity : _maxJumpVelocity;
    private float MinJumpVelocity => _config != null ? _config.minJumpVelocity : _minJumpVelocity;
    private float JumpFeedbackTime => _config != null ? _config.jumpFeedbackDuration : feedbackJumpTime;

    void Awake()
    {
    }

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _physicsHandler = GetComponent<PlayerPhysicsHandler>();

        if (_config == null)
            Debug.LogWarning("PlayerJump: no PlayerGameplayConfig assigned, using local jump values.");

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
        if (!_jumpAvailable || _physicsHandler == null || !CanJumpNow() || !_playerController.CanJump)
            return;

        if (!_showJumpFx)
            Invoke(nameof(ActivateFx), 0.1f);

        _jumpAvailable = false;
        _isJumping = true;
        _physicsHandler.SetVerticalVelocity(MaxJumpVelocity);
        _physicsHandler.DetachFromPlatform(inheritVelocity: true);
        PlayJumpFeedback();
    }

    void OnJumpCanceled(InputAction.CallbackContext context)
    {
        if (_physicsHandler != null &&
            _physicsHandler.VerticalVelocity > MinJumpVelocity &&
            _playerController.CanJump)
        {
            _physicsHandler.SetVerticalVelocity(MinJumpVelocity);
        }

        _isJumping = false;
    }

    void FixedUpdate()
    {
        if (_physicsHandler == null)
            return;

        bool grounded = _physicsHandler.IsGrounded || (_playerController != null && _playerController.Controller != null && _playerController.Controller.isGrounded);
        if (grounded)
        {
            if (_showJumpFx)
            {
                _showJumpFx = false;
                if (jumpFx != null)
                {
                    GameObject fx = Instantiate(
                        jumpFx,
                        new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z),
                        Quaternion.identity);
                    fx.transform.eulerAngles = new Vector3(90f, 0f, 0f);
                }
            }

            _jumpAvailable = true;
            _isJumping = false;
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
        DOTween.To(() => visual.localScale, x => visual.localScale = x, Vector3.one * 0.75f, JumpFeedbackTime)
            .OnComplete(() =>
            {
                DOTween.To(() => visual.localScale, x => visual.localScale = x, Vector3.one, JumpFeedbackTime);
            });
    }
}
