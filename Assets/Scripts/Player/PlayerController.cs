using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using DG.Tweening;

[Flags]
public enum Actions
{
    Move = 1,
    Jump = 2,
    Teleport = 4,
    Pary = 8,
    Dash = 16,
    Action6 = 32
}

public class PlayerController : MonoBehaviour
{
    private Actions _actions;
    public int playerLevel;

    private CharacterController _controller;

    private bool _canMove;
    private bool _canJump;
    private bool _canPary;
    private bool _canTeleport;
    private bool _pause;
    private bool _isParing;
    private bool _canExtraJump;
    private bool _isGrounded;

    private float _jumpVelocity;

    private Vector2 _axisInput;
    private Vector2 _paryInput;

    [SerializeField] private float raycastDistance = 1.1f; // Distance du rayon vers le bas à émettre
    [SerializeField] private LayerMask groundLayer; // Layer de la surface considérée comme le sol

    private GameObject _objectToActivate;

    private Control _control;
    [HideInInspector]public InputAction moveAction;
    [HideInInspector]public InputAction jumpAction;
    [HideInInspector]public InputAction parryAction;
    [HideInInspector]public InputAction teleportAction;

    private Vector3 _startPosition;

    void Awake() {
        _control = new Control();
    }

    void Start()
    {
        _startPosition = transform.position;
        _controller = GetComponent<CharacterController>();
        _objectToActivate = GameRegistry.Instance.GetObjectToActivate();
        if (_objectToActivate == null)
        {
            Debug.LogWarning("ObjectToActivate not found in GameRegistry");
        }
        SetUpActions(playerLevel);
    }

    void OnEnable() {
        moveAction = _control.Player.Move;
        moveAction.Enable();

        jumpAction = _control.Player.Jump;
        jumpAction.Enable();

        parryAction = _control.Player.Parry;
        parryAction.Enable();

        teleportAction = _control.Player.Teleport;
        teleportAction.Enable();
    }

    void OnDisable() {
         moveAction.Disable();
        jumpAction.Disable();
        parryAction.Disable();
        teleportAction.Disable();
    }

    void FixedUpdate()
    {
        // Obtenez la position actuelle du joueur
        Vector3 playerPosition = transform.position;

        // Émettre un rayon vers le bas depuis le joueur
        Ray rayCenter = new Ray(playerPosition, Vector3.down);
        Ray raytopRight = new Ray(playerPosition + new Vector3(0.5f, 0f, 0.5f), Vector3.down);
        Ray raytopLeft = new Ray(playerPosition + new Vector3(-0.5f, 0.5f, 0.5f), Vector3.down);
        Ray raybottomRight = new Ray(playerPosition + new Vector3(0.5f, 0.5f, -0.5f), Vector3.down);
        Ray raybottomLeft = new Ray(playerPosition + new Vector3(-0.5f, 0.5f, -0.5f), Vector3.down);

        _isGrounded = Physics.Raycast(rayCenter, raycastDistance, groundLayer) ||
                      Physics.Raycast(raytopRight, raycastDistance, groundLayer) ||
                      Physics.Raycast(raytopLeft, raycastDistance, groundLayer) ||
                      Physics.Raycast(raybottomRight, raycastDistance, groundLayer) ||
                      Physics.Raycast(raybottomLeft, raycastDistance, groundLayer);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameConstants.TAG_ENEMY)
            Death();

        if (other.tag == GameConstants.TAG_PROJECTILE) {
            Destroy(other.gameObject);
            Death();
        }
        if (other.tag == GameConstants.TAG_PROJECTILE_PARRY && !IsParing)
        {
            Destroy(other.gameObject);
            Death();
        }
    }

    public void Death()
    {
        OnDisable();

        Time.timeScale = 0;

        Camera.main.GetComponent<CameraShake>().Shake();

        CanvasGroup blackScreen = GameRegistry.Instance.GetBlackScreen();
        if (blackScreen == null)
        {
            Debug.LogError("BlackScreen CanvasGroup not found in GameRegistry");
            return;
        }

        DOVirtual.DelayedCall(0.01f, async () =>
        {
            await Utils.Functions.ShowCanvasGroup(blackScreen);
            transform.position = _startPosition;
            Time.timeScale = 1;
            DOVirtual.DelayedCall(0.35f, () =>
            {
                OnEnable();
            });
            await Utils.Functions.HideCanvasGroup(blackScreen);
        }).SetUpdate(true);
    }



    // Méthode pour ajouter une action
    public void AddAction(Actions action)
    {
        // Utiliser l'opérateur bit à bit OR pour ajouter l'action à l'état actuel
        _actions |= action;
    }

    // Méthode pour retirer une action
    public void RemoveAction(Actions action)
    {
        // Utiliser l'opérateur bit à bit AND avec le complément pour retirer l'action de l'état actuel
        _actions &= ~action;
    }

    // Méthode pour ajouter des actions en fonction de la valeur entière
    public void SetUpActions(int valeur)
    {
        foreach (Actions action in Enum.GetValues(typeof(Actions)))
        {
            if (((int)action & valeur) != 0)
            {
                AddAction(action);
            }
        }

        ActivateAction();
        AfficherActions();
    }

    // Méthode pour vérifier si une action est active
    public bool IsActionActive(Actions action)
    {
        // Utilise l'opérateur bit à bit AND pour vérifier si l'action est active
        return (_actions & action) == action;
    }

    public void ActivateAction()
    {
        _canMove        = IsActionActive(Actions.Move);
        _canJump        = IsActionActive(Actions.Jump);
        _canTeleport    = IsActionActive(Actions.Teleport);
        _canPary        = IsActionActive(Actions.Pary);
        // boolAction3 = IsActionActive(Actions.Dash);
        // boolAction3 = IsActionActive(Actions.Action6);
    }

    // Méthode pour afficher l'état actuel des actions
    public void AfficherActions()
    {
        Debug.Log("État des actions : " + _actions);
    }

    #region GETTER & SETTER
    public bool CanMove
    {
        get
        {
            return _canMove;
        }

        set
        {
            _canMove = value;
        }
    }

    public bool CanJump
    {
        get
        {
            return _canJump;
        }

        set
        {
            _canJump = value;
        }
    }

    public bool CanPary
    {
        get
        {
            return _canPary;
        }

        set
        {
            _canPary = value;
        }
    }

    public bool CanTeleport
    {
        get
        {
            return _canTeleport;
        }

        set
        {
            _canTeleport = value;
        }
    }

    public CharacterController Controller
    {
        get
        {
            return _controller;
        }

        set
        {
            _controller = value;
        }
    }

    public float JumpVelocity
    {
        get
        {
            return _jumpVelocity;
        }

        set
        {
            _jumpVelocity = value;
        }
    }

    public Vector2 AxisInput
    {
        get
        {
            return _axisInput;
        }

        set
        {
            _axisInput = value;
        }
    }

    public bool IsParing
    {
        get
        {
            return _isParing;
        }

        set
        {
            _isParing = value;
        }
    }

    public Vector2 ParyInput
    {
        get
        {
            return _paryInput;
        }

        set
        {
            _paryInput = value;
        }
    }

    public bool IsGrounded {
         get
        {
            return _isGrounded;
        }

        set
        {
            _isGrounded = value;
        }
    }

    #endregion
}
