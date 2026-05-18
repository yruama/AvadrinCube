using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// Wrapper utility for CanvasGroup fade operations.
/// Delegates actual implementation to <see cref="Utils.Functions"/> to avoid duplication.
/// </summary>
public class UIFunctions : MonoBehaviour
{
    public float speed;

    /// <summary>
    /// Fait apparaître le CanvasGroup attaché à ce GameObject.
    /// </summary>
    public async void ShowCanvasGroup()
    {
        CanvasGroup canvas = GetComponent<CanvasGroup>();
        if (canvas == null) return;
        await Utils.Functions.ShowCanvasGroup(canvas);
    }

    /// <summary>
    /// Fait disparaître le CanvasGroup attaché à ce GameObject.
    /// </summary>
    public async void HideCanvasGroup()
    {
        CanvasGroup canvas = GetComponent<CanvasGroup>();
        if (canvas == null) return;
        await Utils.Functions.HideCanvasGroup(canvas);
    }
}
