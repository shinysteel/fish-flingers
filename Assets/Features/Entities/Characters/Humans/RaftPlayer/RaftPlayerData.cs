using System;
using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "RaftPlayerData", menuName = "Data/Entities/Characters/RaftPlayerData")]
    public class RaftPlayerData : CharacterData
    {
        [SerializeField] private RaftPlayerPhysicsSettings _physicsSettings;
        [SerializeField] private RaftPlayerInteractSettings _interactSettings;
        [SerializeField] private RaftPlayerTileTargetSettings _tileTargetSettings;

        public RaftPlayerPhysicsSettings PhysicsSettings => _physicsSettings;
        public RaftPlayerInteractSettings InteractSettings => _interactSettings;
        public RaftPlayerTileTargetSettings TileTargetSettings => _tileTargetSettings;
    }
}