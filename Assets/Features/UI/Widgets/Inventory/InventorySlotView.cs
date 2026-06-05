using FishFlingers.Inventories;
using FishFlingers.Pools;
using FishFlingers.States;
using ShinyOwl.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    // Ambigious with FishFlingers.Inventories.InventorySlot, so we use the View suffix
    public class InventorySlotView : SlotView, ITypedPoolable
    {
        private InventoryWidget _inventoryWidget;
        private Vector2Int _cell;

        public InventoryWidget InventoryWidget => _inventoryWidget;
        public Vector2Int Cell => _cell;

        public void SetWidgetAndCell(InventoryWidget widget, Vector2Int cell)
        {
            _inventoryWidget = widget;
            _cell = cell;
        }

        public void OnReturnedToPool()
        {
            OnDestroy();
        }

        public void OnTakenFromPool() { }
    }
}