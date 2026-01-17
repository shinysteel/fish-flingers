using System;
using UnityEngine;

namespace FishFlingers.UI
{
    public class UIAnimationParams
    {
        public Action<bool> SetIsVisible { get; private set; }
        public Action OnComplete { get; private set; }
        public GameObject GameObject { get; private set; }
        public CanvasGroup CanvasGroup { get; private set; }

        public UIAnimationParams(Action<bool> setIsVisible, Action onComplete, GameObject gameObject, CanvasGroup canvasGroup)
        {
            SetIsVisible = setIsVisible;
            OnComplete = onComplete;
            GameObject = gameObject;
            CanvasGroup = canvasGroup;
        }
    }

    public abstract class UIAnimation : MonoBehaviour
    {
        public abstract void Show(UIAnimationParams parameters);
        public abstract void Hide(UIAnimationParams parameters);
    }
}