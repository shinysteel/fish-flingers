using FishFlingers.Items;
using FishFlingers.States;
using UnityEngine;

namespace FishFlingers.Entities
{
    public interface IBuildable : ICraftable
    {
        bool TryBuild(GameplayContext context, RaftPlayerTileTarget target);
    }
}