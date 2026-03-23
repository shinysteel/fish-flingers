using FishFlingers.Cameras;
using FishFlingers.Entities;
using FishFlingers.Saving;
using FishFlingers.Scenes;
using Newtonsoft.Json;
using PurrLobby;
using PurrNet;
using PurrNet.Transports;
using ShinyOwl.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace FishFlingers.Networking
{
    public class PurrnetPlayer : NetBehaviour, ISaveable
    {
        [SerializeField] private RaftPlayer _raftPlayerPrefab;

        private SyncVar<string> _guid = new SyncVar<string>(ownerAuth: true);
        private SyncVar<RaftPlayer> _raftPlayer = new SyncVar<RaftPlayer>(ownerAuth: true);

        public string Guid => _guid;
        public RaftPlayer RaftPlayer => _raftPlayer;

        protected override void OnSpawned()
        {
            base.OnSpawned();

            _instantiateManager.RaiseComponentInstantiated(this);

            if (isOwner)
            {
                _guid.value = _saveManager.UserSave.Guid;
            }
        }

        protected override void OnDespawned()
        {
            base.OnDespawned();

            _instantiateManager.RaiseComponentDestroyed(this);

            if (_networkManager.IsServer)
            {
                ((ISaveable)this).Save();
            }
        }

        public RaftPlayer CreateRaftPlayer()
        {
            _raftPlayer.value = _networkManager.Spawn(_raftPlayerPrefab, new SpawnParams() { Position = NetworkManager.HiddenSpawnPosition });
            return _raftPlayer;
        }

        async Task ISaveable.LoadAsync()
        {
            await _raftPlayer.value.LoadDataAsync(_guid);
        }

        void ISaveable.Save()
        {
            _raftPlayer.value.Save(_guid);
        }
    }
}