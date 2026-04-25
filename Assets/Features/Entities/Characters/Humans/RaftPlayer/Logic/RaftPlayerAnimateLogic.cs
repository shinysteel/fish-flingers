using UnityEngine;
using System.Threading.Tasks;
using PrimeTween;
using ShinyOwl.Common;
using FishFlingers.Items;
using System;

namespace FishFlingers.Entities
{
    public class RaftPlayerAnimateLogic
    {
        private RaftPlayer _player;

        private StateAnimationEvents _attackStateAnimationEvents;

        public StateAnimationEvents AttackStateAnimationEvents => _attackStateAnimationEvents;

        private const string IsMovingBoolName = "IsMoving";
        private const string IsHoldingItemBoolName = "IsHoldingItem";
        private const string IsAttackingBoolName = "IsAttacking";
        private const string AttackStateName = "Attack";
        private const string AttackTriggerName = "Attack";

        private enum Layer
        {
            Base,
            RightArm
        }

        public RaftPlayerAnimateLogic(RaftPlayer player)
        {
            _player = player;

            _attackStateAnimationEvents = new StateAnimationEvents(AttackStateName)
            {
                new StateAnimationEvent(0.3f, () => _player.HeldInventoryItemLogic.HeldModel?.SetTrailEmitting(true)),
                new StateAnimationEvent(0.7f, () => _player.HeldInventoryItemLogic.HeldModel?.SetTrailEmitting(false)),
            };
        }

        public void Tick()
        {
            if (_player.isOwner)
            {
                bool isMoving = _player.InputLogic.MoveDirection != Vector3.zero;
                bool isHoldingItem = _player.Hotbar.SelectedSlot.InventoryItem != null;
                bool isAttacking = _player.AttackLogic.AttackState > RaftPlayerAttackState.None;

                _player.CharacterModel.Animator.SetBool(IsMovingBoolName, isMoving);
                _player.CharacterModel.Animator.SetBool(IsHoldingItemBoolName, isHoldingItem);
                _player.CharacterModel.Animator.SetBool(IsAttackingBoolName, isAttacking);
            }
            
            AnimatorStateInfo info = _player.CharacterModel.Animator.GetCurrentAnimatorStateInfo(0);
            _attackStateAnimationEvents.Tick(info);
        }

        public void Attack()
        {
            _player.CharacterModel.SetTrigger(AttackTriggerName);
        }
    }
}