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
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _image;
        [SerializeField] private CellOutline _cellOutline;

        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _tintedColor;

        public RectTransform RectTransform => _rectTransform;
        public CellOutline CellOutline => _cellOutline;

        private InventoryWidget _inventoryWidget;
        private Vector2Int _cell;

        public InventoryWidget InventoryWidget => _inventoryWidget;
        public Vector2Int Cell => _cell;

        private InventoryItem _inventoryItem;

        public InventoryItem InventoryItem => _inventoryItem;

        public enum EColor
        {
            Default ,
            Tinted  ,
        }

        public void Setup(InventoryWidget inventoryWidget, Vector2Int cell)
        {
            _inventoryWidget = inventoryWidget;
            _cell = cell;

            SetColor(EColor.Default);
        }

        public void SetTransform(Vector2 position, Vector2 size)
        {
            _rectTransform.anchoredPosition = position;
            _rectTransform.sizeDelta = size;
        }

        public void SetInventoryItem(InventoryItem item)
        {
            _inventoryItem = item;

            SetColor(_inventoryItem != null ? EColor.Tinted : EColor.Default);
        }
        
        private void SetColor(EColor colorEnum)
        {
            Color color = colorEnum switch
            { 
                EColor.Default => _defaultColor,
                EColor.Tinted => _tintedColor,
                _ => _defaultColor
            };

            _image.color = color;
        }

        public void OnReturnedToPool() 
        {
            _inventoryItem = null;
        }

        public void OnTakenFromPool() { }
    }
}