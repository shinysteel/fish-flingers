using FishFlingers.Entities;
using FishFlingers.Networking;
using System.Collections.Generic;
using UnityEngine;
using EntityId = FishFlingers.Entities.EntityId;
using System.Linq;
using ShinyOwl.Common;

namespace FishFlingers.Environments
{
    public class DrowningSpawner : GameplayBehaviour, IEntityManagerListener
    {
        private Dictionary<RaftPlayer, Drowning> _playerDrowningMap = new();

        protected override void OnSpawned()
        {
            _entityManager.AddListener(this);

            base.OnSpawned();
        }

        protected override void OnDespawned()
        {
            base.OnDespawned();

            _entityManager?.RemoveListener(this);
        }

        private void Update()
        {
            if (!isOwner)
            {
                return;
            }

            if (!_isInitialised)
            {
                return;
            }

            foreach (RaftPlayer player in _context.Players)
            {
                if (!player.EntityDefeatModule.IsDefeated && player.RaftPlayerPhysicsModule.TimeInWater >= 0.5f)
                {
                    AddDrowning(player);
                }
                else
                {
                    RemoveDrowning(player);
                }
            }
        }

        private void AddDrowning(RaftPlayer player)
        {
            if (_playerDrowningMap.ContainsKey(player))
            {
                return;
            }

            Drowning drowning = (Drowning)_entityManager.Spawn(EntityId.Drowning, new SpawnParams() { Position = NetworkManager.HiddenSpawnPosition });
            drowning.SetTarget(player);
            _playerDrowningMap.Add(player, drowning);
        }

        private void RemoveDrowning(RaftPlayer player)
        {
            if (!_playerDrowningMap.TryGetValue(player, out Drowning drowning))
            {
                return;
            }

            _playerDrowningMap.Remove(player);
            _entityManager.Despawn(drowning);
            
        }

        void IEntityManagerListener.OnEntityDespawned(IEntity entity)
        {
            if (entity is RaftPlayer player)
            {
                RemoveDrowning(player);
            }
            else if (entity is Drowning drowning)
            {
                // Drownings can despawn themselves when coming into contact with a player
                _playerDrowningMap.Remove(_playerDrowningMap.First(kvp => kvp.Value == drowning).Key);
            }
        }
    }
}