using PrimeTween;
using UnityEngine;

namespace FishFlingers.Entities
{
    public class EntityRagdollModule
    {
        private IEntity _entity;
        private RigidbodyConstraints _rigidbodyConstraints;

        public EntityRagdollModule(IEntity entity)
        {
            _entity = entity;

            _rigidbodyConstraints = _entity.Rigidbody.constraints;
        }

        public void SetEnabled(bool enabled)
        {
            _entity.Rigidbody.isKinematic = !enabled;
            _entity.Rigidbody.constraints = enabled ? RigidbodyConstraints.None : _rigidbodyConstraints;

            // Some entities do not have an animator
            if (_entity.EntityModel != null)
            {
                _entity.EntityModel.Animator.enabled = !enabled;
            }

            if (!enabled)
            {
                Tween.StopAll(_entity.Rigidbody.transform);
            }
        }
    }
}