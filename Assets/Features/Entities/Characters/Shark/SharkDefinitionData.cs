using FishFlingers.Hitboxes;
using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "SharkDefinitionData", menuName = "Data/Entities/Characters/SharkDefinitionData")]
    public class SharkDefinitionData : CharacterDefinitionData
    {
        [SerializeField] private HitboxData _biteHitboxData;

        public HitboxData BiteHitboxData => _biteHitboxData;
    }
}