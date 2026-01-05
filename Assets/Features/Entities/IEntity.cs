using FishFlingers.Environments;
using UnityEngine;

namespace FishFlingers.Entities
{
    public interface IEntity
    {
        public int CurrentHealth { get; }
        public int MaxHealth { get; }
        void SetHealth(int health);

        void Initialise(Raft raft);
    }
}