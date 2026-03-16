using UnityEngine;

namespace FishFlingers.Saving
{
    [CreateAssetMenu(fileName = "SaveManagerConfig", menuName = "Configs/Managers/SaveManagerConfig")]
    public class SaveManagerConfig : ScriptableObject
    {
        [SerializeField] private string _userDataFileName;
        [SerializeField] private string _gameDataFileName;

        public string UserDataFileName => _userDataFileName;
        public string GameDataFileName => _gameDataFileName;
    }
}