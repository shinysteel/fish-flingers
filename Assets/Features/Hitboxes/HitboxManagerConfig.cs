using UnityEngine;

namespace FishFlingers.Hitboxes
{
    [CreateAssetMenu(fileName = "HitboxManagerConfig", menuName = "Configs/Managers/HitboxManagerConfig")]
    public class HitboxManagerConfig : ScriptableObject
    {
        [SerializeField] private bool _drawGizmos;

        public bool DrawGizmos => _drawGizmos;
    }
}