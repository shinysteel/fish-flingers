using FishFlingers.Cameras;
using PurrNet;
using ShinyOwl.Common;
using System;
using System.Globalization;
using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "RaftPlayerPhysicsSettings", menuName = "Settings/Entities/RaftPlayerPhysicsSettings")]
    public class RaftPlayerPhysicsSettings : ScriptableObject
    {
        [SerializeField] private RaftPlayerMoveSettings _move;
        [SerializeField] private RaftPlayerLookSettings _look;
        [SerializeField] private RaftPlayerJumpSettings _jump;
        [SerializeField] private RaftPlayerGroundDetectionSettings _groundDetection;
        [SerializeField] private RaftPlayerSwimSettings _swim;

        public RaftPlayerMoveSettings Move => _move;
        public RaftPlayerLookSettings Look => _look;
        public RaftPlayerJumpSettings Jump => _jump;
        public RaftPlayerGroundDetectionSettings GroundDetection => _groundDetection;
        public RaftPlayerSwimSettings Swim => _swim;
    }

    [Serializable]
    public class RaftPlayerMoveSettings
    {
        [SerializeField] private float _speed = 2f;
        [SerializeField] private float _acceleration = 10f;
        [SerializeField] private float _deceleration = 7.5f;
        [SerializeField] private float _attackWindupMultiplier = 0.25f;
        [SerializeField] private float _attackImpactMultiplier = 0.1f;

        public float Speed => _speed;
        public float Acceleration => _acceleration;
        public float Deceleration => _deceleration;
        public float AttackWindupMultiplier => _attackWindupMultiplier;
        public float AttackImpactMultiplier => _attackImpactMultiplier;
    }

    [Serializable]
    public class RaftPlayerLookSettings
    {
        [SerializeField] private float _speed = 7.5f;
        [SerializeField] private float _attackImpactMultiplier = 0.25f;

        public float Speed => _speed;
        public float AttackImpactMultiplier => _attackImpactMultiplier;
    }

    [Serializable]
    public class RaftPlayerJumpSettings
    {
        [SerializeField] private float _strength = 4f;
        [SerializeField] private float _cooldown = 0.1f;

        public float Strength => _strength;
        public float Cooldown => _cooldown;
    }

    [Serializable]
    public class RaftPlayerGroundDetectionSettings
    {
        [SerializeField] private LayerMask _mask;
        [SerializeField] private float _castRadius = 0.125f;
        [SerializeField] private float _castDist = 0.05f;

        public LayerMask Mask => _mask;
        public float CastRadius => _castRadius;
        public float CastDist => _castDist;
    }

    [Serializable]
    public class RaftPlayerSwimSettings
    {
        [SerializeField] private LayerMask _mask;
        [SerializeField] private float _ascendStrength = 30f;
        [SerializeField] private float _ascendDepthThreshold = 0.25f;

        public LayerMask Mask => _mask;
        public float AscendStrength => _ascendStrength;
        public float AscendDepthThreshold => _ascendDepthThreshold;
    }

    public class RaftPlayerPhysicsLogic
    {
        private CameraManager _cameraManager;

        private RaftPlayer _player;
        private CapsuleCollider _capsuleCollider;

        private RaftPlayerPhysicsSettings _settings;

        private float _jumpTimer;
        private bool _jumpRequest;
        private bool _isGrounded;

        private RaycastHit[] _groundedHitsNonAlloc = new RaycastHit[2];
        private Collider[] _swimCollidersNonAlloc = new Collider[1];

        public RaftPlayerPhysicsLogic(RaftPlayer player, CapsuleCollider capsuleCollider)
        {
            _cameraManager = GameManager.Instance.Get<CameraManager>();

            _player = player;
            _capsuleCollider = capsuleCollider;

            _settings = _player.Data.PhysicsSettings;
        }

        public void Tick()
        {
            JumpTick();
        }

        public void FixedTick()
        {
            MoveFixedTick();
            LookFixedTick();
            GroundDetectionFixedTick();
            JumpFixedTick();
            SwimFixedTick();
        }

        private void JumpTick()
        {
            _jumpTimer += Time.deltaTime;

            if (!_player.InputLogic.Jump)
            {
                return;
            }

            if (_jumpTimer < _settings.Jump.Cooldown)
            {
                return;
            }

            // Jump on the next physics step
            _jumpRequest = true;
        }

        private void MoveFixedTick()
        {
            if (_player.AttackLogic.AttackState == RaftPlayerAttackState.Impact)
            {
                return;
            }

            Vector3 targetVelocity = _player.InputLogic.MoveDirection * _settings.Move.Speed;

            if (_player.AttackLogic.AttackState == RaftPlayerAttackState.Windup)
            {
                targetVelocity *= _settings.Move.AttackWindupMultiplier;
            }
            else if (_player.AttackLogic.AttackState == RaftPlayerAttackState.Impact)
            {
                targetVelocity *= _settings.Move.AttackImpactMultiplier;
            }

            targetVelocity.y = _player.Rigidbody.linearVelocity.y;

            float speed = _player.InputLogic.MoveDirection != Vector3.zero ? _settings.Move.Acceleration : _settings.Move.Deceleration;

            _player.Rigidbody.linearVelocity = Vector3.MoveTowards(_player.Rigidbody.linearVelocity, targetVelocity, speed * Time.fixedDeltaTime);
        }

        private void LookFixedTick()
        {
            Vector3 direction = _player.InputLogic.MoveDirection;

            if (direction == Vector3.zero)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            float speed = _settings.Look.Speed;

            if (_player.AttackLogic.AttackState == RaftPlayerAttackState.Impact)
            {
                speed *= _settings.Look.AttackImpactMultiplier;
            }

            _player.Rigidbody.MoveRotation(Quaternion.Slerp(_player.Rigidbody.rotation, targetRotation, speed * Time.fixedDeltaTime));
        }

        private void GroundDetectionFixedTick()
        {
            Vector3 origin = _player.Rigidbody.position + Vector3.up * _settings.GroundDetection.CastRadius;

            int hits = Physics.SphereCastNonAlloc(origin, _settings.GroundDetection.CastRadius, Vector3.down, _groundedHitsNonAlloc, _settings.GroundDetection.CastDist, _settings.GroundDetection.Mask);

            bool grounded = false;

            for (int i = 0; i < hits; i++)
            {
                // Since we include the player layer to jump on other player's heads, we need to ignore our own collider here
                if (_groundedHitsNonAlloc[i].collider != _capsuleCollider)
                {
                    grounded = true;
                    break;
                }
            }

            _isGrounded = grounded;
        }

        private void JumpFixedTick()
        {
            if (!_jumpRequest)
            {
                return;
            }

            // Consume the request
            _jumpTimer = 0f;
            _jumpRequest = false;

            if (!_isGrounded)
            {
                return;
            }

            // Cancel out gravity
            _player.Rigidbody.linearVelocity = new Vector3(_player.Rigidbody.linearVelocity.x, 0f, _player.Rigidbody.linearVelocity.z);
            _player.Rigidbody.AddForce(Vector3.up * _settings.Jump.Strength, ForceMode.Impulse);
        }

        private void SwimFixedTick()
        {
            // While swimming, the player can hold spacebar to propel themselves up
            if (!_player.InputLogic.Ascend)
            {
                return;
            }

            // If we are overlapping a collider on the swim mask, we are swimming
            if (Physics.OverlapCapsuleNonAlloc(_capsuleCollider.bounds.min, _capsuleCollider.bounds.max, _capsuleCollider.radius, _swimCollidersNonAlloc, _settings.Swim.Mask) == 0)
            {
                return;
            }

            Collider waterCollider = _swimCollidersNonAlloc[0];

            Physics.ComputePenetration(_capsuleCollider, _player.Rigidbody.position, _player.Rigidbody.rotation, waterCollider, waterCollider.transform.position, waterCollider.transform.rotation, out _, out float depth);

            float ascendFactor = Mathf.Clamp01(depth / _settings.Swim.AscendDepthThreshold);
            Vector3 ascendForce = Vector3.up * _settings.Swim.AscendStrength * ascendFactor;

            _player.Rigidbody.AddForce(ascendForce, ForceMode.Force);
        }
    }
}