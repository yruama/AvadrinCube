using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace Utils
{
    /// <summary>
    /// Collection de fonctions utilitaires (formatage temps, fade CanvasGroup) utilisées par l'UI.
    /// </summary>
    public class Functions
    {
        public static string FormatTime(float time)
        {
            int intTime = (int)time;
            int minutes = intTime / 60;
            int seconds = intTime % 60;
            float fraction = time * 1000;
            fraction = (fraction % 1000);
            string timeText = String.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
            return timeText;
        }
        
         public static async Task ShowCanvasGroup(CanvasGroup canvas) {
            await canvas.DOFade(1f, 0.75f).SetEase(Ease.InOutQuad).SetUpdate(true).AsyncWaitForCompletion();
        }

        public static async Task HideCanvasGroup(CanvasGroup canvas) {
            await canvas.DOFade(0f, 0.75f).SetEase(Ease.InOutQuad).SetUpdate(true).AsyncWaitForCompletion();
        }
    }
}