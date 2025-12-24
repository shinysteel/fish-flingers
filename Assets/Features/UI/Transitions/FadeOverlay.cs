using PrimeTween;
using ShinyOwl.Common;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI.Transitions
{
    public class FadeOverlay : UIElement
    {
        [SerializeField] private Image _image;

        private TransitionManager _transitionManager;

        public override void Load()
        {
            _transitionManager = GameManager.Instance.Get<TransitionManager>();
        }

        public override void Show(Action onComplete)
        {
            gameObject.SetActive(true);
            _isVisible = true;

            Tween.Alpha(_image, startValue: 0f, endValue: 1f, duration: _transitionManager.Config.Duration, ease: _transitionManager.Config.Ease)
                .OnComplete(onComplete);
        }

        public override void Hide(Action onComplete)
        {
            onComplete += () =>
            {
                gameObject.SetActive(false);
                _isVisible = false;
            };

            Tween.Alpha(_image, startValue: 1f, endValue: 0f, duration: _transitionManager.Config.Duration, ease: _transitionManager.Config.Ease)
                .OnComplete(onComplete);
        }
    }
}