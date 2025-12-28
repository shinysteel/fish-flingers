using PurrNet;
using PurrNet.Packing;
using PurrNet.Transports;
using ShinyOwl.Common;
using ShinyOwl.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace FishFlingers.Networking
{
    public class LANLobbyService : LobbyService, INetworkManagerListener
    {
        private NetworkManager _networkManager;

        private Dictionary<string, Lobby> _knownLobbies = new();

        private UdpClient _broadcastClient;
        private bool _isBroadcasting;

        private UdpClient _listenerClient;
        private bool _isListening;

        private const int BroadcastInterval = 2500; // ms

        private const string AddressKey = "address";

        private struct JoinAcceptMessage : IPackedAuto
        {
            public string lobbyId;
        }

        public LANLobbyService()
        {
            _networkManager = GameManager.Instance.Get<NetworkManager>();

            _broadcastClient = new();
            _broadcastClient.EnableBroadcast = true;

            _listenerClient = new UdpClient(_networkManager.Config.BroadcastPort);

            StartListening();
        }

        public override void Shutdown()
        {
            StopBroadcasting();
            StopListening();

            _broadcastClient?.Close();
            _broadcastClient?.Dispose();

            _listenerClient?.Close();
            _listenerClient?.Dispose();
        }

        public override Task<Lobby[]> SearchLobbiesAsync()
        {
            return Task.FromResult(_knownLobbies.Values.ToArray());
        }

        public override Task<Lobby> CreateLobbyAsync()
        {
            string ownerId = _networkManager.LocalPlayer.ToString();
            string name = $"{ownerId}'s Lobby";
            string lobbyId = Guid.NewGuid().ToString();
            List<LobbyMember> members = new() { new LobbyMember(ownerId, ownerId) };
            string address = Utils.Network.GetLocalIpAddress();
            Dictionary<string, string> properties = new() { { AddressKey, address } };

            _currentLobby = new Lobby(name, lobbyId, ownerId, DefaultMemberLimit, members, properties);

            StartBroadcasting();

            _networkManager.SetClientTransport<UDPTransport>();
            _networkManager.TryGetClientTransport(out UDPTransport transport);
            transport.address = address;

            _networkManager.StartServer();
            _networkManager.StartClient();

            RaiseOnLobbyCreated(_currentLobby);
            RaiseOnLobbyEnter(_currentLobby);

            return Task.FromResult(_currentLobby);
        }

        public override Task<Lobby> JoinLobbyAsync(string lobbyId)
        {
            if (_knownLobbies.TryGetValue(lobbyId, out Lobby lobby))
            {
                return Task.FromException<Lobby>(new Exception("Could not find a lobby with matching id"));
            }

            if (lobby.Properties.TryGetValue(AddressKey, out string address))
            {
                return Task.FromException<Lobby>(new Exception("This lobby is not providing an address to join to"));
            }

            _currentLobby = lobby;

            _networkManager.SetClientTransport<UDPTransport>();
            _networkManager.TryGetClientTransport(out UDPTransport transport);
            transport.address = address;

            _networkManager.StartClient();

            RaiseOnLobbyEnter(lobby);

            return Task.FromResult(lobby);
        }

        public override void StartLobby()
        {
            RaiseOnLobbyStart();
        }

        public override void LeaveLobby()
        {
            _currentLobby = null;

            StopBroadcasting();

            RaiseOnLobbyLeave();
        }

        private void StartBroadcasting()
        {
            if (_currentLobby.OwnerId != _networkManager.LocalPlayer.ToString())
            {
                Debugger.LogError(this, "Tried to broadcast a lobby we do not own");
                return;
            }

            if (_isBroadcasting)
            {
                Debugger.LogError(this, "Only one broadcast task should be active at a time");
                return;
            }

            _isBroadcasting = true;

            Task.Run(async () =>
            {
                while (_isBroadcasting)
                {
                    Debugger.Log(this, "broadcast start");
                    string json = JsonUtility.ToJson(_currentLobby);
                    byte[] bytes = Encoding.UTF8.GetBytes(json);
                    await _broadcastClient.SendAsync(bytes, bytes.Length, new IPEndPoint(IPAddress.Broadcast, _networkManager.Config.BroadcastPort));
                    await Task.Delay(BroadcastInterval);
                    Debugger.Log(this, "broadcast end");
                }
            });
        }

        private void StartListening()
        {
            if (_isListening)
            {
                Debugger.LogError(this, "Only one listener task should be active at a time");
                return;
            }

            _isListening = true;

            Task.Run(async () =>
            {
                while (_isListening)
                {
                    Debugger.Log(this, "listen start");
                    UdpReceiveResult result = await _listenerClient.ReceiveAsync();
                    Debugger.Log(this, "listen result");
                    string json = Encoding.UTF8.GetString(result.Buffer);
                    Lobby lobby = JsonUtility.FromJson<Lobby>(json);
                    _knownLobbies[lobby.LobbyId] = lobby;
                }
            });
        }

        private void StopBroadcasting()
        {
            _isBroadcasting = false;
        }

        private void StopListening()
        {
            _isListening = false;
        }

        public void OnNetworkStarted(bool asServer)
        {
            if (asServer)
            {
                return;
            }

            _networkManager.Subscribe<JoinAcceptMessage>(HandleJoinAcceptMessage, _networkManager.IsServer);
        }

        public void OnNetworkShutdown(bool asServer)
        {
            if (asServer)
            {
                return;
            }

            _networkManager.Unsubscribe<JoinAcceptMessage>(HandleJoinAcceptMessage);
        }

        private void HandleJoinAcceptMessage(PlayerID id, JoinAcceptMessage message, bool asServer)
        {
            Debugger.Log(this, $"Received join accept message. Id: {id}, message.lobbyyId: {message.lobbyId}, asServer: {asServer}");
        }

        public void OnPlayerLeft(PlayerID id) 
        {
            _currentLobby.Members.RemoveAll(member => member.Id == id.ToString());
        }

        public void OnPlayerJoined(PlayerID id, bool isReconnect) 
        { 
            if (_currentLobby.Members.Count >= _currentLobby.MemberLimit)
            {
                _networkManager.KickPlayer(id);
                return;
            }

            LobbyMember member = new LobbyMember(id.ToString(), $"Player {id}");
            _currentLobby.Members.Add(member);
            _networkManager.Send(id, new JoinAcceptMessage() { lobbyId = _currentLobby.LobbyId });
        }

        public void OnLobbyCreated(Lobby lobby) { }
        public void OnLobbyEnter(Lobby lobby) { }
        public void OnLobbyLeave() { }
        public void OnLobbyStart() { }
        public void OnClientConnectionState(ConnectionState state) { }
    }
}