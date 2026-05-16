using FishFlingers.Environments;
using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "IInteractableSettings", menuName = "Settings/Entities/IInteractableSettings")]
    public class IInteractableSettings : ScriptableObject
    {
        [SerializeField] private ActionHotkey _hotkey;
        [SerializeField] private int _priority;
        [SerializeField] private float _maxAngle = 45f;
        [SerializeField] private float _maxDistance = 0.5f;
        [SerializeField] private PropId _previewId;

        public ActionHotkey Hotkey => _hotkey;
        public int Priority => _priority;
        public float MaxAngle => _maxAngle;
        public float MaxDistance => _maxDistance;
        public PropId PreviewId => _previewId;
    }
}