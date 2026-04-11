using UnityEngine;

namespace FishFlingers.Entities
{
    public class RaftPlayerAnimateLogic
    {
        private RaftPlayer _player;
        private CharacterModel _model;

        private const string IsMovingName = "IsMoving";

        public RaftPlayerAnimateLogic(RaftPlayer player, CharacterModel model)
        {
            _player = player;
            _model = model;
        }

        public void Tick()
        {
            bool isMoving = _player.InputLogic.MoveDirection != Vector3.zero;

            _model.Animator.SetBool(IsMovingName, isMoving);
        }
    }
}