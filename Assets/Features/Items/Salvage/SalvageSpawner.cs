using PurrNet;
using UnityEngine;
using FishFlingers.Environments;

using NetworkManager = FishFlingers.Networking.NetworkManager;

namespace FishFlingers.Items
{
    public class SalvageSpawner : NetworkBehaviour
    {
        [SerializeField] private Salvage _driftwoodPrefab;

        [SerializeField] private float _spawnInterval = 5f;

        private NetworkManager _networkManager;

        private Raft _raft;

        private float _spawnTimer;

        public void Initialise(Raft raft)   
        {
            _raft = raft;
        }

        protected override void OnInitializeModules()
        {
            _networkManager = GameManager.Instance.Get<NetworkManager>();
        }

        private void Update()
        {
            _spawnTimer += Time.deltaTime;

            if (_spawnTimer < _spawnInterval)
            {
                return;
            }

            _spawnTimer -= _spawnInterval;

            _networkManager.Spawn(_driftwoodPrefab, NetworkManager.HiddenSpawnPosition);
        }
    }
}