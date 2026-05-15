using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "RaftPlayerInteractSettings", menuName = "Settings/Entities/RaftPlayerInteractSettings")]
    public class RaftPlayerInteractSettings : ScriptableObject
    {
        [SerializeField] private LayerMask _mask;
        [SerializeField] private float _radius = 1f;

        public LayerMask Mask => _mask;
        public float Radius => _radius;
    }
}