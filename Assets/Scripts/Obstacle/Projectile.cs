using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Représente un projectile se déplaçant vers une position cible et pouvant être détruit.
/// </summary>
public class Projectile : MonoBehaviour
{
    public GameObject fx;
    public float speed;
    private Vector3 _pos;
    private Vector3 _startpos;

    private bool _playerActivateIt;
    private float _speed;

    void Start() {
        _speed = speed;
    }

	public void SetTarget(Transform p, Vector3 pos, bool fromParry = false)
    {
        if (fromParry)
        {
            gameObject.tag = "paredProjectile";
            gameObject.layer = 0;
        } 

        _startpos = transform.position;
        _pos = pos;
    }

    public void ParadeNull()
    {
        _pos = _startpos;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _pos, speed * Time.deltaTime);

        if (transform.position == _pos)
        {
            Destruction();
        }
    }

    public void Destruction()
    {
        Instantiate(fx, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameConstants.TAG_WALL))
            Destruction();
    }
}
