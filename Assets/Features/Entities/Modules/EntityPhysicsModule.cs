using FishFlingers.Audio;
using FishFlingers.Cameras;
using ShinyOwl.Common;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace FishFlingers.Entities
{
    public class EntityPhysicsModule
    {
        protected CameraManager _cameraManager;
        protected AudioManager _audioManager;

        protected IEntity _entity;
        protected Rigidbody _rigidbody;

        protected EntityPhysicsSettings _entityPhysicsSettings;

        public Rigidbody Rigidbody => _rigidbody;

        public EntityPhysicsModule(IEntity entity, Rigidbody rigidbody)
        {
            _cameraManager = GameManager.Instance.Get<CameraManager>();
            _audioManager = GameManager.Instance.Get<AudioManager>();

            _entity = entity;
            _rigidbody = rigidbody;

            _entityPhysicsSettings = _entity.EntityDefinitionData.EntityPhysicsSettings;
        }

        public virtual void Tick()
        { }

        public virtual void FixedTick()
        { }
    }
}