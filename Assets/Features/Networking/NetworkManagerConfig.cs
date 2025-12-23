using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishFlingers.Networking
{
    [CreateAssetMenu(fileName = "NetworkManagerConfig", menuName = "Configs/Managers/NetworkMangerConfig")]
    public class NetworkManagerConfig : ScriptableObject
    {
        [SerializeField] private PurrNet.NetworkManager _purrnetNetworkManagerPrefab;
        [SerializeField] private PurrLobby.LobbyManager _purrnetLobbyManagerPrefab;

        public PurrNet.NetworkManager PurrnetNetworkManagerPrefab => _purrnetNetworkManagerPrefab;
        public PurrLobby.LobbyManager PurrentLobbyManagerPrefab => _purrnetLobbyManagerPrefab;
    }
}