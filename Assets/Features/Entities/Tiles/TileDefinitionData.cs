using FishFlingers.Environments;
using FishFlingers.Items;
using FishFlingers.States;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FishFlingers.Entities
{
    [Serializable]
    public class TileBobSettings
    {
        [SerializeField] private float _amplitude = 0.125f;
        [SerializeField] private float _noiseScale = 0.5f;
        [SerializeField] private float _timeScale = 0.25f;

        public float Amplitude => _amplitude;
        public float NoiseScale => _noiseScale;
        public float TimeScale => _timeScale;
    }

    [Serializable]
    public class TileSinkSettings
    {
        [SerializeField] private LayerMask _mask;
        [SerializeField] private float _radius = 1f;
        [SerializeField] private float _speed = 0.25f;

        public LayerMask Mask => _mask;
        public float Radius => _radius;
        public float Speed => _speed;
    }

    [CreateAssetMenu(fileName = "TileDefinitionData", menuName = "Data/Entities/TileDefinitionData")]
    public class TileDefinitionData : EntityDefinitionData, IBuildable
    {
        [SerializeField] private Recipe _recipe;
        [SerializeField] private TileBobSettings _bobSettings;
        [SerializeField] private TileSinkSettings _sinkSettings;

        public DefinitionData DefinitionData => this;
        public Recipe Recipe => _recipe;
        public TileBobSettings BobSettings => _bobSettings;
        public TileSinkSettings SinkSettings => _sinkSettings;

        public bool TryBuild(GameplayContext context, RaftPlayerTileTarget target)
        {
            if (target.Tile != null)
            {
                return false;
            }

            context.Raft.AddNetTileRpc(target.Cell, _id, _health, Random.Range(0, 4));

            return true;
        }
    }
}