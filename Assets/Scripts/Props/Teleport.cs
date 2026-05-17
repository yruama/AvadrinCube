using UnityEngine;
using DG.Tweening;

public class Teleport : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _transitionDuration = 0.1f;

    private bool _canTeleport = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameConstants.TAG_PLAYER) && _canTeleport)
        {
            TeleportPlayer(other);
            return;
        }

        if (other.CompareTag(GameConstants.TAG_PROJECTILE_PARRY) && _canTeleport)
        {
            _target.GetComponent<Teleport>().CanTeleport = false;
            other.transform.position = _target.position;
        }
    }

    private void TeleportPlayer(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
            return;

        player.CanJump = false;
        player.CanParry = false;
        player.CanMove = false;
        player.CanTeleport = false;

        _target.GetComponent<Teleport>().CanTeleport = false;

        PlayerTeleportEffects.PlayStretchPulse(other.transform, _transitionDuration, () =>
        {
            other.transform.position = new Vector3(
                _target.position.x,
                _target.position.y + 0.5f,
                _target.position.z);

            DOVirtual.DelayedCall(0.25f, () =>
            {
                PlayerTeleportEffects.PlayLandScalePulse(other.transform, _transitionDuration, () =>
                {
                    player.CanJump = true;
                    player.CanParry = true;
                    player.CanMove = true;
                    player.CanTeleport = true;
                });
            });
        });
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameConstants.TAG_PLAYER) || other.CompareTag(GameConstants.TAG_PROJECTILE_PARRY))
            _canTeleport = true;
    }

    public bool CanTeleport
    {
        get => _canTeleport;
        set => _canTeleport = value;
    }
}
