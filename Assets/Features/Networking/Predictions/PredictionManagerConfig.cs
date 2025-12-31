using FishFlingers.Networking.Predictions;
using UnityEngine;

namespace FishFlingers.Networking.Predictions
{
    [CreateAssetMenu(fileName = "PredictionManagerConfig", menuName = "Configs/Managers/Networking/PredictionManagerConfig")]
    public class PredictionManagerConfig : ScriptableObject
    {
        [SerializeField] private PurrdictionPredictionManager _purrdictionPredictionManagerPrefab;
        [SerializeField] private PredictedPlayerSpawner _playerSpawnerPrefab;

        public PurrdictionPredictionManager PurrdictionPredictionManagerPrefab => _purrdictionPredictionManagerPrefab;
        public PredictedPlayerSpawner PlayerSpawnerPrefab => _playerSpawnerPrefab;
    }
}