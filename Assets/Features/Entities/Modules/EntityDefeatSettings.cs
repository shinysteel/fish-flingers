using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "EntityDefeatSettings", menuName = "Settings/Entities/EntityDefeatSettings")]
    public class EntityDefeatSettings : ScriptableObject
    {
        [SerializeField] private float _duration = 2.5f;

        public float Duration => _duration;
    }
}