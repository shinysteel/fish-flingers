using PurrNet;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace FishFlingers.GameObjects
{
    public interface IGameObjectManagerListener
    {
        void OnGameObjectInstantiated(GameObject gameObject) { }
        void OnGameObjectDestroyed(GameObject gameObject) { }
    }

    public class GameObjectManager : GameSystem<IGameObjectManagerListener>
    {
        private GameObjectManagerConfig _config;

        public override void Initialise(GameManagerConfig config)
        {
            _config = config.GameObjectManagerConfig;

            base.Initialise(config);
        }

        public T UnityProxyInstantiate<T>(T component, Vector3 position, Quaternion rotation, Transform parent) where T : Component => Instantiate(() => UnityProxy.Instantiate(component, position, rotation, parent));
        public T UnityProxyInstantiate<T>(T component, Vector3 position, Quaternion rotation, Scene scene) where T : Component => Instantiate(() => UnityProxy.Instantiate(component, position, rotation, scene));
        public T Instantiate<T>(T component, Vector3 position, Quaternion rotation, Transform parent) where T : Component => Instantiate(() => Object.Instantiate(component, position, rotation, parent));
        public T Instantiate<T>(T component, Transform parent, bool worldPositionStays) where T : Component => Instantiate(() => Object.Instantiate(component, parent, worldPositionStays));
        public T Instantiate<T>(T component, Transform parent) where T : Component => Instantiate(component, Vector3.zero, Quaternion.identity, parent);
        public T Instantiate<T>(T component) where T : Component => Instantiate(component, Vector3.zero, Quaternion.identity, null);

        private T Instantiate<T>(Func<T> instantiate) where T : Component
        {
            T obj = instantiate();
            NotifyGameObjectInstantiated(obj.gameObject);
            return obj;
        }

        public AsyncInstantiateOperation<T> InstantiateAsync<T>(T component, InstantiateParameters parameters) where T : Component
        {
            AsyncInstantiateOperation<T> op = Object.InstantiateAsync(component, parameters);

            op.completed += _ =>
            {
                NotifyGameObjectInstantiated(op.Result[0].gameObject);
            };

            return op;
        }

        public void Destroy(GameObject gameObject)
        {
            NotifyGameObjectDestroyed(gameObject);
            Object.Destroy(gameObject);
        }

        private void NotifyGameObjectInstantiated(GameObject gameObject) => Listeners.Dispatch(listener => listener.OnGameObjectInstantiated(gameObject));
        private void NotifyGameObjectDestroyed(GameObject gameObject) => Listeners.Dispatch(listener => listener.OnGameObjectDestroyed(gameObject));
    }
}