using FishFlingers.Networking.Predictions;
using UnityEngine;

namespace FishFlingers.Networking.Predictions
{
    [CreateAssetMenu(fileName = "PredictionManagerConfig", menuName = "Configs/Managers/Networking/PredictionManagerConfig")]
    public class PredictionManagerConfig : ScriptableObject
    {
        [SerializeField] private PurrNet.Prediction.PredictionManager _purrdictionPredictionManagerPrefab;
        [SerializeField] private PredictedPlayerSpawner _playerSpawnerPrefab;

        public PurrNet.Prediction.PredictionManager PurrdictionPredictionManagerPrefab => _purrdictionPredictionManagerPrefab;
        public PredictedPlayerSpawner PlayerSpawnerPrefab => _playerSpawnerPrefab;
    }
}