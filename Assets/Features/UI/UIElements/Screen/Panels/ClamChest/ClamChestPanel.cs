using FishFlingers.Inventories;
using FishFlingers.States;
using ShinyOwl.Common;
using UnityEngine;

namespace FishFlingers.UI
{
    public class ClamChestPanel : Panel
    {
        [SerializeField] private InventoryWidget _playerInventoryWidget;
        [SerializeField] private InventoryWidget _chestInventoryWidget;

        public void Setup(GameplayContext context, Inventory chestInventory)
        {
            _playerInventoryWidget.Setup(context, context.LocalPlayer.Inventory);
            _chestInventoryWidget.Setup(context, chestInventory);
        }
    }
}