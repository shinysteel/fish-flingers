using ShinyOwl.Common;
using UnityEngine;

namespace FishFlingers.Entities
{
    public class InputLogic
    {
        private RaftPlayer _player;

        // Vars always active
        private Vector2 _mouse;
        private bool _leftClick;
        private bool _rightClick;
        private bool _rotateItem;
        private bool _dropItem;
        private bool _toggleSettings;
        private bool _toggleFishingBag;

        public Vector2 Mouse => _mouse;
        public bool LeftClick => _leftClick;
        public bool RightClick => _rightClick;
        public bool RotateItem => _rotateItem;
        public bool DropItem => _dropItem;
        public bool ToggleSettings => _toggleSettings;
        public bool ToggleFishingBag => _toggleFishingBag;

        // Vars dependent on RaftPlayer.CanAct
        private Vector2 _gameplayMouse;
        private Vector3 _moveDirection;
        private bool _jump;
        private bool _ascend;
        private bool _interact;

        public Vector2 GameplayMouse => _gameplayMouse;
        public Vector3 MoveDirection => _moveDirection;
        public bool Jump => _jump;
        public bool Ascend => _ascend;
        public bool Interact => _interact;

        private const string HorizontalAxis = "Horizontal";
        private const string VerticalAxis = "Vertical";

        public InputLogic(RaftPlayer player)
        {
            _player = player;
        }

        public void Tick()
        {
            _mouse = Input.mousePosition;
            _leftClick = Input.GetMouseButtonDown(0);
            _rightClick = Input.GetMouseButtonDown(1);
            _rotateItem = Input.GetKeyDown(KeyCode.R);
            _dropItem = Input.GetKeyDown(KeyCode.Q);
            _toggleSettings = Input.GetKeyDown(KeyCode.Escape);
            _toggleFishingBag = Input.GetKeyDown(KeyCode.E);

            if (_player.CanAct)
            {
                _gameplayMouse = _mouse;
                _moveDirection = Vector3.ClampMagnitude(new Vector3(Input.GetAxisRaw(HorizontalAxis), 0f, Input.GetAxisRaw(VerticalAxis)), 1f);
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