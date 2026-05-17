using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIFunctions : MonoBehaviour
{
    public float speed;

    public async void ShowCanvasGroup() {
        CanvasGroup canvas = GetComponent<CanvasGroup>();
        await canvas.DOFade(0f, 2f).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();
    }

    public async void HideCanvasGroup() {
        CanvasGroup canvas = GetComponent<CanvasGroup>();
        await canvas.DOFade(1f, 2f).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();
    }
}
