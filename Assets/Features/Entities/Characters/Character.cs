using PrimeTween;
using PurrNet;
using UnityEngine;

namespace FishFlingers.Entities
{
    public abstract class Character : NetEntity
    {
        public CharacterDefinitionData CharacterDefinitionData => (CharacterDefinitionData)_entityDefinitionData;
        public CharacterModel CharacterModel => (CharacterModel)_entityModel;

        [SerializeField] protected Collider _characterCollider;
        public Collider CharacterCollider => _characterCollider;

        private CharacterRagdollLogic _ragdollLogic;
        protected CharacterPhysicsLogic _physicsLogic;
        protected CharacterStunLogic _stunLogic;

        public CharacterRagdollLogic RagdollLogic => _ragdollLogic;
        public CharacterPhysicsLogic PhysicsLogic => _physicsLogic;

        protected override void OnInitializeModules()
        {
            _effectsModule = new CharacterEffectsModule(this);

            base.OnInitializeModules();
        }

        protected override EntityDefeatModule CreateDefeatModule()
        {
            return new CharacterDefeatModule(this);
        }

        protected override void OnSpawned()
        {
            _ragdollLogic = new CharacterRagdollLogic(this);

            // Some characters setup their own inherited logic script
            _physicsLogic ??= new CharacterPhysicsLogic(this);

            _stunLogic = new CharacterStunLogic();

            base.OnSpawned();
        }

        protected override void Update()
        {
            base.Update();

            if (!isOwner)
            {
                return;
            }

            if (!isFullySpawned)
            {
                return;
            }
            
            _physicsLogic.Tick();
            _stunLogic.Tick();
        }

        protected virtual void FixedUpdate()
        {
            if (!isOwner)
            {
                return;
            }

            if (!isFullySpawned)
            {
                return;
            }

            _physicsLogic.FixedTick();
        }

        [ServerRpc]
        public void StunRpc(float duration)
        {
            _stunLogic.Stun(duration);
        }
    }

    public abstract class Character<T> : Character where T : CharacterDefinitionData
    {
        public T DefinitionData => (T)_entityDefinitionData;
    }
}