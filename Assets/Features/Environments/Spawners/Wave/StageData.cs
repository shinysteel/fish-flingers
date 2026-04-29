using UnityEngine;

namespace FishFlingers.Environments
{
    [CreateAssetMenu(fileName = "StageData", menuName = "Data/Environments/StageData")]
    public class StageData : ScriptableObject
    {
        [SerializeField] private Wave[] _waves;

        public Wave[] Waves => _waves;
    }
}