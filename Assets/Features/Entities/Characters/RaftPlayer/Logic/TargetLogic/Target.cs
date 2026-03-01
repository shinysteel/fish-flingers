using UnityEngine;

namespace FishFlingers.Entities
{
    public class Target : MonoBehaviour
    {
        [SerializeField] private Transform _visualTransform;

        public void SetVisualScale(Vector3 scale)
        {
            _visualTransform.localScale = scale;
        }
    }
}