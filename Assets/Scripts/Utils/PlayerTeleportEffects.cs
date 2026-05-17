using System;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Effets visuels partagés pour les téléportations du joueur.
/// </summary>
public static class PlayerTeleportEffects
{
    public static void AnimateTo(Transform player, Vector3 worldPosition, float duration, Action onComplete = null)
    {
        if (player == null)
            return;

        DOTween.To(() => player.position, x => player.position = x, worldPosition, duration)
            .OnComplete(() =>
            {
                PlayLandScalePulse(player, 0.1f, onComplete);
            });
    }

    public static void PlayLandScalePulse(Transform player, float duration, Action onComplete = null)
    {
        if (player == null)
        {
            onComplete?.Invoke();
            return;
        }

        Transform visual = player.childCount > 0 ? player.GetChild(0) : player;
        visual.localScale = new Vector3(2f, 2f, 2f);
        DOTween.To(() => visual.localScale, x => visual.localScale = x, Vector3.one, duration)
            .OnComplete(() => onComplete?.Invoke());
    }

    public static void PlayStretchPulse(Transform player, float duration, Action onComplete = null)
    {
        if (player == null || player.childCount == 0)
        {
            onComplete?.Invoke();
            return;
        }

        Transform visual = player.GetChild(0);
        DOTween.To(() => visual.localScale, x => visual.localScale = x, new Vector3(0.5f, 3f, 0.5f), duration)
            .OnComplete(() => onComplete?.Invoke());
    }
}
