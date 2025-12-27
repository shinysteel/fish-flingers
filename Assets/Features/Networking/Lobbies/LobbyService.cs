using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace FishFlingers.Networking
{
    public class LobbyMember
    {
        public string Id { get; private set; }
        public string DisplayName { get; private set; }

        // public Texture2D Avatar { get; private set; }

        public LobbyMember(string id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }
    }

    public class Lobby
    {
        public string Name { get; private set; }
        public string LobbyId { get; private set; }
        public string OwnerId { get; private set; }
        public int MemberLimit { get; private set; }
        public List<LobbyMember> Members { get; private set; }
        public Dictionary<string, string> Properties { get; private set; }

        public Lobby(string name, string lobbyId, string ownerId, int memberLimit, List<LobbyMember> members, Dictionary<string, string> properties)
        {
            Name = name;
            LobbyId = lobbyId;
            OwnerId = ownerId;
            MemberLimit = memberLimit;
            Members = members;
            Properties = properties;
        }
    }

    public abstract class LobbyService
    {
        protected Lobby _currentLobby;
        public Lobby CurrentLobby => _currentLobby;

        protected const int DefaultMemberLimit = 4;

        public event Action<Lobby> OnLobbyCreated;
        public event Action<Lobby> OnLobbyEnter;
        public event Action OnLobbyLeave;
        public event Action OnLobbyGameServerSet;

        public abstract void Shutdown();
        public abstract Task<Lobby[]> SearchLobbiesAsync();
        public abstract Task<Lobby> CreateLobbyAsync();
        public abstract Task<Lobby> JoinLobbyAsync(string lobbyId);
        public abstract void StartLobby();
        public abstract void LeaveLobby();

        protected void RaiseOnLobbyCreated(Lobby lobby) => OnLobbyCreated?.Invoke(lobby);
        protected void RaiseOnLobbyEnter(Lobby lobby) => OnLobbyEnter?.Invoke(lobby);
        protected void RaiseOnLobbyLeave() => OnLobbyLeave?.Invoke();
        protected void RaiseOnLobbyGameServerSet() => OnLobbyGameServerSet?.Invoke();
    }
}