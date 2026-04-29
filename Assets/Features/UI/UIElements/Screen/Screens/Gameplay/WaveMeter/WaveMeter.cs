using FishFlingers.Environments;
using FishFlingers.States;
using PrimeTween;
using ShinyOwl.Common;
using TMPro;
using UnityEngine;

namespace FishFlingers.UI
{
    public class WaveMeter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _remainingWavesText;

        [SerializeField] private RectTransform _conveyerRectTransform;
        [SerializeField] private AnimationCurve _conveyerEase;
        [SerializeField] private float _conveyerDuration = 0.5f;

        [SerializeField] private WaveMeterCell[] _cells;

        private WaveSpawner _waveSpawner;

        private Tween _conveyerTween;

        private const float CellWidth = 64f;

        public void Setup(GameplayContext context)
        {
            _waveSpawner = context.WaveSpawner;

            HandleWaveIndexChanged(_waveSpawner.WaveIndex);

            _waveSpawner.OnWaveIndexChanged += HandleWaveIndexChanged;
        }

        private void OnDestroy()
        {
            if (_waveSpawner != null)
            {
                _waveSpawner.OnWaveIndexChanged -= HandleWaveIndexChanged;
            }
        }

        private void HandleWaveIndexChanged(int index)
        {
            _conveyerTween.Complete();

            void setupCell(int cellIndex, int offset)
            {
                int index = _waveSpawner.WaveIndex + cellIndex + offset;
                _cells[cellIndex].Setup(index >= 1 && index < _waveSpawner.StageData.Waves.Length + 1);
            }

            for (int i = 0; i < _cells.Length; i++)
            {
                setupCell(i, -1);
            }

            _remainingWavesText.text = Mathf.Max(_waveSpawner.StageData.Waves.Length - _waveSpawner.WaveIndex - 2, 0).ToString();

            _conveyerTween = Tween.UIAnchoredPosition(_conveyerRectTransform, endValue: Vector2.left * CellWidth, duration: _conveyerDuration, ease: _conveyerEase).OnComplete(() =>
            {
                _conveyerRectTransform.anchoredPosition = Vector2.zero;

                for (int i = 0; i < _cells.Length; i++)
                {
                    setupCell(i, 0);
                }
            });
        }
    }
}