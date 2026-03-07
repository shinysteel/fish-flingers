using FishFlingers.Entities;
using UnityEngine;
using FishFlingers.Networking;

using NetworkManager = FishFlingers.Networking.NetworkManager;
using EntityId = FishFlingers.Entities.EntityId;

namespace FishFlingers.Environments
{
    public class WaveSpawner : GameplayBehaviour
    {
        [SerializeField] private float _spawnInterval = 2.5f;

        private float _spawnTimer;
        
        private void Update()
        {
            SpawnUpdate();
        }

        private void SpawnUpdate()
        {
            if (!isServer)
            {
                return;
            }

            if (_spawnTimer < _spawnInterval)
            {
                _spawnTimer += Time.deltaTime;
                return;
            }

            _spawnTimer -= _spawnInterval;

            Spawn();
        }

        private void Spawn()
        {
            _entityManager.Spawn(EntityId.FlyingFish, new SpawnParams() { Position = NetworkManager.HiddenSpawnPosition });
        }
    }
}