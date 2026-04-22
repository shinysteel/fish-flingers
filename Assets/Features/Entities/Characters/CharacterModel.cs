using PurrNet;
using UnityEngine;

namespace FishFlingers.Entities
{
    public class CharacterModel : EntityModel
    {
        [SerializeField] private Transform _itemLocator;

        private float _blinkTimer = 0f;
        private float _minBlinkInterval = 2.5f;
        private float _maxBlinkInterval = 7.5f;

        public Transform ItemLocator => _itemLocator;

        // Animator
        private const string BlinkAnimatorTriggerName = "Blink";
        public const string IsDefeatedAnimatorBoolName = "IsDefeated";

        // Shader
        public const string DefeatBlendShaderPropertyName = "_DefeatBlend";

        private void Start()
        {
            ResetBlinkTimer();
        }

        private void Update()
        {
            BlinkUpdate();
        }

        private void BlinkUpdate()
        {
            _blinkTimer -= Time.deltaTime;

            if (_blinkTimer > 0f)
            {
                return;
            }

            _animator.SetTrigger(BlinkAnimatorTriggerName);

            ResetBlinkTimer();
        }

        private void ResetBlinkTimer()
        {
            _blinkTimer = Random.Range(_minBlinkInterval, _maxBlinkInterval);
        }
    }
}