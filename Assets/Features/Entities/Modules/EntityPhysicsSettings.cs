using System;
using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "EntityPhysicsSettings", menuName = "Settings/Entities/EntityPhysicsSettings")]
    public class EntityPhysicsSettings : ScriptableObject
    {
        [SerializeField] private EntityGroundDetectionSettings _groundDetection;

        public EntityGroundDetectionSettings GroundDetection => _groundDetection;
    }

    [Serializable]
    public class EntityGroundDetectionSettings
    {
        [SerializeField] private LayerMask _mask;
        [SerializeField] private float _castRadius = 0.125f;
        [SerializeField] private float _castDist = 0.05f;

        public LayerMask Mask => _mask;
        public float CastRadius => _castRadius;
        public float CastDist => _castDist;
    }
}