using FishFlingers.Entities;
using UnityEngine;

namespace FishFlingers.Environments
{
    public class Ocean : MonoBehaviour
    {
        [SerializeField] private BoxCollider _boxCollider;

        [SerializeField] private float _buoyancyStrength = 30f;

        [SerializeField] private float _currentSpeed = 0.5f;

        private void OnTriggerStay(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out IEntity entity))
            {
                return;
            }
            
            BuoyancyOnTriggerStay(other, entity);
            CurrentOnTriggerStay(entity);
        }

        private void BuoyancyOnTriggerStay(Collider other, IEntity entity)
        {
            float surfaceY = _boxCollider.bounds.max.y;
            float depth = surfaceY - other.bounds.min.y;
            Vector3 force = Vector3.up * _buoyancyStrength * entity.EntityData.BuoyancyFactor * depth;

            // Push the entity upwards to simulate floating
            entity.Rigidbody.AddForce(force);
        }

        // Current is refering to motion in water
        private void CurrentOnTriggerStay(IEntity entity)
        {
            if (!entity.Rigidbody.isKinematic)
            {
                entity.Rigidbody.MovePosition(entity.Rigidbody.position + Vector3.back * _currentSpeed * Time.fixedDeltaTime);
            }
        }
    }
}