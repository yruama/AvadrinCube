using UnityEngine;
using UnityEngine.InputSystem;
using System;

[Flags]
public enum Actions
{
    Move = 1,
    Jump = 2,
    Teleport = 4,
    Parry = 8,
    Dash = 16,
    Action6 = 32
}

/// <summary>
/// Hub central : lecture des InputActions et garde l'état des permissions du joueur
/// (Move, Jump, Parry, Teleport). Les modules interrogeront cette classe pour savoir
/// si une action est autorisée.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Actions initialActions = Actions.Move | Actions.Jump | Actions.Teleport | Actions.Parry;

    [Tooltip("Legacy fallback: if non-zero, ce int est interprété comme un mask Actions.")]
    public int playerLevel;

    [SerializeField] private LayerMask groundLayer;

    private Actions _actions;
    private CharacterController _controller;

    private bool _canMove;
    private bool _canJump;
    private bool _canParry;
    private bool _canTeleport;
    private bool _isParrying;
    private bool _inputEnabled;

    private Control _control;

    [HideInInspector] public InputAction moveAction;
    [HideInInspector] public InputAction jumpAction;
    [HideInInspector] public InputAction parryAction;
    [HideInInspector] public InputAction teleportAction;

    void Awake()
    {
        _control = new Control();
        _controller = GetComponent<CharacterController>();

        if (_controller == null)
        {
            Debug.LogError("PlayerController requires a CharacterController on the GameObject.");
            enabled = false;
            return;
        }

        moveAction = _control.Player.Move;
        jumpAction = _control.Player.Jump;
        parryAction = _control.Player.Parry;
        teleportAction = _control.Player.Teleport;
    }

    void Start()
    {
        Actions actions = initialActions != 0 ? initialActions : (Actions)playerLevel;
        if (actions == 0)
            actions = Actions.Move | Actions.Jump | Actions.Teleport | Actions.Parry;

        SetUpActions(actions);
    }

    void OnEnable()
    {
        SetInputEnabled(true);
    }

    void OnDisable()
    {
        SetInputEnabled(false);
    }

    public void SetInputEnabled(bool enabled)
    {
        if (moveAction == null)
            return;

        if (enabled)
        {
            if (_inputEnabled)
                return;

            moveAction.Enable();
            jumpAction.Enable();
            parryAction.Enable();
            teleportAction.Enable();
            _inputEnabled = true;
        }
        else
        {
            if (!_inputEnabled)
                return;

            moveAction.Disable();
            jumpAction.Disable();
            parryAction.Disable();
            teleportAction.Disable();
            _inputEnabled = false;
        }
    }

    public void AddAction(Actions action) => _actions |= action;
    public void RemoveAction(Actions action) => _actions &= ~action;

    public void SetUpActions(Actions actions)
    {
        _actions = actions;
        ActivateAction();
    }

    public bool IsActionActive(Actions action) => (_actions & action) == action;

    public Vector2 MoveInput => moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;

    public void ActivateAction()
    {
        _canMove = IsActionActive(Actions.Move);
        _canJump = IsActionActive(Actions.Jump);
        _canTeleport = IsActionActive(Actions.Teleport);
        _canParry = IsActionActive(Actions.Parry);
    }

    public bool CanMove
    {
        get => _canMove;
        set => _canMove = value;
    }

    public bool CanJump
    {
        get => _canJump;
        set => _canJump = value;
    }

    public bool CanParry
    {
        get => _canParry;
        set => _canParry = value;
    }

    public bool CanTeleport
    {
        get => _canTeleport;
        set => _canTeleport = value;
    }

    public CharacterController Controller => _controller;

    public bool IsParrying
    {
        get => _isParrying;
        set => _isParrying = value;
    }

    /// <summary>Proxy vers PlayerPhysicsHandler (source de vérité).</summary>
    public bool IsGrounded
    {
        get
        {
            PlayerPhysicsHandler physics = GetComponent<PlayerPhysicsHandler>();
            return physics != null && physics.IsGrounded;
        }
    }

    public LayerMask GroundLayer => groundLayer;
}
