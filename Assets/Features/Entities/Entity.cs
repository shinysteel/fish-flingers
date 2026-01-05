using FishFlingers.Environments;
using FishFlingers.Networking;
using FishFlingers.Pools;
using UnityEngine;

namespace FishFlingers.Entities
{
    public abstract class Entity : MonoBehaviour, IEntity, IPoolable
    {
        [SerializeField] protected int _maxHealth = 1;

        protected NetworkManager _networkManager;

        protected Raft _raft;
        protected int _currentHealth;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;

        protected virtual void Awake()
        {
            _networkManager = GameManager.Instance.Get<NetworkManager>();
        }

        public virtual void OnTakenFromPool()
        {
            if (!_networkManager.IsServer)
            {
                return;
            }

            SetHealth(_maxHealth);
        }

        public virtual void Initialise(Raft raft)
        {
            _raft = raft;
        }

        public virtual void SetHealth(int health)
        {
            _currentHealth = Mathf.Clamp(health, 0, _maxHealth);
        }

        public virtual void OnReturnedToPool() 
        {
            _raft = null;
        }
    }
}