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
using EntityId = FishFlingers.Entities.EntityId;

namespace FishFlingers.Networking
{
    public class PurrnetPlayerSave
    {
        [JsonProperty] public int SaveId { get; private set; }
        [JsonProperty] public int ItemInstanceIdCounter { get; private set; } = 0;
        [JsonProperty] public RaftPlayerSave RaftPlayer { get; private set; } = new();

        public PurrnetPlayerSave()
        { }

        public PurrnetPlayerSave(int saveId, int itemInstanceIdCounter, RaftPlayerSave raftPlayer)
        {
            SaveId = saveId;
            ItemInstanceIdCounter = itemInstanceIdCounter;
            RaftPlayer = raftPlayer;
        }

        public async Task LoadToAsync(PurrnetPlayer purrnetPlayer)
        {
            // The raft player may not be created yet
            while (purrnetPlayer.GetNetRaftPlayer() == null)
            {
                await Task.Yield();
            }

            purrnetPlayer.SetNetSaveId(SaveId);
            purrnetPlayer.SetNetItemInstanceIdCounter(ItemInstanceIdCounter);

            await RaftPlayer.LoadToAsync(purrnetPlayer.GetNetRaftPlayer());
        }

        public void SaveFrom(PurrnetPlayer purrnetPlayer)
        {
            SaveId = purrnetPlayer.GetNetSaveId();
            ItemInstanceIdCounter = purrnetPlayer.GetNetItemInstanceIdCounter();
            RaftPlayer.SaveFrom(purrnetPlayer.GetNetRaftPlayer());
        }

        public void ApplyDefaults()
        {
            RaftPlayer.ApplyDefaults();
        }

        // We can't safely reference any managers in the default constructor, so this exists
        public void SetSaveId(int id)
        {
            SaveId = id;
        }
    }

    public class PurrnetPlayer : NetBehaviour, ISaveable
    {
        private SyncVar<string> _netGuid = new SyncVar<string>(ownerAuth: true);
        private SyncVar<int> _netSaveId = new SyncVar<int>(ownerAuth: true);
        private SyncVar<int> _netItemInstanceIdCounter = new SyncVar<int>(ownerAuth: true);
        private SyncVar<RaftPlayer> _netRaftPlayer = new SyncVar<RaftPlayer>(ownerAuth: true);

        protected override void OnSpawned()
        {
            base.OnSpawned();

            _instantiateManager.RaiseComponentInstantiated(this);

            if (isOwner)
            {
                _netGuid.value = _saveManager.UserSave.Guid;
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

        // Every instance of an inventory item needs a unique id, and we can guarantee this by combining the
        // player's id with a local counter
        public string GetNextNetItemInstanceId()
        {
            return $"{_netSaveId.value}_{_netItemInstanceIdCounter.value++}";
        }

        public RaftPlayer CreateRaftPlayer()
        {
            _netRaftPlayer.value = (RaftPlayer)_entityManager.Spawn(EntityId.RaftPlayer, new SpawnParams() { Position = NetworkManager.HiddenSpawnPosition });
            return _netRaftPlayer;
        }

        public RaftPlayer GetNetRaftPlayer()
        {
            return _netRaftPlayer.value;
        }

        public int GetNetSaveId()
        {
            return _netSaveId.value;
        }

        public void SetNetSaveId(int id)
        {
            _netSaveId.value = id;
        }

        public int GetNetItemInstanceIdCounter()
        {
            return _netItemInstanceIdCounter.value;
        }

        public void SetNetItemInstanceIdCounter(int counter)
        {
            _netItemInstanceIdCounter.value = counter;
        }

        [ServerRpc]
        private async Task<PurrnetPlayerSave> GetSaveRpc()
        {
            // Syncvars won't be ready if this is requested as the host is initialising
            while (!isSpawned)
            {
                await Task.Yield();
            }

            if (!_saveManager.GameSave.Players.ContainsKey(_netGuid))
            {
                _saveManager.GameSave.Players[_netGuid] = new();
                _saveManager.GameSave.Players[_netGuid].ApplyDefaults();
                _saveManager.GameSave.Players[_netGuid].SetSaveId(_saveManager.GameSave.Players.Count - 1);
            }

            return _saveManager.GameSave.Players[_netGuid];
        }

        async Task ISaveable.LoadAsync()
        {
            // Clients can join while the host is still initialising, and so they will be in the collection of Saveables
            // to load on the server. We can just return here knowing that clients will load their player themselves
            if (!isOwner)
            {
                return;
            }

            PurrnetPlayerSave save = await GetSaveRpc();

            await save.LoadToAsync(this);
        }

        void ISaveable.Save()
        {
            _saveManager.GameSave.Players[_netGuid.value].SaveFrom(this);
        }
    }
}