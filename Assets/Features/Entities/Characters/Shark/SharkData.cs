using FishFlingers.Hitboxes;
using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "SharkData", menuName = "Data/Entities/Characters/SharkData")]
    public class SharkData : CharacterData
    {
        [SerializeField] private HitboxData _biteHitboxData;

        public HitboxData BiteHitboxData => _biteHitboxData;
    }
}