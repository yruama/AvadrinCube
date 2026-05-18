using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

/// <summary>
/// Gère la mécanique de parry du joueur : ralentissement du temps, fenêtre de parry,
/// renvoi ou destruction des projectiles selon l'input directionnel.
/// </summary>
public class PlayerParry : MonoBehaviour
{
    public GameObject _arrow;
    public GameObject _timer;
    public float _slowTime;
    public float _reload;
    public float _durationTime;

    [SerializeField] private LayerMask _layer;
    [HideInInspector] public bool _getPower;

    private bool _canParryWindow;
    private Vector2 _axisInput;
    private PlayerController _playerController;
    private Tween _tweenTimer;

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _playerController.parryAction.canceled += OnParryCanceled;
        _playerController.parryAction.performed += OnParryPerformed;
    }

    void OnDestroy()
    {
        if (_playerController?.parryAction == null)
            return;

        _playerController.parryAction.canceled -= OnParryCanceled;
        _playerController.parryAction.performed -= OnParryPerformed;
    }

    void OnParryPerformed(InputAction.CallbackContext context)
    {
        _playerController.CanMove = false;
        _playerController.CanJump = false;
        _playerController.CanTeleport = false;
        _canParryWindow = true;
        _playerController.IsParrying = true;

        Time.timeScale = _slowTime;
        _timer.SetActive(true);

        Transform sprite = _timer.transform.GetChild(0);
        sprite.localScale = new Vector3(0f, 1.75f, 1f);

        _tweenTimer = sprite.DOScaleX(23f, _durationTime).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
        {
            if (_canParryWindow)
            {
                _tweenTimer?.Kill();
                StartCoroutine(ParryWindowCoroutine());
            }
        });
    }

    void OnParryCanceled(InputAction.CallbackContext context)
    {
        if (_canParryWindow)
        {
            _tweenTimer?.Kill();
            StartCoroutine(ParryWindowCoroutine());
        }
    }

    IEnumerator ParryWindowCoroutine()
    {
        Time.timeScale = 1f;
        EndParry();

        float timer = 0f;
        while (timer < 1f)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 1.15f, _layer);
            foreach (Collider hit in hits)
            {
                if (_axisInput == Vector2.zero)
                {
                    hit.GetComponent<Projectile>()?.Destruction();
                    timer = 99f;
                }
                else
                {
                    hit.transform.position = transform.position;
                    Vector3 target = new Vector3(
                        transform.position.x + _axisInput.x * 1000000f,
                        hit.transform.position.y,
                        transform.position.z + _axisInput.y * 1000000f);
                    hit.GetComponent<Projectile>()?.SetTarget(transform, target, true);
                    timer = 99f;
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    void EndParry()
    {
        _canParryWindow = false;
        _timer.SetActive(false);

        transform.GetChild(0).DORotate(new Vector3(0f, 180f, 0f), 0.25f).OnComplete(() =>
        {
            transform.GetChild(0).DORotate(Vector3.zero, 0f);
            _arrow.SetActive(false);
            _playerController.CanMove = true;
            _playerController.CanJump = true;
            _playerController.CanTeleport = true;
            _playerController.IsParrying = false;
        });
    }

    void Update()
    {
        if (!_playerController.CanParry)
            return;

        _axisInput = _playerController.moveAction.ReadValue<Vector2>();

        if (!_canParryWindow)
            return;

        _arrow.SetActive(_axisInput != Vector2.zero);
        if (_axisInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(_axisInput.x, _axisInput.y) * Mathf.Rad2Deg + 180f;
            if (angle < 0f) angle += 360f;
            _arrow.transform.eulerAngles = new Vector3(0f, angle, 0f);
        }
    }
}
