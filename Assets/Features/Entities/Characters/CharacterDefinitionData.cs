using UnityEngine;

namespace FishFlingers.Entities
{
    public abstract class CharacterDefinitionData : EntityDefinitionData
    {
        [SerializeField] protected CharacterPhysicsSettings _characterPhysicsSettings;
        public CharacterPhysicsSettings CharacterPhysicsSettings => _characterPhysicsSettings;
    }
}