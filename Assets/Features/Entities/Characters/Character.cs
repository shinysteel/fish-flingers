using PrimeTween;
using UnityEngine;

namespace FishFlingers.Entities
{
    public abstract class Character : NetEntity
    {
        public CharacterData CharacterData => (CharacterData)_entityData;
        public CharacterModel CharacterModel => (CharacterModel)_entityModel;

        [SerializeField] protected Collider _characterCollider;
        public Collider CharacterCollider => _characterCollider;

        private CharacterRagdollLogic _ragdollLogic;
        protected CharacterPhysicsLogic _physicsLogic;
        protected CharacterDefeatLogic _defeatLogic;

        public CharacterRagdollLogic RagdollLogic => _ragdollLogic;
        public CharacterPhysicsLogic PhysicsLogic => _physicsLogic;

        protected override void OnSpawned()
        {
            _ragdollLogic = new CharacterRagdollLogic(this);

            // Some characters setup their own inherited logic script
            _physicsLogic ??= new CharacterPhysicsLogic(this);

            _defeatLogic = new CharacterDefeatLogic(this);

            _healthModule.OnChanged += HandleHealthChangned;

            base.OnSpawned();
        }

        protected override void OnDespawned()
        {
            _healthModule.OnChanged -= HandleHealthChangned;

            base.OnDespawned();
        }

        private void HandleHealthChangned(int previous, int current)
        {
            if (current == 0)
            {
                return;
            }

            if (current < previous)
            {
                CharacterModel.AnimateHurt();
            }
        }

        protected virtual void Update()
        {
            if (!isFullySpawned)
            {
                return;
            }

            if (!isOwner)
            {
                return;
            }

            _defeatLogic.Tick();
            _physicsLogic.Tick();
        }

        protected virtual void FixedUpdate()
        {
            if (!isFullySpawned)
            {
                return;
            }

            if (!isOwner)
            {
                return;
            }

            _physicsLogic.FixedTick();
        }
    }

    public abstract class Character<T> : Character where T : CharacterData
    {
        public T Data => (T)_entityData;
    }
}