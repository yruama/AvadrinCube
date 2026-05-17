

using System.Collections;
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
    [SerializeField] Material activatedMaterial;
    private Material _mat;

    [SerializeField] bool Timer = false;
    [SerializeField] float timerDuration;

    private bool isActive;

    [SerializeField] GameObject[] objectsToActivate;
    private float _timer;

    public GameObject trailObj;
    private List<GameObject> _trail = new List<GameObject>();


    [SerializeField] SwitchType switchType;
    [SerializeField] SwitchCall switchCall;

    [SerializeField] Transform _sprite;

    private void Start()
    {
        _mat = transform.GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        if (Timer && isActive && Time.time - _timer > timerDuration)
        {
            gameObject.GetComponent<MeshRenderer>().material = _mat;
            _timer = Time.time;
            _sprite.gameObject.SetActive(false);
            isActive = false;

            foreach (var item in objectsToActivate)
            {
                // Destroy(_trail[0]);
                // _trail.RemoveAt(0);

                if (switchCall == SwitchCall.ActiveGameObject)
                    item.SetActive(!item.activeSelf);
                else
                    item.SendMessage("CallFromSwitch");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((switchType == SwitchType.Pression && other.gameObject.tag == GameConstants.TAG_PLAYER) ||
            (switchType == SwitchType.Projectile && other.gameObject.tag == GameConstants.TAG_PROJECTILE_PARRY))
        {
            if (Timer)
            {
                _timer = Time.time;
                gameObject.GetComponent<MeshRenderer>().material = activatedMaterial;
                if (!isActive) ActivateObject();
                isActive = true;

                _sprite.gameObject.SetActive(true);
                _sprite.transform.GetChild(0).localScale = new Vector3(0, 1.75f, 1);
                _sprite.GetChild(0).DOScaleX(23f, timerDuration).SetEase(Ease.Linear);
            }
            else if (!isActive)
            {
                _timer = Time.time;
                isActive = true;

                ActivateObject();

                gameObject.GetComponent<MeshRenderer>().material = activatedMaterial;
            }
        }
    }

    void ActivateObject()
    {
        foreach (var item in objectsToActivate)
        {
            // GameObject go = Instantiate(trailObj, transform.position, Quaternion.identity) as GameObject;
            // go.transform.SetParent(transform);
            // go.GetComponent<TrailSwitchTimer>().SetTarget(item);
            // _trail.Add(go);

            if (switchCall == SwitchCall.ActiveGameObject)
            {
                item.SetActive(!item.activeSelf);
            }
            else
            {
                item.SendMessage("EnableFromSwitch");
            }

        }
    }
    
    public void ResetState()
    {
        
    }
}