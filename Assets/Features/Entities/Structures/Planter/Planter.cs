using FishFlingers.UI;
using UnityEngine;
using FishFlingers.Items;

namespace FishFlingers.Entities
{
    public class Planter : Structure<PlanterDefinitionData>, IInteractable
    {
        InteractableSettings IInteractable.Settings => DefinitionData.InteractableSettings;

        bool IInteractable.CanPrompt()
        {
            return _context.LocalPlayer.Hotbar.SelectedSlot.InventoryItem?.ItemInstance.Data.ItemId == ItemId.PalmSeed;
        }

        WorldUI IInteractable.CreatePromptUI()
        {
            return _uiManager.CreateWorldUI(_uiManager.Config.RequirementPromptUIPrefab, Vector3.zero);
        }

        void IInteractable.Interact()
        { }
    }
}