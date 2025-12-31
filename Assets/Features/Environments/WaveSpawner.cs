using FishFlingers.Entities;
using FishFlingers.Networking.Predictions;
using PurrNet;
using PurrNet.Prediction;
using UnityEngine;

using PredictionManager = FishFlingers.Networking.Predictions.PredictionManager;

namespace FishFlingers.Environments
{
    public class WaveSpawner : PredictedIdentity<WaveSpawner.State>
    {
        [SerializeField] private float _waveInterval = 2.5f;

        [SerializeField] private FlyingFish _flyingFishPrefab;

        private PredictionManager _predictionManager;

        public struct State : IPredictedData<State>
        {
            public float WaveTimer;

            public void Dispose() { }
        }

        protected override void LateAwake()
        {
            _predictionManager = GameManager.Instance.Get<PredictionManager>();
        }

        protected override void Simulate(ref State state, float delta)
        {
            state.WaveTimer += delta;

            if (state.WaveTimer < _waveInterval)
            {
                return;
            }

            state.WaveTimer -= _waveInterval;

            _predictionManager.Spawn(_flyingFishPrefab.gameObject, PlayerID.Server);
        }
    }
}