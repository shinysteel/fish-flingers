using FishFlingers.States;
using PurrNet;
using ShinyOwl.Common;
using UnityEngine;

namespace FishFlingers.Networking
{
    public abstract class GameplayBehaviour : NetBehaviour
    {
        protected GameplayContext _context;

        protected bool _isInitialised;
        public bool IsInitialised => _isInitialised;

        protected override void OnSpawned()
        {
            base.OnSpawned();

            if (isServer)
            {
                return;
            }

            if (TryGetComponent<NetworkTransform>(out _))
            {
                ForceSyncNetworkTransform();
            }
        }

        /// <summary>
        /// Since scene setup is not immediate, clients can have their NetworkTransform parents be desynced. 
        /// We can force a sync on the server by making the parent dirty
        /// </summary>
        private void ForceSyncNetworkTransform()
        {
            Transform parent = transform.parent;
            transform.SetParent(null);
            transform.SetParent(parent);
        }

        public virtual void Initialise(GameplayContext context)
        {
            _context = context;

            _isInitialised = true;
        }
    }
}