using FishFlingers.Inventories;
using FishFlingers.Pools;
using ShinyOwl.Common;
using ShinyOwl.Common.Structures;
using System.Linq;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class InventoryItemView : MonoBehaviour, IPoolable 
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _itemImage;
        [SerializeField] private TMP_Text _countText;

        private InventoryWidget _inventoryWidget;
        private InventoryItem _inventoryItem;

        private BoolGrid _shape;

        public void Setup(InventoryWidget inventoryWidget, InventoryItem inventoryItem)
        {
            _inventoryWidget = inventoryWidget;
            _inventoryItem = inventoryItem;

            _shape = inventoryItem.Rotations == 0 ? inventoryItem.ItemInstance.Data.Shape : inventoryItem.ItemInstance.Data.Shape.GetRotated(inventoryItem.Rotations);

            SetupCountText();
            SetupItemImage();
        }

        private void SetupCountText()
        {
            // Count
            _countText.text = _inventoryItem.ItemInstance.Count.ToString();

            Vector2Int cell = _shape
                .Where(kvp => kvp.Value == true)
                .OrderBy(kvp => kvp.Key.y)
                .ThenByDescending(kvp => kvp.Key.x)
                .First()
                .Key;

            _countText.rectTransform.anchoredPosition = cell * _rectTransform.sizeDelta;
        }

        private void SetupItemImage()
        {
            // Size
            _rectTransform.sizeDelta = Vector2.one * _inventoryWidget.SlotSize;

            _itemImage.rectTransform.anchorMax = new Vector2(
                _inventoryItem.Rotations % 2 == 0 ? _shape.Columns : _shape.Rows,
                _inventoryItem.Rotations % 2 == 0 ? _shape.Rows : _shape.Columns);

            _itemImage.rectTransform.offsetMin = Vector2.zero;
            _itemImage.rectTransform.offsetMax = Vector2.zero;

            // Position
            _itemImage.rectTransform.pivot = new Vector2(1f / (_inventoryItem.ItemInstance.Data.Shape.Columns * 2f), 1f / (_inventoryItem.ItemInstance.Data.Shape.Rows * 2f));
            InventorySlotView pivotInventorySlotView = _inventoryWidget.InventorySlotViews[_inventoryItem.Pivot];
            _rectTransform.anchoredPosition = pivotInventorySlotView.RectTransform.anchoredPosition;

            // Rotation, negative Z is clockwise
            _itemImage.rectTransform.localEulerAngles = new Vector3(0f, 0f, _inventoryItem.Rotations * -90f);

            // Sprite
            _itemImage.sprite = _inventoryItem.ItemInstance.Data.Sprite;
        }

        public void OnReturnedToPool() { }
        public void OnTakenFromPool() { }
    }
}