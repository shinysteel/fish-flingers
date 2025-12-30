using PrimeTween;
using UnityEngine;

namespace FishFlingers.UI.Transitions
{
    [CreateAssetMenu(fileName = "TransitionManagerConfig", menuName = "Configs/Managers/UI/TransitionManagerConfig")]
    public class TransitionManagerConfig : ScriptableObject
    {
        [SerializeField] private float _duration;
        [SerializeField] private Ease _ease;

        public float Duration => _duration;
        public Ease Ease => _ease;
    }
}