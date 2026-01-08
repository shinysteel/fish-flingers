using UnityEngine;
using FishFlingers.UI;
using FishFlingers.Environments;
using FishFlingers.Entities;

namespace FishFlingers.Pools
{
    [CreateAssetMenu(fileName = "PoolManagerConfig", menuName = "Configs/Managers/PoolManagerConfig")]
    public class PoolManagerConfig : ScriptableObject
    {
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private LobbyEntry _lobbyEntryPrefab;

        public Tile TilePrefab => _tilePrefab;
        public LobbyEntry LobbyEntryPrefab => _lobbyEntryPrefab;
    }
}