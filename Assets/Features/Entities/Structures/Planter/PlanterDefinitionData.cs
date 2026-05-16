using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "SaplingPlanterDefinitionData", menuName = "Data/Entities/Structures/SaplingPlanterDefinitionData")]
    public class PlanterDefinitionData : StructureDefinitionData
    {
        [SerializeField] private IInteractableSettings _iInteractableSettings;

        public IInteractableSettings IInteractableSettings => _iInteractableSettings;
    }
}