using FishFlingers.Networking;
using FishFlingers.Pools;
using FishFlingers.Scenes;
using ShinyOwl.Common;
using System.Collections.Generic;
using UnityEngine;

namespace FishFlingers.Entities
{
    public enum EEntity
    {
        RaftPlayer  = 1 ,
        DroppedItem = 2 ,
        RaftTile    = 3 ,
        WaveSign    = 4 ,
        FlyingFish  = 5 ,
    }

    public interface IEntityManagerListener
    { }

    public class EntityManager : GameSystem<IEntityManagerListener>
    {
        private NetworkManager _networkManager;
        private PoolManager _poolManager;

        private EntityManagerConfig _config;

        private Dictionary<EEntity, IEntity> _entityPrefabMap = new();

        public override void Initialise(GameManagerConfig config)
        {
            _networkManager = GameManager.Instance.Get<NetworkManager>();
            _poolManager = GameManager.Instance.Get<PoolManager>();

            _config = config.EntityManagerConfig;

            foreach (EntityMapping mapping in _config.EntityMappings)
            {
                _entityPrefabMap.Add(mapping.Entity, mapping.Prefab.GetComponent<IEntity>());
            }

            base.Initialise(config);
        }

        /// <summary>
        /// Centralised spawn method for entities, handling NetEntity, Entity, and Poolables all in one
        /// </summary>
        public IEntity Spawn(EEntity type, SpawnParams parameters)
        {
            if (!_entityPrefabMap.TryGetValue(type, out IEntity prefab))
            {
                Debugger.LogError(this, $"The entity {type} has not been mapped to a prefab");
                return default;
            }

            // NetEntity
            if (prefab is NetEntity netEntity)
            {
                return _networkManager.Spawn(netEntity, parameters);
            }

            Entity entity = prefab as Entity;

            // Entity + Poolable
            if (entity is IPoolable)
            {
                return (IEntity)_poolManager.Get(entity.GetType(), parameters);
            }

            // Entity
            if (entity != null)
            {
                return parameters.Spawn(entity);
            }

            Debugger.LogError(this, $"Failed to cast {prefab} into a known entity class");
            return default;
        }

        public void Despawn(IEntity entity)
        {
            // NetEntity
            if (entity is NetEntity netEntity)
            {
                _networkManager.Despawn(netEntity);
                return;
            }

            Entity obj = entity as Entity;

            // Entity + Poolable
            if (obj is IPoolable)
            {
                _poolManager.Return(obj);
                return;
            }

            // Entity
            if (obj != null)
            {
                Object.Destroy(obj.gameObject);
                return;
            }
        }
    }
}