using FishFlingers.Inventories;
using FishFlingers.Items;
using FishFlingers.Pools;
using FishFlingers.States;
using PurrLobby;
using ShinyOwl.Common;
using ShinyOwl.Common.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class HotbarWidgetSlot : SlotView, ITypedPoolable
    {
        [SerializeField] private Image _assignmentImage;
        
        private PoolManager _poolManager;
        private ItemManager _itemManager;

        private int _index = -1;
        public int Index => _index;

        private UnitItemView _unitItemView;

        private const float SlotSizeScalar = 0.9f;

        private void Awake()
        {
            _poolManager = GameManager.Instance.Get<PoolManager>();
            _itemManager = GameManager.Instance.Get<ItemManager>();
        }

        public void SetIndex(int index)
        {
            _index = index;

            _cellOutline.SetColor(CellOutline.EColor.Default);
            _cellOutline.SetEnabled(true, true, true, true);
        }

        public override void SetInventoryItem(InventoryItem item)
        {
            base.SetInventoryItem(item);
            
            RefreshAssignmentImage();

            if (item == null)
            {
                ReturnUnitItemView();
                return;
            }

            if (_unitItemView == null)
            {
                _unitItemView = _poolManager.GetTypedPoolable<UnitItemView>(new SpawnParams() { Parent = transform });
                _unitItemView.SetSlotSize(_rectTransform.rect.size * SlotSizeScalar);
            }

            _unitItemView.Setup(_context, item);
        }

        public override void SetTransform(Vector2 position, Vector2 size)
        {
            base.SetTransform(position, size);

            if (_unitItemView != null)
            {
                _unitItemView.SetSlotSize(_rectTransform.rect.size * SlotSizeScalar);
                _unitItemView.Refresh();
            }
        }

        private void RefreshAssignmentImage()
        {
            if (_inventoryItem != null && _context.LocalPlayer.Hotbar.IsItemAssigned(_inventoryItem, out int index))
            {
                _assignmentImage.sprite = _itemManager.GetAssignmentSprite(index);
                _assignmentImage.enabled = true;
            }
            else
            {
                _assignmentImage.enabled = false;
            }
        }
        
        private void ReturnUnitItemView()
        {
            if (_unitItemView != null)
            {
                _poolManager.ReturnTypedPoolable(_unitItemView);
            }

            _unitItemView = null;
        }

        public void OnReturnedToPool()
        {
            OnDestroy();

            ReturnUnitItemView();
        }

        public void OnTakenFromPool()
        { }
    }
}