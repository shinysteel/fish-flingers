using FishFlingers.Environments;
using FishFlingers.Networking;
using PurrNet;
using UnityEngine;

using NetworkManager = FishFlingers.Networking.NetworkManager;

namespace FishFlingers.Entities
{
    public abstract class NetEntity : NetworkBehaviour, IEntity
    {
        [SerializeField] protected int _maxHealth = 1;

        protected NetworkManager _networkManager;

        protected Raft _raft;
        protected SyncVar<int> _currentHealth;

        public int CurrentHealth => _currentHealth.value;

        public int MaxHealth => _maxHealth;

        public void Initialise(Raft raft)
        {
            _raft = raft;
        }

        protected override void OnInitializeModules()
        {
            _networkManager = GameManager.Instance.Get<NetworkManager>();

            _currentHealth = new SyncVar<int>(_maxHealth);
        }

        protected override void OnSpawned()
        {
            if (!isServer)
            {
                return;
            }

            SetHealth(_maxHealth);
        }

        public void SetHealth(int health)
        {
            _currentHealth.value = Mathf.Clamp(health, 0, _maxHealth);
        }
    }
}