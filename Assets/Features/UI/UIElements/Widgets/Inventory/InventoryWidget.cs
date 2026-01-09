using FishFlingers.Items;
using System.Collections.Generic;
using UnityEngine;

namespace FishFlingers.UI
{
    public class InventoryWidget : MonoBehaviour
    {
        [SerializeField] private float _padding = 5f;

        [SerializeField] private InventorySlot _inventorySlotPrefab;

        private Inventory _inventory;

        public void Setup(Inventory inventory)
        {
            _inventory = inventory;

            Refresh();
        }

        private void Refresh()
        {
            foreach (KeyValuePair<Vector2Int, Items.InventorySlot> kvp in _inventory)
            {
                InventorySlot slot = Instantiate(_inventorySlotPrefab, transform);

                Vector2Int cell = kvp.Key;

                Vector3 position = new Vector3(cell.x * slot.RectTransform.sizeDelta.x + cell.x * _padding, cell.y * slot.RectTransform.sizeDelta.y + kvp.Key.y * _padding);

                slot.Setup(new Vector2Int(kvp.Key.x, kvp.Key.y), position);
            }
        }
    }
}