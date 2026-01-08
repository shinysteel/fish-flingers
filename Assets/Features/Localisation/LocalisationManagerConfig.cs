using UnityEngine;

namespace FishFlingers.Localisation
{
    [CreateAssetMenu(fileName = "LocalisationManagerConfig", menuName = "Configs/Managers/LocalisationManagerConfig")]
    public class LocalisationManagerConfig : ScriptableObject
    {
        [SerializeField] private LocalisationTable _browseGamesPanelTable;
        [SerializeField] private LocalisationTable _commonTable;
        [SerializeField] private LocalisationTable _mainMenuScreenTable;

        public LocalisationTable[] GetTables()
        {
            return new LocalisationTable[]
            {
                _browseGamesPanelTable,
                _commonTable,
                _mainMenuScreenTable
            };
        }
    }
}