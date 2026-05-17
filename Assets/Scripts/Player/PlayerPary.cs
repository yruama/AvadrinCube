using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class PlayerParry : MonoBehaviour
{
    public GameObject _arrow;
    public GameObject _timer;

    public float _slowTime;

    public float _reload;

    public float _maxDuration;
    public float _durationTime;
    private float _time;

    public float transitionTime;

    [SerializeField]
    private LayerMask _layer;
    [HideInInspector] public bool _getPower;

    private bool _pary;
    private bool _canParry;
    private bool _reloadB;
    private bool _reflect;

    private Vector2 _axisInput;

    private PlayerController _playerController;

    private const float c_transitionTime = 0.15f;

    private int _parryState = 0;

    private Tween _tweenTimer; 

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _time = -_reload;
        _reflect = false;
        _time = Time.time;
        _canParry = false;

        _playerController.parryAction.canceled += OnMyParryActionCanceled;
        _playerController.parryAction.performed += OnMyParryActionPerformed;
    }

    void OnMyParryActionCanceled(InputAction.CallbackContext context)
    {
        if (_canParry)
        {
            if (_tweenTimer != null) _tweenTimer.Kill();
            StartCoroutine(ParryWindowCoroutine());
        }
    }

    void OnMyParryActionPerformed(InputAction.CallbackContext context)
    {
        _playerController.CanMove = false;
        _playerController.CanJump = false;
        _playerController.CanTeleport = false;
        _canParry = true;

        Time.timeScale = _slowTime;
        _parryState = 1;
        _playerController.IsParing = true;
        _timer.SetActive(true);

        Transform sprite = _timer.transform.GetChild(0).transform;
        sprite.transform.localScale = new Vector3(0, 1.75f, 1);

        _tweenTimer = sprite.DOScaleX(23f, _durationTime).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
        {
            if (_canParry) {
                if (_tweenTimer != null) _tweenTimer.Kill();
                StartCoroutine(ParryWindowCoroutine());   
            }
        });
    }

    private IEnumerator ParryWindowCoroutine()
    {
        _canParry = true;
        float timer = 0f;
        Time.timeScale = 1;
        EndParry();

        while (timer < 1)
        {
            Collider[] col = Physics.OverlapSphere(transform.position, 1.15f, _layer);

            foreach (var item in col)
            {
                _reflect = true;

                if (_axisInput == Vector2.zero)
                {
                    item.GetComponent<Projectile>()?.Destruction();
                    timer = 99f;
                }
                else
                {
                    item.transform.position = transform.position;
                    Vector3 target = new Vector3(
                        transform.position.x + _axisInput.x * 1000000,
                        item.transform.position.y,
                        transform.position.z + _axisInput.y * 1000000
                    );

                    item.GetComponent<Projectile>()?.SetTarget(transform, target, true);
                    timer = 99f;
                }
            }

            _reflect = false;

            timer += Time.deltaTime;
            yield return null; // attendre frame suivante
        }

       
    }

    private void EndParry()
    {
        _canParry = false;
        

        // Anim avec DOTween
        transform.GetChild(0).DORotate(new Vector3(0, 180f, 0), 0.25f).OnComplete(() =>
        {
            transform.GetChild(0).DORotate(Vector3.zero, 0);
            _arrow.SetActive(false);
            _playerController.CanMove = true;
            _playerController.CanJump = true;
            _playerController.CanTeleport = true;
            _playerController.IsParing = false;
        });

        _timer.SetActive(false);
    }

    void Update()
    {
        if (!_playerController.CanPary)
            return;

        _axisInput = _playerController.moveAction.ReadValue<Vector2>();

        if (_canParry)
        {
            if (_axisInput != Vector2.zero)
                _arrow.SetActive(true);
            else
                _arrow.SetActive(false);

            Vector2 v2 = _axisInput;
            float angle = Mathf.Atan2(v2.x, v2.y) * Mathf.Rad2Deg + 180;
            if (angle < 0) angle += 360;
            _arrow.transform.eulerAngles = new Vector3(0, angle, 0);
        }

    }
}