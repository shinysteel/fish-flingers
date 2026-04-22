using UnityEngine;
using System;
using ShinyOwl.Common;

namespace FishFlingers.Entities
{
    public class CharacterDefeatLogic
    {
        private EntityManager _entityManager;

        private Character _character;

        private bool _isDefeated;

        private float _defeatTimer;

        public event Action OnDefeated;

        public CharacterDefeatLogic(Character character)
        {
            _entityManager = GameManager.Instance.Get<EntityManager>();

            _character = character;

            _character.HealthModule.OnChanged += HandleHealthChanged;
        }

        ~CharacterDefeatLogic()
        {
            if (_character != null)
            {
                _character.HealthModule.OnChanged -= HandleHealthChanged;
            }
        }

        public void Tick()
        {
            if (!_isDefeated)
            {
                return;
            }

            if (!_character.PhysicsLogic.IsGrounded && !_character.PhysicsLogic.InWater)
            {
                return;
            }
            
            _defeatTimer += Time.deltaTime;

            if (_defeatTimer < _character.CharacterData.CharacterDefeatSettings.Duration)
            {
                return;
            }

            _character.RagdollLogic.SetEnabled(false);

            _character.CharacterModel.Material.SetFloat(CharacterModel.DefeatBlendShaderPropertyName, 0f);

            _character.CharacterModel.Animator.SetBool(CharacterModel.IsDefeatedAnimatorBoolName, false);

            // Simulate 1 second to have the character unblink
            _character.CharacterModel.Animator.Update(1f);

            _entityManager.Despawn(_character);
        }

        private void HandleHealthChanged(int previous, int current)
        {
            if (_isDefeated)
            {
                return;
            }

            if (current > 0)
            {
                return;
            }

            Defeat();
        }

        public void Defeat()
        {
            _isDefeated = true;

            _defeatTimer = 0f;

            _character.CharacterModel.Material.SetFloat(CharacterModel.DefeatBlendShaderPropertyName, 1f);

            _character.CharacterModel.Animator.SetBool(CharacterModel.IsDefeatedAnimatorBoolName, true);

            _character.CharacterModel.Animator.Update(0f);
            
            _character.RagdollLogic.SetEnabled(true);

            OnDefeated?.Invoke();
        }
    }
}