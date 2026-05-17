using UnityEngine;

/// <summary>
/// Détecte les dangers (ennemis, projectiles) et déclenche la mort.
/// </summary>
[RequireComponent(typeof(PlayerDeath))]
public class PlayerHazardDetector : MonoBehaviour
{
    private PlayerDeath _playerDeath;
    private PlayerController _playerController;

    void Awake()
    {
        _playerDeath = GetComponent<PlayerDeath>();
        _playerController = GetComponent<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameConstants.TAG_ENEMY))
        {
            _playerDeath.Die();
            return;
        }

        if (other.CompareTag(GameConstants.TAG_PROJECTILE))
        {
            Destroy(other.gameObject);
            _playerDeath.Die();
            return;
        }

        if (other.CompareTag(GameConstants.TAG_PROJECTILE_PARRY) && !_playerController.IsParrying)
        {
            Destroy(other.gameObject);
            _playerDeath.Die();
        }
    }
}
