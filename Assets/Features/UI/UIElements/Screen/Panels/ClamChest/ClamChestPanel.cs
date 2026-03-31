using FishFlingers.States;
using UnityEngine;

namespace FishFlingers.UI
{
    public class ClamChestPanel : Panel
    {
        [SerializeField] private InventoryWidget _playerInventoryWidget;
        [SerializeField] private InventoryWidget _chestInventoryWidget;

        public void Setup(GameplayContext context)
        {
            _playerInventoryWidget.Setup(context, context.LocalPlayer.Inventory);
            _chestInventoryWidget.Setup(context, context.LocalPlayer.Inventory);
        }
    }
}