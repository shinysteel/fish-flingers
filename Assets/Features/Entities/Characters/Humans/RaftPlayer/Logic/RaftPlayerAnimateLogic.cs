using UnityEngine;

namespace FishFlingers.Entities
{
    public class RaftPlayerAnimateLogic
    {
        private RaftPlayer _player;
        private CharacterModel _characterModel;

        private const string IsMovingName = "IsMoving";

        public RaftPlayerAnimateLogic(RaftPlayer player, CharacterModel characterModel)
        {
            _player = player;
            _characterModel = characterModel;
        }

        public void Tick()
        {
            bool isMoving = _player.InputLogic.MoveDirection != Vector3.zero;

            _characterModel.Animator.SetBool(IsMovingName, isMoving);
        }
    }
}