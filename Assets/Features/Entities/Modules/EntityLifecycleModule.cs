using UnityEngine;

namespace FishFlingers.Entities
{
    public class EntityLifecycleModule
    {
        private EntityLifecycleSettings _settings;

        private float _spawnTime;

        public float TimeAlive => Time.time - _spawnTime;
        public bool InGracePeriod => TimeAlive < _settings.GracePeriod;

        public EntityLifecycleModule(IEntity entity)
        {
            _settings = entity.EntityDefinitionData.EntityLifecycleSettings;

            _spawnTime = Time.time;
        }
    }
}