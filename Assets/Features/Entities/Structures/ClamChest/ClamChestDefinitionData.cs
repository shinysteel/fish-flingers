using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "ClamChestDefinitionData", menuName = "Data/Entities/Structures/ClamChestDefinitionData")]
    public class ClamChestDefinitionData : StructureDefinitionData
    {
        [SerializeField] private InteractableSettings _interactableSettings;

        public InteractableSettings InteractableSettings => _interactableSettings;
    }
}