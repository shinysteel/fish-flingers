using FishFlingers.Hitboxes;
using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "WaveSignDefinitionData", menuName = "Data/Entities/Structures/WaveSignDefinitionData")]
    public class WaveSignDefinitionData : StructureDefinitionData
    {
        [SerializeField] private HitboxData _slamHitboxData;

        public HitboxData SlamHitboxData => _slamHitboxData;
    }
}