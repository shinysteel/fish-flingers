using UnityEngine;

namespace FishFlingers.UI
{
    public class InstantUIAnimation : UIAnimation
    {
        public override void Show(UIAnimationParams parameters)
        {
            parameters.GameObject.SetActive(true);
            parameters.SetIsVisible(true);
            parameters.OnComplete?.Invoke();
        }

        public override void Hide(UIAnimationParams parameters)
        {
            parameters.GameObject.SetActive(false);
            parameters.SetIsVisible(false);
            parameters.OnComplete?.Invoke();
        }
    }
}