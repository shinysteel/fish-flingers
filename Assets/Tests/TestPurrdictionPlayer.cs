using FishFlingers.Networking.Predictions;
using PurrNet.Packing;
using PurrNet.Prediction;
using UnityEngine;

namespace FishFlingers.Tests
{
    public class TestPurrdictionPlayer : PredictedIdentity<TestPurrdictionPlayer.Input, TestPurrdictionPlayer.State>
    {
        [SerializeField] private PredictedRigidbody _rigidbody;

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

        protected override void GetFinalInput(ref Input input)
        {
            input.Horizontal = (int)UnityEngine.Input.GetAxisRaw("Horizontal");
            input.Vertical = (int)UnityEngine.Input.GetAxisRaw("Vertical");
        }

        protected override void UpdateInput(ref Input input)
        {
            input.Jump |= UnityEngine.Input.GetKeyDown(KeyCode.Space);
        }

        protected override void SanitizeInput(ref Input input)
        {
            Vector2 direction = Vector2.ClampMagnitude(new Vector2(input.Horizontal, input.Vertical), 1f);
            input.Horizontal = direction.x;
            input.Vertical = direction.y;
        }

        protected override void Simulate(Input input, ref State state, float delta)
        {
            _rigidbody.AddForce(new Vector3(input.Horizontal, 0f, input.Vertical) * 5f);

            if (input.Jump)
            {
                _rigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            }
        }
    }
}