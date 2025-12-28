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
using Newtonsoft.Json;

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
            _networkManager.AddListener(this);

            _broadcastClient = new();
            _broadcastClient.EnableBroadcast = true;

            _listenerClient = new UdpClient(_networkManager.Config.BroadcastPort);

            StartListening();
        }

        public override void Shutdown()
        {
            _networkManager?.RemoveListener(this);

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
            Dictionary<string, string> properties = new() { { AddressKey, address }, { StartedKey, false.ToString() } };

            _currentLobby = new Lobby(name, lobbyId, ownerId, DefaultMemberLimit, members, properties);

            StartBroadcasting();

            _networkManager.SetClientTransport<UDPTransport>();
            _networkManager.TryGetClientTransport(out UDPTransport transport);
            transport.address = address;

            _networkManager.StartServer();
            _networkManager.StartClient();

            RaiseOnLobbyCreated(_currentLobby);
            RaiseOnLobbyEnter(_currentLobby);

            StartLobby();

            return Task.FromResult(_currentLobby);
        }

        public override Task<Lobby> JoinLobbyAsync(string lobbyId)
        {
            if (!_knownLobbies.TryGetValue(lobbyId, out Lobby lobby))
            {
                return Task.FromException<Lobby>(new Exception("Could not find a lobby with matching id"));
            }

            if (!lobby.Properties.TryGetValue(AddressKey, out string address))
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
            _currentLobby.Properties[StartedKey] = true.ToString();

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
                    string json = JsonConvert.SerializeObject(_currentLobby);
                    byte[] bytes = Encoding.UTF8.GetBytes(json);
                    await _broadcastClient.SendAsync(bytes, bytes.Length, new IPEndPoint(IPAddress.Broadcast, _networkManager.Config.BroadcastPort));
                    await Task.Delay(BroadcastInterval);
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
                    try
                    {
                        UdpReceiveResult result = await _listenerClient.ReceiveAsync();
                        string json = Encoding.UTF8.GetString(result.Buffer);
                        Lobby lobby = JsonConvert.DeserializeObject<Lobby>(json);
                        
                        // Ignore our own broadcasts
                        if (_currentLobby != null && _currentLobby.OwnerId == _networkManager.LocalPlayer.ToString())
                        {
                            continue;
                        }

                        // Detects when the 'started' property goes from false to true and relays it. Can eventually
                        // be moved into a method that raises events for anything we are interested in
                        if (_currentLobby != null && _currentLobby.LobbyId == lobby.LobbyId)
                        {
                            if (bool.Parse(_currentLobby.Properties[StartedKey]) == false && bool.Parse(lobby.Properties[StartedKey]) == true)
                            {
                                RaiseOnLobbyStart();
                            }
                        }

                        _knownLobbies[lobby.LobbyId] = lobby;
                    }
                    catch { } // Preserve the loop and ignore
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
            if (_networkManager.LocalPlayer == id)
            {
                return;
            }

            _currentLobby.Members.RemoveAll(member => member.Id == id.ToString());
        }

        public void OnPlayerJoined(PlayerID id, bool isReconnect) 
        { 
            if (_networkManager.LocalPlayer == id)
            {
                return;
            }

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