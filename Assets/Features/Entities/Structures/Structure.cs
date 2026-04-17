using FishFlingers.Items;
using FishFlingers.Saving;
using Newtonsoft.Json;
using System;
using UnityEngine;
using ShinyOwl.Common.Utils;

namespace FishFlingers.Entities
{
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

        protected override void OnSpawned()
        {
            base.OnSpawned();

            transform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
        }
    }

    public abstract class Structure<T> : Structure where T : StructureData
    {
        public T Data => (T)_entityData;
    }
}