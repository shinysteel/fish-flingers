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
        public Character Character => (Character)_entity;
        public CharacterDefeatSettings CharacterDefeatSettings => (CharacterDefeatSettings)_entityDefeatSettings;

        
        private float _defeatTimer;

        private Tween _defeatTween;

        public CharacterDefeatModule(Character character) : base(character)
        { }

        public override void DefeatTick()
        {
            if (!_isDefeated)
            {
                return;
            }

            if (_defeatTween.isAlive)
            {
                return;
            }

            if (!Character.PhysicsLogic.IsGrounded && !Character.PhysicsLogic.InWater)
            {
                return;
            }

            _defeatTimer += Time.deltaTime;

            if (_defeatTimer < CharacterDefeatSettings.DefeatDuration)
            {
                return;
            }

            _defeatTween = Tween.Scale(Character.transform, endValue: Vector3.zero, duration: CharacterDefeatSettings.TweenDuration, ease: Ease.InBack).OnComplete(Despawn);
        }

        public override void Defeat()
        {
            _defeatTimer = 0f;

            Character.CharacterModel.SetDefeated(true);

            Character.CharacterModel.Animator.Update(0f);

            Character.RagdollLogic.SetEnabled(true);

            base.Defeat();
        }

        public override void Despawn()
        {
            Character.RagdollLogic.SetEnabled(false);

            Character.CharacterModel.SetDefeated(false);

            // Simulate 1 second to have the character unblink
            Character.CharacterModel.Animator.Update(1f);

            base.Despawn();
        }
    }
}