using System;
using System.Threading.Tasks;
using UnityEngine;

namespace FishFlingers.Entities
{
    public class EntityDefeatModule
    {
        private EntityManager _entityManager;

        private IEntity _entity;

        private bool _isDefeated;

        public event Action OnDefeated;

        public EntityDefeatModule(IEntity entity)
        {
            _entityManager = GameManager.Instance.Get<EntityManager>();
            
            _entity = entity;

            if (_entity.EntityModel != null)
            {
                _entity.EntityModel.Material.SetFloat(EntityModel.DefeatBlendName, 0f);
            }
        }

        public void Defeat()
        {
            if (_isDefeated)
            {
                return;
            }

            _isDefeated = true;

            _entity.RagdollModule.SetEnabled(true);

            _entity.EntityModel.Material.SetFloat(EntityModel.DefeatBlendName, 1f);

            OnDefeated?.Invoke();

            _ = DespawnAsync();
        }

        private async Task DespawnAsync()
        {
            await Task.Delay(Mathf.RoundToInt(_entity.EntityData.DefeatTime * 1000f));

            _entityManager.Despawn(_entity);
        }
    }
}
