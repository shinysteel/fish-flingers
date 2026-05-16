using FishFlingers.Items;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using ShinyOwl.Common.Utils;
using FishFlingers.Pools;
using FishFlingers.States;

namespace FishFlingers.UI
{
    public class RequirementPromptUI : InteractPromptUI
    {
        private List<RequirementPromptItem> _items = new();

        public void SetupRequirement(GameplayContext context, Recipe recipe)
        {
            Utils.Collections.ResizeList(_items, recipe.Requirements.Length,
                createElement: () => _poolManager.GetTypedPoolable<RequirementPromptItem>(new SpawnParams() { Parent = transform }),
                removeElement: (RequirementPromptItem item) => _poolManager.ReturnTypedPoolable(item),
                processElement: (RequirementPromptItem item, int index) => item.Setup(context, recipe.Requirements[index]));
        }
    }
}