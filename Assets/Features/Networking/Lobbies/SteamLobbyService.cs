using PurrLobby;
using ShinyOwl.Common;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace FishFlingers.Networking
{
    public class SteamLobbyService : LobbyService
    {
        private CallResult<LobbyMatchList_t> _lobbyMatchListListener;
        private CallResult<LobbyCreated_t> _lobbyCreatedListener;
        private CallResult<LobbyEnter_t> _lobbyEnterListener;

        // Replace this once the player can specify what lobby they want to create
        private const ELobbyType DefaultLobbyType = ELobbyType.k_ELobbyTypePublic;

        private const string NameKey = "name";

        public override void Shutdown()
        {
            _lobbyCreatedListener?.Dispose();
            _lobbyEnterListener?.Dispose();

            LeaveLobby();
        }

        private bool IsAvailable()
        {
            try
            {
                InteropHelp.TestIfAvailableClient();
                return true;
            }
            catch
            {
                return false;
            }
        } 

        // It's no good storing the same thing in two places, so instead of storing the lobby id both here
        // and in _currentLobby.LobbyId, let's just point to it. The tradeoff is we need to create it for each request
        private CSteamID GetLobbyId()
        {
            return new CSteamID(ulong.Parse(_currentLobby.LobbyId));
        }

        public override async Task<Lobby[]> SearchLobbiesAsync()
        {
            if (!IsAvailable())
            {
                return null;
            }

            // Tell steam to search for lobbies
            SteamAPICall_t call = SteamMatchmaking.RequestLobbyList();

            // Listen for the result
            TaskCompletionSource<Lobby[]> tcs = new();
            _lobbyMatchListListener ??= CallResult<LobbyMatchList_t>.Create();
            _lobbyMatchListListener.Set(call, (LobbyMatchList_t lobbyMatchList, bool ioFailure) =>
            {
                Lobby[] lobbies = new Lobby[lobbyMatchList.m_nLobbiesMatching];

                for (int i = 0; i < lobbies.Length; i++)
                {
                    CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
                    string name = SteamMatchmaking.GetLobbyData(lobbyId, NameKey);
                    CSteamID ownerId = SteamMatchmaking.GetLobbyOwner(lobbyId);
                    int memberLimit = SteamMatchmaking.GetLobbyMemberLimit(lobbyId);
                    List<LobbyMember> members = GetLobbyMembers(lobbyId);
                    Dictionary<string, string> properties = GetLobbyProperties(lobbyId);

                    lobbies[i] = new Lobby(name, lobbyId.ToString(), ownerId.ToString(), memberLimit, members, properties);
                }

                tcs.SetResult(lobbies);
            });

            return await tcs.Task;
        }

        public override async Task<Lobby> CreateLobbyAsync()
        {
            if (!IsAvailable())
            {
                return null;
            }

            CSteamID lobbyId = CSteamID.Nil;

            // Tell steam to create the lobby
            SteamAPICall_t call = SteamMatchmaking.CreateLobby(DefaultLobbyType, DefaultMemberLimit);

            // Listen for the result
            TaskCompletionSource<bool> tcs = new();
            _lobbyCreatedListener ??= CallResult<LobbyCreated_t>.Create();
            _lobbyCreatedListener.Set(call, (LobbyCreated_t lobbyCreated, bool ioFailure) =>
            {
                bool result = lobbyCreated.m_eResult == EResult.k_EResultOK && !ioFailure;
                if (result)
                {
                    lobbyId = new CSteamID(lobbyCreated.m_ulSteamIDLobby);
                }

                tcs.SetResult(result);
            });

            if (!await tcs.Task)
            {
                return null;
            }

            string lobbyName = $"{SteamFriends.GetPersonaName()}'s Lobby";
            SteamMatchmaking.SetLobbyData(lobbyId, NameKey, lobbyName);

            CSteamID ownerId = SteamMatchmaking.GetLobbyOwner(lobbyId);
            List<LobbyMember> members = GetLobbyMembers(lobbyId);
            Dictionary<string, string> properties = GetLobbyProperties(lobbyId);

            _currentLobby = new Lobby(lobbyName, lobbyId.ToString(), ownerId.ToString(), DefaultMemberLimit, members, properties);

            RaiseOnLobbyCreated(_currentLobby);
            RaiseOnLobbyEnter(_currentLobby);
            return _currentLobby;
        }

        public override async Task<Lobby> JoinLobbyAsync(string lobbyId)
        {
            if (!IsAvailable())
            {
                return null;
            }

            CSteamID cLobbyId = new CSteamID(ulong.Parse(lobbyId));

            // Tell steam to join a lobby
            SteamAPICall_t call = SteamMatchmaking.JoinLobby(cLobbyId);

            // Listen for the result
            TaskCompletionSource<bool> tcs = new();
            _lobbyEnterListener ??= CallResult<LobbyEnter_t>.Create();
            _lobbyEnterListener.Set(call, (LobbyEnter_t lobbyEnter, bool ioFailure) =>
            {
                bool result = lobbyEnter.m_EChatRoomEnterResponse == (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess;
                tcs.SetResult(result);
            });

            if (!await tcs.Task)
            {
                return null;
            }

            string lobbyName = SteamMatchmaking.GetLobbyData(cLobbyId, NameKey);
            CSteamID ownerId = SteamMatchmaking.GetLobbyOwner(cLobbyId);
            int memberLimit = SteamMatchmaking.GetLobbyMemberLimit(cLobbyId);
            List<LobbyMember> members = GetLobbyMembers(cLobbyId);
            Dictionary<string, string> properties = GetLobbyProperties(cLobbyId);

            _currentLobby = new Lobby(lobbyName, lobbyId, ownerId.ToString(), memberLimit, members, properties);

            RaiseOnLobbyEnter(_currentLobby);
            return _currentLobby;
        }

        public override void StartLobby()
        {
            if (!IsAvailable())
            {
                return;
            }

            if (_currentLobby == null)
            {
                return;
            }

            SteamMatchmaking.SetLobbyGameServer(GetLobbyId(), 0, 0, SteamUser.GetSteamID());
            
            RaiseOnLobbyStart();
        }

        public override void LeaveLobby()
        {
            if (!IsAvailable())
            {
                return;
            }

            if (_currentLobby == null)
            {
                return;
            }

            SteamMatchmaking.LeaveLobby(GetLobbyId());
            _currentLobby = null;

            RaiseOnLobbyLeave();
        }

        public override bool IsLobbyOwner(Lobby lobby)
        {
            return lobby.LobbyId == SteamUser.GetSteamID().ToString();
        }

        private List<LobbyMember> GetLobbyMembers(CSteamID lobbyId)
        {
            List<LobbyMember> members = new(); 

            for (int i = 0; i < SteamMatchmaking.GetNumLobbyMembers(lobbyId); i++)
            {
                CSteamID memberId = SteamMatchmaking.GetLobbyMemberByIndex(lobbyId, i);
                string displayName = SteamFriends.GetFriendPersonaName(memberId);

                members.Add(new LobbyMember(memberId.ToString(), displayName));
            }

            return members;
        }

        private Dictionary<string, string> GetLobbyProperties(CSteamID lobbyId)
        {
            Dictionary<string, string> properties = new();
            int count = SteamMatchmaking.GetLobbyDataCount(lobbyId);

            int keySize = 256;
            int valueSize = 256;

            for (int i = 0; i < count; i++)
            {
                if (SteamMatchmaking.GetLobbyDataByIndex(lobbyId, i, out string key, keySize, out string value, valueSize))
                {
                    key = key.TrimEnd('\0');
                    value = value.TrimEnd('\0');
                    properties[key] = value;
                }
            }

            return properties;
        }
    }
}