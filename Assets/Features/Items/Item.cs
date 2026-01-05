using PurrNet;
using UnityEngine;
using FishFlingers.Environments;

namespace FishFlingers.Items
{
    public class Item : NetworkBehaviour
    {
        protected Raft _raft;

        public void Initialise(Raft raft)
        {
            _raft = raft;
        }

        protected override void OnDespawned()
        {
            _raft = null;
        }
    }
}