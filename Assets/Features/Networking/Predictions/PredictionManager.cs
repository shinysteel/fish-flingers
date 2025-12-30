using FishFlingers.Scenes;
using FishFlingers.States;
using PrimeTween;
using PurrNet;
using ShinyOwl.Common;
using UnityEngine;

namespace FishFlingers.Networking.Predictions
{
    public interface IPredictionManagerListener
    { }

    public class PredictionManager : GameSystem<IPredictionManagerListener>, ISceneManagerListener
    {
        private PredictionManagerConfig _config;

        private SceneManager _sceneManager;

        public override void Initialise(GameManagerConfig config)
        {
            _config = config.PredictionManagerConfig;

            _sceneManager = GameManager.Instance.Get<SceneManager>();
            _sceneManager.AddListener(this);

            base.Initialise(config);
        }

        public override void Shutdown()
        {
            _sceneManager?.RemoveListener(this);

            base.Shutdown();
        }

        public T Spawn<T>(T prefab, PlayerID? owner) where T : Component
        {
            return null;
        }

        public void OnSceneSetActive(EScene previous, EScene current) 
        {
            //if (current == EScene.Game)
            //{
            //    // Had to use this at one point, leaving this here if it comes up again
            //    // _service = UnityProxy.InstantiateDirectly(_config.PredictionServicePrefab);

            //    _service = Object.Instantiate(_config.PredictionServicePrefab);

            //    // Not ideal to take references from a service like this, but it
            //    // wasn't great having to define a method 3 times if it originated in the spawner
            //    _spawner = _service.Spawner;
            //}
            //else if (previous == EScene.Game)
            //{
            //    _service = null;
            //    _spawner = null;
            //}
        }

        public void OnSceneLoaded(EScene scene, LoadSceneMode mode) { }
        public void OnSceneUnloaded(EScene scene) { }
    }
}
