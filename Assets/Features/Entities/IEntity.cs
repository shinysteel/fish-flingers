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

        EntityHealthModule HealthModule { get; }
        EntityDefeatModule DefeatModule { get; }
        EntityLifecycleModule LifecycleModule { get; }
        EntityEffectsModule EffectsModule { get; }

        Transform Transform { get; }
        Rigidbody Rigidbody { get; }

        void AddForce(Vector3 force);
        void AddTorque(Vector3 torque);
    }
}