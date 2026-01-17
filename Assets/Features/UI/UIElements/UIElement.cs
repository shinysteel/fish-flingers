using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace FishFlingers.UI
{
    public abstract class UIElement : MonoBehaviour
    {
        [SerializeField] private UIAnimation _uiAnimation;
        [SerializeField] protected RectTransform _rectTransform;
        [SerializeField] protected CanvasGroup _canvasGroup;

        protected Canvas _canvas;
        protected bool _isVisible;

        public RectTransform RectTransform => _rectTransform;
        public Canvas Canvas => _canvas;

        public virtual void Load(Canvas canvas)
        {
            _canvas = canvas;
        }

        public virtual void Show(Action onComplete)
        {
            _uiAnimation.Show(new UIAnimationParams(SetIsVisible, onComplete, gameObject, _canvasGroup));
        }

        public virtual void Hide(Action onComplete)
        {
            _uiAnimation.Hide(new UIAnimationParams(SetIsVisible, onComplete, gameObject, _canvasGroup));
        }

        public virtual void Unload()
        {
            _canvas = null;
        }

        private void SetIsVisible(bool visible)
        {
            _isVisible = visible;
        }
    }
}