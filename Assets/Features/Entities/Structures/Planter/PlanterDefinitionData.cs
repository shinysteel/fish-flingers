using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "SaplingPlanterDefinitionData", menuName = "Data/Entities/Structures/SaplingPlanterDefinitionData")]
    public class PlanterDefinitionData : StructureDefinitionData
    {
        [SerializeField] private InteractableSettings _interactableSettings;

        public InteractableSettings InteractableSettings => _interactableSettings;
    }
}