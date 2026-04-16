using UnityEngine;
using System.Threading.Tasks;
using PrimeTween;

namespace FishFlingers.Entities
{
    public class RaftPlayerAnimateLogic
    {
        private RaftPlayer _player;
        private CharacterModel _model;

        private const string IsMovingBoolName = "IsMoving";
        private const string IsHoldingItemBoolName = "IsHoldingItem";
        private const string IsAttackingBoolName = "IsAttacking";
        private const string AttackStateName = "Attack";

        private enum Layer
        {
            Base,
            RightArm
        }

        public RaftPlayerAnimateLogic(RaftPlayer player, CharacterModel model)
        {
            _player = player;
            _model = model;
        }

        public void Tick()
        {
            bool isMoving = _player.InputLogic.MoveDirection != Vector3.zero;
            bool isHoldingItem = _player.Hotbar.SelectedSlot.InventoryItem != null;
            
            _model.SetBool(IsMovingBoolName, isMoving);
            _model.SetBool(IsHoldingItemBoolName, isHoldingItem);
        }

        public async Task Attack()
        {
            _model.SetBool(IsAttackingBoolName, true);
            
            while (!_model.GetCurrentAnimatorStateInfo((int)Layer.Base).IsName(AttackStateName))
            {
                await Task.Yield();
            }
            
            while (!_model.IsInTransition((int)Layer.Base))
            {
                await Task.Yield();
            }

            _model.SetBool(IsAttackingBoolName, false);
        }
    }
}