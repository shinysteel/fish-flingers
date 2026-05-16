using FishFlingers.Items;
using UnityEngine;

namespace FishFlingers.Entities
{
    [CreateAssetMenu(fileName = "SaplingPlanterDefinitionData", menuName = "Data/Entities/Structures/SaplingPlanterDefinitionData")]
    public class PlanterDefinitionData : StructureDefinitionData
    {
        [SerializeField] private IInteractableSettings _iInteractableSettings;
        [SerializeField] private Recipe _plantRecipe;

        public IInteractableSettings IInteractableSettings => _iInteractableSettings;
        public Recipe PlantRecipe => _plantRecipe;
    }
}