using FishFlingers.Pools;
using PurrLobby;
using TMPro;
using UnityEngine;

namespace FishFlingers.UI
{
    public class LobbyEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _playerCountText;

        public void Setup(Lobby lobby)
        {
            _nameText.text = lobby.Name;
            _playerCountText.text = $"{lobby.Members.Count} / {lobby.MaxPlayers}";
        }
    }
}