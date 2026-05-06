using UnityEngine;

namespace FishFlingers.Entities
{
    public class EntityEffectsModule
    {
        protected IEntity _entity;

        public EntityEffectsModule(IEntity entity)
        {
            _entity = entity;
        }

        // AnimateHurt is intentionally not linked to change in health, since some entities aren't damageable like RaftPlayer
        public virtual void AnimateHurt()
        { }
    }
}