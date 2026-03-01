using ShinyOwl.Common;
using UnityEngine;

namespace FishFlingers.Entities
{
    public class InputLogic
    {
        private RaftPlayer _player;

        // Vars always active
        private bool _leftClick;
        private bool _rightClick;
        private bool _rotateItem;
        private bool _openSettings;
        private bool _openFishingBag;

        public bool LeftClick => _leftClick;
        public bool RightClick => _rightClick;
        public bool RotateItem => _rotateItem;
        public bool OpenSettings => _openSettings;
        public bool OpenFishingBag => _openFishingBag;

        // Vars dependent on RaftPlayer.CanAct
        private Vector2 _mouse;
        private Vector3 _moveDirection;
        private bool _jump;
        private bool _ascend;
        private bool _interact;

        public Vector2 Mouse => _mouse;
        public Vector3 MoveDirection => _moveDirection;
        public bool Jump => _jump;
        public bool Ascend => _ascend;
        public bool Interact => _interact;

        public InputLogic(RaftPlayer player)
        {
            _player = player;
        }

        public void Tick()
        {
            _leftClick = Input.GetMouseButtonDown(0);
            _rightClick = Input.GetMouseButtonDown(1);
            _rotateItem = Input.GetKeyDown(KeyCode.R);
            _openSettings = Input.GetKeyDown(KeyCode.Escape);
            _openFishingBag = Input.GetKeyDown(KeyCode.E);

            if (_player.CanAct)
            {
                _mouse = Input.mousePosition;
                _moveDirection = Vector3.ClampMagnitude(new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")), 1f);
                _jump = Input.GetKeyDown(KeyCode.Space);
                _ascend = Input.GetKey(KeyCode.Space);
                _interact = Input.GetKeyDown(KeyCode.F);
            }
            else
            {
                _moveDirection = Vector3.zero;
                _jump = false;
                _ascend = false;
                _interact = false;
            }
        }
    }
}