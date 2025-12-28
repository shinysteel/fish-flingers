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
        [SerializeField] private ushort _udpServerPort;
        [SerializeField] private ushort _steamServerPort;
        [SerializeField] private ushort _broadcastPort;

        public PurrNet.NetworkManager PurrnetNetworkManagerPrefab => _purrnetNetworkManagerPrefab;
        public PurrLobby.LobbyManager PurrentLobbyManagerPrefab => _purrnetLobbyManagerPrefab;
        public ushort UDPServerPort => _udpServerPort;
        public ushort SteamServerPort => _steamServerPort;
        public ushort BroadcastPort => _broadcastPort;
    }
}