using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Teleport : MonoBehaviour
{
    [SerializeField] Transform _target;

    PlayerController _player;
    private bool _canTeleport = true;

      private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GameConstants.TAG_PLAYER && _canTeleport == true)
        {
            PlayerTeleport(other);
        } else if (other.gameObject.tag == GameConstants.TAG_PROJECTILE_PARRY && _canTeleport == true) {
            _target.GetComponent<Teleport>().CanTeleport = false;
             other.gameObject.transform.position = _target.position;
        }
    }

    void PlayerTeleport(Collider other) {
        _player = other.GetComponent<PlayerController>();
        _player.CanJump = false;
        _player.CanPary = false;
        _player.CanMove = false;
        _player.CanTeleport = false;

        _target.GetComponent<Teleport>().CanTeleport = false;
        DOTween.To(() => other.gameObject.transform.GetChild(0).localScale, x => other.gameObject.transform.GetChild(0).localScale = x, new Vector3(0.5f, 3f, 0.5f), 0.1f).OnComplete(() => {
            other.gameObject.transform.position = new Vector3(_target.position.x, _target.position.y + 0.5f, _target.position.z);

            DOTween.To(() => 0, x => {}, 0, 0).SetDelay(0.25f).OnComplete(() => {

                DOTween.To(() => other.gameObject.transform.GetChild(0).localScale, x => other.gameObject.transform.GetChild(0).localScale = x, Vector3.one, 0.1f);

                _player.CanJump = true;
                _player.CanPary = true;
                _player.CanMove = true;
                _player.CanTeleport = true;
            });
        });
    }

    void OnTriggerExit(Collider other) {
         if (other.gameObject.tag == GameConstants.TAG_PLAYER || other.gameObject.tag == GameConstants.TAG_PROJECTILE_PARRY)
        {
            _canTeleport = true;
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
}
