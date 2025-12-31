using FishFlingers.Pools;
using PurrNet.Prediction;
using ShinyOwl.Common;
using System;
using UnityEngine;

namespace FishFlingers.Environments
{
    public class Tile : PredictedIdentity<Tile.State>
    {
        [Serializable]
        private class BobSettings
        {
            // The class is private, so we don't need to over-engineer with read only getters
            [SerializeField] private float _amplitude = 0.125f;
            [SerializeField] private float _noiseScale = 0.5f;
            [SerializeField] private float _timeScale = 0.25f;

            public float Amplitude => _amplitude;
            public float NoiseScale => _noiseScale;
            public float TimeScale => _timeScale;
        }

        [SerializeField] private BobSettings _bobSettings;

        public struct State : IPredictedData<State>
        {
            public Vector2Int Cell;
            public int Health;

            public void Dispose() { }
        }

        private const float YCoord = 0f;
        public const int DefaultHealth = 3;
        
        public void SetCell(Vector2Int cell)
        {
            currentState.Cell = cell;
        }

        public void SetHealth(int health)
        {
            currentState.Health = health;
        }
        
        protected override void Simulate(ref State state, float delta)
        {
            BobSimulate(ref state);
        }

        [SimulationOnly]
        private void BobSimulate(ref State state)
        {
            float y = YCoord + _bobSettings.Amplitude * Mathf.PerlinNoise(
                state.Cell.x * _bobSettings.NoiseScale + predictionManager.time.time * _bobSettings.TimeScale,
                state.Cell.y * _bobSettings.NoiseScale + predictionManager.time.time * _bobSettings.TimeScale);

            transform.position = new Vector3(state.Cell.x, y, state.Cell.y);
        }
    }
}