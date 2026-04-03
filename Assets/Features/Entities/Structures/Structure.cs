using FishFlingers.Items;
using FishFlingers.Saving;
using Newtonsoft.Json;
using System;
using UnityEngine;
using ShinyOwl.Common.Utils;

namespace FishFlingers.Entities
{
    public class StructureSave
    {
        [JsonProperty] private SimpleVector2Int _cell = new();
        [JsonProperty] public EntityId StructureId { get; private set; }
        [JsonProperty] public string JsonData { get; private set; }

        [JsonIgnore] public Vector2Int Cell
        {
            get => _cell.ToVector2Int();
            set => _cell = new SimpleVector2Int(value);
        }

        public StructureSave()
        { }

        public StructureSave(Vector2Int cell, EntityId structureId, string jsonData)
        {
            Cell = cell;
            StructureId = structureId;
            JsonData = jsonData;
        }

        public StructureSave(Structure structure) : this(structure.Cell, structure.StructureData.Id, structure.GetJsonData())
        { }
    }

    public abstract class Structure : NetEntity
    {
        private Vector2Int _cell;
        public Vector2Int Cell => _cell;

        public StructureData StructureData => (StructureData)_entityData;

        public virtual string GetJsonData()
        {
            return null;
        }

        public virtual void LoadJsonData(string json)
        { }

        public void SetCell(Vector2Int cell)
        {
            _cell = cell;
        }
    }

    public abstract class Structure<T> : Structure where T : StructureData
    {
        public T Data => (T)_entityData;
    }
}