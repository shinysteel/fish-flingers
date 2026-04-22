using System;
using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "CharacterPhysicsSettings", menuName = "Settings/Entities/CharacterPhysicsSettings")]
    public class CharacterPhysicsSettings : ScriptableObject
    {
        [SerializeField] private CharacterContactDetectionSettings _contactDetection;

        public CharacterContactDetectionSettings ContactDetection => _contactDetection;
    }

    [Serializable]
    public class CharacterContactDetectionSettings
    {
        [SerializeField] private float _castRadius = 0.125f;
        [SerializeField] private float _castDistance = 0.05f;
        [SerializeField] private LayerMask _groundedMask;
        [SerializeField] private LayerMask _floatingMask;

        public float CastRadius => _castRadius;
        public float CastDistance => _castDistance;
        public LayerMask GroundedMask => _groundedMask;
        public LayerMask FloatingMask => _floatingMask;
    }
}