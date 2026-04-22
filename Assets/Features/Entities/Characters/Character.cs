using UnityEngine;

namespace FishFlingers.Entities
{
    public abstract class Character : NetEntity
    {
        public CharacterData CharacterData => (CharacterData)_entityData;
        public CharacterModel CharacterModel => (CharacterModel)_entityModel;

        [SerializeField] protected Collider _characterCollider;
        public Collider CharacterCollider => _characterCollider;


        protected CharacterDefeatLogic _defeatLogic;
        protected CharacterPhysicsLogic _physicsLogic;
        private CharacterRagdollLogic _ragdollLogic;

        public CharacterPhysicsLogic PhysicsLogic => _physicsLogic;
        public CharacterRagdollLogic RagdollLogic => _ragdollLogic;

        protected override void OnSpawned()
        {
            _defeatLogic = new CharacterDefeatLogic(this);

            // Some characters setup their own inherited logic script
            _physicsLogic ??= new CharacterPhysicsLogic(this);
            
            _ragdollLogic = new CharacterRagdollLogic(this);

            base.OnSpawned();
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