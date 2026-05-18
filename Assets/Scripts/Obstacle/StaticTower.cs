using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tour statique qui génère périodiquement des projectiles vers une cible.
/// </summary>
public class StaticTower : MonoBehaviour
{
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Transform _startPosition;

    [SerializeField]
    private GameObject _fx;

    [SerializeField]
    private float _reload;
    private float _time;

    [SerializeField]
    private float _delayBeforeStart = 0;

    private bool _playerActivateIt;

    private bool _isActive = true;

    private void Start() {
        _time = -_reload + _delayBeforeStart;
    }

    private void Update()
    {
        if (Time.time - _time > _reload && _isActive)
        {
            GameObject projectile = Instantiate(_fx, _startPosition.position, Quaternion.identity);
            projectile.GetComponent<Projectile>().SetTarget(transform, _target.position);
            _time = Time.time;
        }
    }

    public void ActivateAfterPlayerMove() {
        _time = -_reload;
        _isActive = true;
    }
}
