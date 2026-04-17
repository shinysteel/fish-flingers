using Newtonsoft.Json;
using ShinyOwl.Common.Utils;
using UnityEngine;

namespace FishFlingers.Entities
{
    public class TileSave
    {
        [JsonProperty] private SimpleVector2Int _cell = new();
        [JsonProperty] public int Health { get; private set; }

        [JsonIgnore]
        public Vector2Int Cell
        {
            get => _cell.ToVector2Int();
            set => _cell = new SimpleVector2Int(value);
        }

        public TileSave()
        { }

        public TileSave(Vector2Int cell, int health)
        {
            Cell = cell;
            Health = health;
        }

        public TileSave(Tile tile) : this(tile.Cell, tile.CurrentHealth)
        { }
    }
}