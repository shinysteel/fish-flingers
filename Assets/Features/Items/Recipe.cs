using FishFlingers.Inventories;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace FishFlingers.Items
{
    [Serializable]
    public class RecipeRequirement
    {
        [SerializeField] private ItemId _itemId;
        [SerializeField] private int _count;

        public ItemId ItemId => _itemId;
        public int Count => _count;
    }

    [Serializable]
    public class Recipe
    {
        [SerializeField] private RecipeRequirement[] _requirements;

        public RecipeRequirement[] Requirements => _requirements;

        public List<ChangeParams> ToChangeParams()
        {
            List<ChangeParams> parameters = new();

            foreach (RecipeRequirement requirement in _requirements)
            {
                parameters.Add(new ChangeParams()
                {
                    ItemId = requirement.ItemId,
                    Count = requirement.Count
                });
            }

            return parameters;
        }
    }
}