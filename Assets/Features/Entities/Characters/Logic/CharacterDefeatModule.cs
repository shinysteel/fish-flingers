using FishFlingers.Items;
using PrimeTween;
using ShinyOwl.Common;
using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace FishFlingers.Entities
{
    public class CharacterDefeatModule : EntityDefeatModule
    {
        private Character _character;
        private CharacterDefeatSettings _settings;


        private float _defeatTimer;

        private Tween _defeatTween;

        public CharacterDefeatModule(Character character) : base(character)
        {
            _character = character;
            _settings = (CharacterDefeatSettings)_character.EntityDefinitionData.EntityDefeatSettings;
        }

        public override void Tick()
        {
            if (!_character.isOwner)
            {
                return;
            }

            if (!_isDefeated)
            {
                return;
            }

            if (_defeatTween.isAlive)
            {
                return;
            }

            if (!_character.CharacterPhysicsModule.IsGrounded && !_character.CharacterPhysicsModule.InWater)
            {
                return;
            }

            _defeatTimer += Time.deltaTime;

            if (_defeatTimer < _settings.DefeatDuration)
            {
                return;
            }

            _defeatTween = Tween.Scale(_character.transform, endValue: Vector3.zero, duration: _settings.TweenDuration, ease: Ease.InBack)
                .OnComplete(Despawn);
        }

        public override void Defeat()
        {
            _defeatTimer = 0f;

            _character.CharacterModel.SetDefeated(true);

            _character.CharacterModel.Animator.Update(0f);

            _character.RagdollLogic.SetEnabled(true);

            _isDefeated = true;

            RaiseDefeated();
        }

        protected override void Despawn()
        {
            _character.RagdollLogic.SetEnabled(false);

            _character.CharacterModel.SetDefeated(false);

            // Simulate 1 second to have the character unblink
            _character.CharacterModel.Animator.Update(1f);

            base.Despawn();
        }
    }
}