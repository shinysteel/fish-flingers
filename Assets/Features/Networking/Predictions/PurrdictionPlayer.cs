using FishFlingers.Cameras;
using ShinyOwl.Common;
using UnityEngine;
using FishFlingers.Networking;
using PurrNet;
using PurrNet.Prediction;
using FishFlingers.Scenes;
using System.Threading.Tasks;
using System;
using PurrNet.Packing;

namespace FishFlingers.Networking.Predictions
{
    public class PurrdictionPlayer : PredictedIdentity<PurrdictionPlayer.Input, PurrdictionPlayer.State>
    {
        [SerializeField] private Transform _visuals;

        [SerializeField] private PredictedRigidbody _rigidbody;

        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _moveAcceleration = 10f;
        [SerializeField] private float _moveDeceleration = 7.5f;

        [SerializeField] private float _jumpForce = 3f;
        
        private CameraManager _cameraManager;

        public struct Input : IPredictedData<Input>
        {
            public NormalizedFloat Horizontal;
            public NormalizedFloat Vertical;
            public bool Jump;

            public void Dispose() { }
        }

        public struct State : IPredictedData<State>
        {
            public void Dispose() { }
        }

        protected override void LateAwake()
        {
            _cameraManager = GameManager.Instance.Get<CameraManager>();

            if (isOwner)
            {
                _cameraManager.SetMode(new FollowCameraMode(_visuals, new Vector3(0f, 3f, -5f)));
            }
        }

        // Runs every tick, and afterwards resets the values
        protected override void GetFinalInput(ref Input input)
        {
            input.Horizontal = (int)UnityEngine.Input.GetAxisRaw("Horizontal");
            input.Vertical = (int)UnityEngine.Input.GetAxisRaw("Vertical");
        }

        // Runs every frame
        protected override void UpdateInput(ref Input input)
        {
            // Checks like GetKeyDown are frame sensitive, so they they to be done here
            // We use |= condition here, since we want to preserve the input once first captured
            input.Jump |= UnityEngine.Input.GetKeyDown(KeyCode.Space);
        }

        protected override void SanitizeInput(ref Input input)
        {
            // We never trust clients, and need to validate any input we receive
            Vector2 direction = Vector2.ClampMagnitude(new Vector2(input.Horizontal, input.Vertical), 1f);
            input.Horizontal = direction.x;
            input.Vertical = direction.y;
        }

        protected override void ModifyExtrapolatedInput(ref Input input)
        {
            // Some input doesn't make sense to extrapolate, such as jumping. This essentially
            // means if they jump and than we don't hear from them for a while, don't assume they
            // will keep jumping
            input.Jump = false;
        }

        protected override void Simulate(Input input, ref State state, float delta)
        {
            // Move
            Vector3 moveDirection = new Vector3(input.Horizontal, 0f, input.Vertical);
            Vector3 targetVelocity = moveDirection * _moveSpeed;
            targetVelocity.y = _rigidbody.linearVelocity.y;
            float speed = moveDirection != Vector3.zero ? _moveAcceleration : _moveDeceleration;
            _rigidbody.linearVelocity = Vector3.MoveTowards(_rigidbody.linearVelocity, targetVelocity, speed * Time.fixedDeltaTime);

            // Jump
            if (input.Jump)
            {
                _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            }
        }
    }
}