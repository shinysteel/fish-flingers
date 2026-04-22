using FishFlingers.Entities;
using UnityEngine;

namespace FishFlingers.Environments
{
    public class Ocean : MonoBehaviour
    {
        [SerializeField] private BoxCollider _boxCollider;

        [SerializeField, Range(0f, 1f)] private float _submergePercent = 0.5f;
        [SerializeField] private Vector3 _linearDrag = new Vector3(5f, 1f, 5f);
        [SerializeField] private float _angularDrag = 2.5f;
        [SerializeField] private float _currentSpeed = 0.25f;

        private void OnTriggerStay(Collider collider)
        {
            if (!collider.gameObject.TryGetComponent(out IEntity entity))
            {
                return;
            }
            
            BuoyancyOnTriggerStay(collider, entity);
            CurrentOnTriggerStay(entity);
            DragOnTriggerStay(collider, entity);
        }

        private float GetBuoyancyFactor(Collider collider)
        {
            float surfaceY = _boxCollider.bounds.max.y;
            float depth = surfaceY - collider.bounds.min.y;
            return Mathf.Clamp01(depth / collider.bounds.size.y);
        }

        private void BuoyancyOnTriggerStay(Collider collider, IEntity entity)
        { 
            // More mass = more force
            float strength = entity.Rigidbody.mass * Physics.gravity.magnitude / _submergePercent;
            float factor = GetBuoyancyFactor(collider);
            Vector3 force = Vector3.up * strength * factor;

            // Push the entity upwards to simulate floating
            entity.Rigidbody.AddForce(force);
        }

        // Current is referring to motion in water
        private void CurrentOnTriggerStay(IEntity entity)
        {
            if (!entity.Rigidbody.isKinematic)
            {
                entity.Rigidbody.MovePosition(entity.Rigidbody.position + Vector3.back * _currentSpeed * Time.fixedDeltaTime);
            }
        }

        private void DragOnTriggerStay(Collider collider, IEntity entity)
        {
            // Drag stops the entity being 'launched' from buoyancy, and
            // slows it down on the XZ plane
            entity.Rigidbody.AddForce(Vector3.Scale(-entity.Rigidbody.linearVelocity, _linearDrag), ForceMode.Acceleration);
            entity.Rigidbody.AddTorque(-entity.Rigidbody.angularVelocity * _angularDrag, ForceMode.Acceleration);
        }
    }
}