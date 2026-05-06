using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Data/Entities/Characters/CharacterData")]
    public abstract class CharacterDefinitionData : EntityDefinitionData
    {
        [SerializeField] protected CharacterPhysicsSettings _characterPhysicsSettings;
        public CharacterPhysicsSettings CharacterPhysicsSettings => _characterPhysicsSettings;
    }
}