using FishFlingers.Entities;
using FishFlingers.Networking;
using FishFlingers.States;
using Newtonsoft.Json;
using ParrelSync;
using PurrNet;
using ShinyOwl.Common;
using ShinyOwl.Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using NetworkManager = FishFlingers.Networking.NetworkManager;

namespace FishFlingers.Saving
{
    [Serializable]
    public class UserData
    {
        [JsonProperty] public string Guid { get; private set; }

        public UserData()
        {
            Guid = System.Guid.NewGuid().ToString();
        }
    }

    [Serializable]
    public class GameData
    {
        [JsonProperty] public Dictionary<string, RaftPlayerData> RaftPlayerDatas { get; private set; } = new();
    }

    [Serializable]
    public class RaftPlayerData
    {
        [JsonProperty] public SerialisableVector3 Position { get; private set; }
        [JsonProperty] public SerialisableQuaternion Rotation { get; private set; }

        private const int Precision = 1;

        public RaftPlayerData(Vector3 position, Quaternion rotation)
        {
            position = Utils.Math.RoundVector3(position, Precision);
            rotation = Utils.Math.RoundQuaternion(rotation, Precision);

            Position = new SerialisableVector3(position);
            Rotation = new SerialisableQuaternion(rotation);
        }
    }

    public class SerialisableVector3
    {
        [JsonProperty] public float X { get; private set; }
        [JsonProperty] public float Y { get; private set; }
        [JsonProperty] public float Z { get; private set; }

        public SerialisableVector3() : this(Vector3.zero) 
        { }

        public SerialisableVector3(Vector3 vector3)
        {
            X = vector3.x;
            Y = vector3.y;
            Z = vector3.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }

    public class SerialisableQuaternion
    {
        [JsonProperty] public float X { get; private set; }
        [JsonProperty] public float Y { get; private set; }
        [JsonProperty] public float Z { get; private set; }
        [JsonProperty] public float W { get; private set; }

        public SerialisableQuaternion() : this(Quaternion.identity)
        { }

        public SerialisableQuaternion(Quaternion quaternion)
        {
            X = quaternion.x;
            Y = quaternion.y;
            Z = quaternion.z;
            W = quaternion.w;
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(X, Y, Z, W);
        }
    }

    public interface ISaveManagerListener
    { }

    public class SaveManager : GameSystem<ISaveManagerListener>
    {
        private NetworkManager _networkManager;

        private SaveManagerConfig _config;

        private string _persistentDataPath;
        private string _userDataPath;
        private string _gameDataPath;

        private UserData _userData;
        private GameData _gameData;

        public UserData UserData => _userData;
        public GameData GameData => _gameData;

        public override void Initialise(GameManagerConfig config)
        {
            _networkManager = GameManager.Instance.Get<NetworkManager>();

            _config = config.SaveManagerConfig;

            _persistentDataPath = CreatePersistentDataPath();

            _userDataPath = Path.Combine(_persistentDataPath, $"{_config.UserDataFileName}.json");
            _gameDataPath = Path.Combine(_persistentDataPath, $"{_config.GameDataFileName}.json");

            LoadUser();

            base.Initialise(config);
        }

        private string CreatePersistentDataPath()
        {
            string path = Application.persistentDataPath;

            if (ClonesManager.IsClone())
            {
                int cloneNumber = int.Parse(ClonesManager.GetCurrentProjectPath().Split($"{ClonesManager.CloneNameSuffix}_")[1]);
                path += $"{ClonesManager.CloneNameSuffix}_{cloneNumber}";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            return path;
        }

        /// <summary>
        /// Loads the user's details, which currently just contains their guid. No save equivalent is necessary, 
        /// since after first load this data will never change
        /// </summary>
        private void LoadUser()
        {
            if (File.Exists(_userDataPath))
            {
                string json = File.ReadAllText(_userDataPath);
                _userData = JsonConvert.DeserializeObject<UserData>(json);
            }
            else
            {
                _userData = new();
                string json = JsonConvert.SerializeObject(_userData);
                File.WriteAllText(_userDataPath, json);
            }
        }

        /// <summary>
        /// Loads a game save file. This includes data for players, other entities, and the raft
        /// </summary>
        public void LoadGame()
        {
            if (File.Exists(_gameDataPath))
            {
                string json = File.ReadAllText(_gameDataPath);
                _gameData = JsonConvert.DeserializeObject<GameData>(json);
            }
            else
            {
                _gameData = new();
            }
        }

        public void SaveGame()
        {
            foreach (PurrnetPlayer purrnetPlayer in _networkManager.PurrnetPlayers.Values)
            {
                SaveRaftPlayer(purrnetPlayer.Guid, purrnetPlayer.RaftPlayer);
            }
            
            string json = JsonConvert.SerializeObject(_gameData, Formatting.Indented);
            File.WriteAllText(_gameDataPath, json);
        }

        public RaftPlayerData GetRaftPlayerData(string guid, GameplayContext context)
        {
            if (!_gameData.RaftPlayerDatas.ContainsKey(guid))
            {
                Vector3 position;
                
                if (context.Raft.TryGetRandomTile(out RaftTile tile))
                {
                    position = context.Raft.CellToWorldPosition(tile.Cell);
                    position.y = tile.GetSurfaceY();
                }
                else
                {
                    position = Vector3.zero;
                }

                _gameData.RaftPlayerDatas[guid] = new RaftPlayerData(position, Quaternion.identity);
            }

            return _gameData.RaftPlayerDatas[guid];
        }

        public void LoadRaftPlayer(RaftPlayer raftPlayer, RaftPlayerData data)
        {
            raftPlayer.transform.position = data.Position.ToVector3();
            raftPlayer.transform.rotation = data.Rotation.ToQuaternion();

            raftPlayer.Rigidbody.linearVelocity = Vector3.zero;
            raftPlayer.Rigidbody.angularVelocity = Vector3.zero;
        }
        
        public void SaveRaftPlayer(string guid, RaftPlayer raftPlayer)
        {
            _gameData.RaftPlayerDatas[guid] = new RaftPlayerData(raftPlayer.transform.position, raftPlayer.transform.rotation);
        }
    }
}