using FishFlingers.Pools;
using UnityEngine;

namespace FishFlingers.Effects
{
    public class VFX : MonoBehaviour, IPoolable
    {
        [SerializeField] private VfxId _id;
        [SerializeField] private ParticleSystem _particleSystem;

        private EffectManager _effectManager;

        private float _timer;

        public VfxId Id => _id;

        private void Awake()
        {
            _effectManager = GameManager.Instance.Get<EffectManager>();
        }

        public void OnTakenFromPool()
        {
            _particleSystem.Play();

            _timer = 0f;
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer < _particleSystem.main.duration)
            {
                return;
            }

            _effectManager.ReturnVfx(this);
        }

        public void OnReturnedToPool()
        {
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}