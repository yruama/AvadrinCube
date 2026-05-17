using UnityEngine;
using DG.Tweening;

/// <summary>
/// Gère la mort et le respawn du joueur.
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class PlayerDeath : MonoBehaviour
{
    private PlayerController _playerController;
    private Vector3 _spawnPosition;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    void Start()
    {
        _spawnPosition = transform.position;
    }

    public void Die()
    {
        _playerController.SetInputEnabled(false);
        Time.timeScale = 0f;

        Camera.main?.GetComponent<CameraShake>()?.Shake();

        CanvasGroup blackScreen = GameRegistry.Instance.GetBlackScreen();
        if (blackScreen == null)
        {
            Debug.LogError("BlackScreen CanvasGroup not found in GameRegistry");
            RespawnImmediate();
            return;
        }

        DOVirtual.DelayedCall(0.01f, async () =>
        {
            await Utils.Functions.ShowCanvasGroup(blackScreen);
            RespawnImmediate();
            Time.timeScale = 1f;
            DOVirtual.DelayedCall(0.35f, () => _playerController.SetInputEnabled(true));
            await Utils.Functions.HideCanvasGroup(blackScreen);
        }).SetUpdate(true);
    }

    private void RespawnImmediate()
    {
        GetComponent<PlayerPhysicsHandler>()?.DetachFromPlatform(false);
        transform.position = _spawnPosition;
    }

    public void SetSpawnPosition(Vector3 position) => _spawnPosition = position;
}
