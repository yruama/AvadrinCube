using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

enum SwitchType
{
    Pression,
    Projectile
}

enum SwitchCall
{
    ActiveGameObject,
    ActiveScript
}

public class Switches : MonoBehaviour, IResettable
{
    [SerializeField] private Material activatedMaterial;
    [SerializeField] private bool Timer;
    [SerializeField] private float timerDuration;
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private SwitchType switchType;
    [SerializeField] private SwitchCall switchCall;
    [SerializeField] private Transform _sprite;

    private Material _defaultMaterial;
    private bool _isActive;
    private float _timerStart;
    private bool[] _initialObjectStates;

    private void Start()
    {
        _defaultMaterial = GetComponent<MeshRenderer>().material;

        if (objectsToActivate != null)
        {
            _initialObjectStates = new bool[objectsToActivate.Length];
            for (int i = 0; i < objectsToActivate.Length; i++)
                _initialObjectStates[i] = objectsToActivate[i] != null && objectsToActivate[i].activeSelf;
        }
    }

    void Update()
    {
        if (!Timer || !_isActive || Time.time - _timerStart <= timerDuration)
            return;

        DeactivateSwitch();
    }

    private void OnTriggerEnter(Collider other)
    {
        bool playerPress = switchType == SwitchType.Pression && other.CompareTag(GameConstants.TAG_PLAYER);
        bool projectileHit = switchType == SwitchType.Projectile && other.CompareTag(GameConstants.TAG_PROJECTILE_PARRY);

        if (!playerPress && !projectileHit)
            return;

        if (Timer)
        {
            _timerStart = Time.time;
            GetComponent<MeshRenderer>().material = activatedMaterial;
            if (!_isActive)
                ActivateTargets();
            _isActive = true;

            _sprite.gameObject.SetActive(true);
            _sprite.GetChild(0).localScale = new Vector3(0f, 1.75f, 1f);
            _sprite.GetChild(0).DOScaleX(23f, timerDuration).SetEase(Ease.Linear);
        }
        else if (!_isActive)
        {
            _isActive = true;
            ActivateTargets();
            GetComponent<MeshRenderer>().material = activatedMaterial;
        }
    }

    private void ActivateTargets()
    {
        if (objectsToActivate == null)
            return;

        foreach (GameObject item in objectsToActivate)
        {
            if (item == null)
                continue;

            if (switchCall == SwitchCall.ActiveGameObject)
            {
                item.SetActive(!item.activeSelf);
                continue;
            }

            ISwitchable switchable = item.GetComponent<ISwitchable>();
            if (switchable != null)
                switchable.EnableFromSwitch();
            else
                item.BroadcastMessage("EnableFromSwitch", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void DeactivateSwitch()
    {
        GetComponent<MeshRenderer>().material = _defaultMaterial;
        _sprite.gameObject.SetActive(false);
        _isActive = false;

        if (objectsToActivate == null)
            return;

        foreach (GameObject item in objectsToActivate)
        {
            if (item == null)
                continue;

            if (switchCall == SwitchCall.ActiveGameObject)
            {
                item.SetActive(!item.activeSelf);
                continue;
            }

            ISwitchable switchable = item.GetComponent<ISwitchable>();
            if (switchable != null)
                switchable.CallFromSwitch();
            else
                item.BroadcastMessage("CallFromSwitch", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void ResetState()
    {
        _isActive = false;
        GetComponent<MeshRenderer>().material = _defaultMaterial;

        if (_sprite != null)
            _sprite.gameObject.SetActive(false);

        if (objectsToActivate == null || _initialObjectStates == null)
            return;

        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            if (objectsToActivate[i] != null)
                objectsToActivate[i].SetActive(_initialObjectStates[i]);
        }
    }
}
