using PrimeTween;
using ShinyOwl.Common;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI.Transitions
{
    public class FadeOverlay : UIElementAnimated
    {
        private TransitionManager _transitionManager;

        public override void Load()
        {
            _transitionManager = GameManager.Instance.Get<TransitionManager>();
        }

        public override Sequence CreateShowSequence()
        {
            return Sequence.Create(Tween.Alpha(_canvasGroup, startValue: 0f, endValue: 1f, _transitionManager.Config.Duration, _transitionManager.Config.Ease));
        }

        public override Sequence CreateHideSequence()
        {
            return Sequence.Create(Tween.Alpha(_canvasGroup, startValue: 1f, endValue: 0f, _transitionManager.Config.Duration, _transitionManager.Config.Ease));
        }
    }
}