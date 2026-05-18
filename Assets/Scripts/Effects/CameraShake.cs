using UnityEngine;
using DG.Tweening;

/// <summary>
/// Fournit une animation de tremblement de la caméra via DOTween.
/// </summary>
public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float duration = 0.5f;   // Durée du tremblement
    public float strength = 0.5f;   // Force du tremblement
    public int vibrato = 20;        // Nombre d’oscillations
    public float randomness = 90f;  // Aléatoire dans les directions
    public bool ignoreTimeScale = true;

    private Tween shakeTween;

    /// <summary>
    /// Lance un tremblement de caméra.
    /// </summary>
    public void Shake()
    {
        // Si un shake est déjà en cours, on le kill pour ne pas cumuler
        if (shakeTween != null && shakeTween.IsActive())
            shakeTween.Kill();

        // DOShakeRotation secoue la rotation (plus réaliste qu’un position shake pour la caméra)
        shakeTween = transform.DOShakeRotation(
            duration,
            strength,
            vibrato,
            randomness,
            false
        )
        .SetUpdate(ignoreTimeScale); // Ignore le timeScale si demandé
    }
}
