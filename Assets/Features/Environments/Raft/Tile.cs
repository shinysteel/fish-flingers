using FishFlingers.Networking.Predictions;
using FishFlingers.Pools;
using PurrNet;
using PurrNet.Modules;
using PurrNet.Packing;
using PurrNet.Prediction;
using ShinyOwl.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FishFlingers.Environments
{
    public class Tile : PredictedIdentity<Tile.State>
    {
        [SerializeField] private BobSettings _bobSettings;
        [SerializeField] private SinkSettings _sinkSettings;
        [SerializeField] private MeshRenderer _renderer;

        [Serializable]
        private class BobSettings
        {
            [SerializeField] private float _amplitude = 0.125f;
            [SerializeField] private float _noiseScale = 0.5f;
            [SerializeField] private float _timeScale = 0.25f;

            public float Amplitude => _amplitude;
            public float NoiseScale => _noiseScale;
            public float TimeScale => _timeScale;
        }

        [Serializable]
        private class SinkSettings
        {
            [SerializeField] private float _radius = 1.333f;
            [SerializeField] private float _speed = 0.25f;

            public float Radius => _radius;
            public float Speed => _speed;
        }

        public struct State : IPredictedData<State>
        {
            public Vector2Int Cell;
            public int Health;

            public void Dispose() { }
        }

        private Material _material;

        private const float YCoord = 0f;

        private const string DamagedBlendName = "_DamagedBlend";

        public const int DefaultHealth = 3;

        private void Start()
        {
            _material = _renderer.material;
            _material.SetFloat(DamagedBlendName, 1f - ((float)currentState.Health / DefaultHealth));
        }

        public void SetCell(Vector2Int cell)
        {
            currentState.Cell = cell;
        }

        public void SetHealth(int health)
        {
            currentState.Health = health;
        }

        protected override void SimulationStart()
        {
            PositionSimulate(ref currentState, Mathf.Infinity);
        }
        
        protected override void Simulate(ref State state, float delta)
        {   
            PositionSimulate(ref state, delta);
        }
        
        [SimulationOnly]
        private void PositionSimulate(ref State state, float delta)
        {
            LayerMask mask = LayerMask.GetMask("Player");
            bool sink = Physics.CheckSphere(new Vector3(state.Cell.x, YCoord, state.Cell.y), _sinkSettings.Radius, mask);

            float targetY;

            if (sink)
            {
                // Sit just above the water
                targetY = YCoord;
            }
            else
            {
                // Bob up and down
                targetY = YCoord + _bobSettings.Amplitude * Mathf.PerlinNoise(
                    state.Cell.x * _bobSettings.NoiseScale + predictionManager.time.time * _bobSettings.TimeScale,
                    state.Cell.y * _bobSettings.NoiseScale + predictionManager.time.time * _bobSettings.TimeScale);
            }
            
            Vector3 targetPosition = new Vector3(state.Cell.x, targetY, state.Cell.y);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, delta * _sinkSettings.Speed);
        }
    }
}