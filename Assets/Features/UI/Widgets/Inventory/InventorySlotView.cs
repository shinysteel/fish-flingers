using FishFlingers.Inventories;
using FishFlingers.Pools;
using ShinyOwl.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    // Ambigious with FishFlingers.Inventories.InventorySlot, so we use the View suffix
    public class InventorySlotView : MonoBehaviour, IPoolable
    {
        [SerializeField] private SlotView _view;

        public SlotView View => _view;

        private InventoryWidget _inventoryWidget;
        private Vector2Int _cell;

        public InventoryWidget InventoryWidget => _inventoryWidget;
        public Vector2Int Cell => _cell;

        private InventoryItem _inventoryItem;

        public InventoryItem InventoryItem => _inventoryItem;

        public void Setup(InventoryWidget inventoryWidget, Vector2Int cell)
        {
            _inventoryWidget = inventoryWidget;
            _cell = cell;

            _view.SetColor(SlotView.EColor.Default);
        }

        public void SetInventoryItem(InventoryItem item)
        {
            _inventoryItem = item;

            _view.SetColor(_inventoryItem != null ? SlotView.EColor.Tinted : SlotView.EColor.Default);
        }
        
        public void OnReturnedToPool() 
        {
            _inventoryItem = null;
        }

        public void OnTakenFromPool() { }
    }
}