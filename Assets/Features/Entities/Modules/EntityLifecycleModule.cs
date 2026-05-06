using UnityEngine;

namespace FishFlingers.Entities
{
    public class EntityLifecycleModule
    {
        private EntityLifecycleSettings _settings;

        private float _spawnTime;

        public bool InGracePeriod => Time.time - _spawnTime < _settings.GracePeriod;

        public EntityLifecycleModule(IEntity entity)
        {
            _settings = entity.EntityDefinitionData.EntityLifecycleSettings;

            _spawnTime = Time.time;
        }
    }
}