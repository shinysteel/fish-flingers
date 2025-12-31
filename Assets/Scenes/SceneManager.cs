using FishFlingers.Networking;
using ShinyOwl.Common;
using ShinyOwl.Common.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FishFlingers.Scenes
{
    // Try to keep these 1:1 with scene names for clarity
    public enum EScene
    {
        Startup,
        Default,
        Game,
        EnvironmentMainMenu,
        EnvironmentGameplay,
    }

    public enum LoadSceneMode
    {
        Single   ,
        Additive , 
    }

    public interface ISceneManagerListener
    {
        void OnSceneLoaded(EScene scene, LoadSceneMode mode);
        void OnSceneUnloaded(EScene scene);
        void OnActiveSceneChanged(EScene previous, EScene current);
    }

    public class SceneManager : GameSystem<ISceneManagerListener>
    {
        private SceneManagerConfig _config;

        private NetworkManager _networkManager;

        private Dictionary<EScene, string> _sceneNameMap;

        public override void Initialise(GameManagerConfig config)
        {
            _config = config.SceneManagerConfig;

            _networkManager = GameManager.Instance.Get<NetworkManager>();

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += HandleSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += HandleSceneUnloaded;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += HandleActiveSceneChanged;

            _sceneNameMap = new();
            foreach (SceneMapping mapping in _config.SceneMappings)
            {
                _sceneNameMap.Add(mapping.Enum, mapping.Name);
            }

            base.Initialise(config);
        }

        public override void Shutdown()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= HandleSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= HandleSceneUnloaded;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= HandleActiveSceneChanged;

            base.Shutdown();
        }

        public EScene GetSceneEnum(Scene scene)
        {
            return _sceneNameMap.FirstOrDefault(kvp => kvp.Value == scene.name).Key;
        }

        public Scene GetScene(EScene scene)
        {
            return UnityEngine.SceneManagement.SceneManager.GetSceneByName(GetSceneName(scene));
        }

        public string GetSceneName(EScene scene)
        {
            return _sceneNameMap[scene];
        }

        private EScene GetActiveSceneEnum()
        {
            return GetSceneEnum(GetActiveScene());
        }

        public Scene GetActiveScene()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        }

        public void SetActiveScene(EScene scene)
        {
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(GetScene(scene));
        }

        public void MoveGameObjectToScene(GameObject obj, EScene scene)
        {
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(obj, GetScene(scene));
        }

        public bool IsSceneActive(EScene scene)
        {
            return GetActiveScene().name == GetSceneName(scene);
        }

        public bool IsSceneLoaded(EScene scene)
        {
            return GetScene(scene).isLoaded;
        }

        public void LoadScene(EScene scene, LoadSceneMode mode = LoadSceneMode.Single)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(GetSceneName(scene), (UnityEngine.SceneManagement.LoadSceneMode)mode);
        }

        public AsyncOperationBridge LoadSceneAsync(EScene scene, LoadSceneMode mode = LoadSceneMode.Single)
        {
            return new AsyncOperationBridge(UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GetSceneName(scene), (UnityEngine.SceneManagement.LoadSceneMode)mode));
        }

        public AsyncOperationBridge UnloadSceneAsync(EScene scene)
        {
            return new AsyncOperationBridge(UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(GetSceneName(scene)));
        }

        private void HandleSceneLoaded(Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode) => Listeners.Dispatch(NotifyOnSceneLoaded, GetSceneEnum(scene), (LoadSceneMode)mode);
        private void HandleSceneUnloaded(Scene scene) => Listeners.Dispatch(NotifyOnSceneUnloaded, GetSceneEnum(scene));
        private void HandleActiveSceneChanged(Scene previous, Scene current) => Listeners.Dispatch(NotifyOnActiveSceneChanged, GetSceneEnum(previous), GetSceneEnum(current));

        private void NotifyOnSceneLoaded(ISceneManagerListener listener, EScene scene, LoadSceneMode mode) => listener.OnSceneLoaded(scene, mode);
        private void NotifyOnSceneUnloaded(ISceneManagerListener listener, EScene scene) => listener.OnSceneUnloaded(scene);
        private void NotifyOnActiveSceneChanged(ISceneManagerListener listener, EScene previous, EScene current) => listener.OnActiveSceneChanged(previous, current);
    }
}