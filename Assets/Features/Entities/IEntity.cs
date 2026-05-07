using FishFlingers.Environments;
using FishFlingers.States;
using UnityEngine;

namespace FishFlingers.Entities
{
    public interface IEntity
    {
        // Ordered to simplify NetEntity and Entity's implementations

        void Initialise(GameplayContext context);

        EntityDefinitionData EntityDefinitionData { get; }
        EntityModel EntityModel { get; }

        bool IsSpawned { get; }
        bool IsOwner { get; }
        Transform Transform { get; }

        EntityHealthModule EntityHealthModule { get; }
        EntityDefeatModule EntityDefeatModule { get; }
        EntityLifecycleModule EntityLifecycleModule { get; }
        EntityEffectsModule EntityEffectsModule { get; }
        EntityPhysicsModule EntityPhysicsModule { get; }
    }
}