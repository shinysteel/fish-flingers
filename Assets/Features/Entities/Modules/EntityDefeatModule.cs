using FishFlingers.Items;
using PrimeTween;
using UnityEngine;
using System;
using ShinyOwl.Common;
using FishFlingers.Networking;
using FishFlingers.Pools;

namespace FishFlingers.Entities
{
    public class EntityDefeatModule
    {
        protected EntityManager _entityManager;
        protected ItemManager _itemManager;
        protected NetworkManager _networkManager;
        protected PoolManager _poolManager;

        protected IEntity _entity;

        protected EntityDefeatSettings _entityDefeatSettings;

        protected bool _isDefeated;
        public bool IsDefeated => _isDefeated;

        public event Action OnDefeated;

        public EntityDefeatModule(IEntity entity)
        {
            _entityManager = GameManager.Instance.Get<EntityManager>();
            _itemManager = GameManager.Instance.Get<ItemManager>();
            _networkManager = GameManager.Instance.Get<NetworkManager>();
            _poolManager = GameManager.Instance.Get<PoolManager>();

            _entity = entity;

            _entityDefeatSettings = _entity.EntityDefinitionData.EntityDefeatSettings;

            _entity.EntityHealthModule.OnChanged += HandleHealthChanged;
        }

        ~EntityDefeatModule()
        {
            if (_entity != null)
            {
                _entity.EntityHealthModule.OnChanged -= HandleHealthChanged;
            }
        }

        public virtual void Tick()
        { }

        private void HandleHealthChanged(int previous, int current)
        {
            if (_isDefeated)
            {
                return;
            }

            if (current > 0)
            {
                return;
            }

            Defeat();
        }

        protected void RaiseDefeated()
        {
            OnDefeated?.Invoke();
        }

        public virtual void Defeat()
        {
            _isDefeated = true;

            RaiseDefeated();

            // Entities are local, so they need to be 'despawned' on all clients
            Despawn();
        }

        protected virtual void Despawn()
        {
            if (_networkManager.IsServer)
            {
                _itemManager.SpawnDrops(_entity.Transform.position, DroppedItemType.Default, _entity.EntityDefinitionData.DropTables);
            }

            _entityManager.Despawn(_entity);
        }
    }
}