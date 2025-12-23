using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishFlingers.Cameras
{
    public interface ICameraManagerListener
    { }

    public class CameraManager : GameSystem<ICameraManagerListener>
    {
        private CameraManagerConfig _config;

        private Camera _camera;

        public override void Initialise(GameManagerConfig gameManagerConfig)
        {
            _config = gameManagerConfig.CameraManagerConfig;

            _camera = Object.Instantiate(_config.GameCameraPrefab);

            Object.DontDestroyOnLoad(_camera.gameObject);

            base.Initialise(gameManagerConfig);
        }

        public override void Shutdown()
        {
            _camera = null;

            base.Shutdown();
        }
    }
}