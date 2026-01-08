using FishFlingers.UI.Transitions;
using PrimeTween;
using ShinyOwl.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FishFlingers.UI
{
    public abstract class UIElementAnimated : UIElement
    {
        [SerializeField] protected CanvasGroup _canvasGroup;

        private Sequence _showSequence;
        private Sequence _hideSequence;

        private Action _onComplete;

        private const float DefaultDuration = 0.1f;

        public virtual Sequence CreateShowSequence()
        {
            return Sequence.Create(Tween.Alpha(_canvasGroup, startValue: 0f, endValue: 1f, DefaultDuration, Ease.OutQuad));
        }

        public virtual Sequence CreateHideSequence()
        {
            return Sequence.Create(Tween.Alpha(_canvasGroup, startValue: 1f, endValue: 0f, DefaultDuration, Ease.OutQuad));
        }

        public override void Show(Action onComplete)
        {
            if (_showSequence.isAlive)
            {
                Debugger.LogError(this, "Tried to show a UI Element when it is already being animated");
                return;
            }

            if (_hideSequence.isAlive)
            {
                ExecutePending();
                _hideSequence.Stop();
            }

            _canvasGroup.interactable = false;

            _onComplete = onComplete;

            _isVisible = true;
            gameObject.SetActive(true);

            _showSequence = CreateShowSequence();
            _showSequence.OnComplete(() =>
            {
                _canvasGroup.interactable = true;
                ExecutePending();
            });
        }

        public override void Hide(Action onComplete)
        {
            if (_hideSequence.isAlive)
            {
                Debugger.LogError(this, "Tried to hide a UI Element when it is already being animated");
                return;
            }

            if (_showSequence.isAlive)
            {
                ExecutePending();
                _showSequence.Stop();
            }

            _canvasGroup.interactable = false;

            _onComplete = onComplete;

            _hideSequence = CreateHideSequence();
            _hideSequence.OnComplete(() =>
            {
                _isVisible = false;
                gameObject.SetActive(false);
                ExecutePending();
            });
        }

        public override void Unload()
        {
            _showSequence.Stop();
            _hideSequence.Stop();
        }

        private void ExecutePending()
        {
            _onComplete?.Invoke();
            _onComplete = null;
        }
    }
}