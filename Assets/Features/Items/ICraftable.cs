using FishFlingers;
using FishFlingers.Items;
using UnityEngine;

namespace FishFlingers.Items
{
    public interface ICraftable
    {
        DefinitionData DefinitionData { get; }
        Recipe Recipe { get; }
    }
}