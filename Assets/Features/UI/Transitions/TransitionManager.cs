using FishFlingers.UI;
using System;
using UnityEngine;

namespace FishFlingers.UI.Transitions
{
    public interface ITransitionManagerListener
    { }

    public class TransitionManager : GameSystem<ITransitionManagerListener>
    {
        private TransitionManagerConfig _config;
        public TransitionManagerConfig Config => _config;

        private UIManager _uiManager;

        private FadeOverlay _fadeOverlay;

        public override void Initialise(GameManagerConfig gameManagerConfig)
        {
            _config = gameManagerConfig.TransitionManagerConfig;

            _uiManager = GameManager.Instance.Get<UIManager>();

            _fadeOverlay = _uiManager.CreateUIElementInLayer(_uiManager.Config.FadeOverlay, UILayer.Overlay);

            base.Initialise(gameManagerConfig);
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public void CoverScreen(Action onComplete)
        {
            _fadeOverlay.Show(onComplete);
        }

        public void UncoverScreen(Action onComplete)
        {
            _fadeOverlay.Hide(onComplete);
        }
    }
}