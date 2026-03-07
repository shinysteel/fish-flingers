using FishFlingers.Items;
using FishFlingers.States;
using UnityEngine;

namespace FishFlingers.Entities
{
    public interface IBuildable
    {
        EntityData EntityData { get; }
        Recipe Recipe { get; }

        bool TryBuild(GameplayContext context, RaftPlayerTarget target);
    }
}