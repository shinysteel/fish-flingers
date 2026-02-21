using FishFlingers.Inventories;
using FishFlingers.Pools;
using FishFlingers.States;
using ShinyOwl.Common;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class HotbarWidget : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;

        private PoolManager _poolManager;

        private Hotbar _hotbar;

        private HotbarWidgetSlot[] _slots;

        private void Awake()
        {
            _poolManager = GameManager.Instance.Get<PoolManager>();
        }

        public void Setup(GameplayContext context)
        {
            _hotbar = context.LocalPlayer.Hotbar;

            _slots = new HotbarWidgetSlot[_hotbar.Slots.Count];

            for (int i = 0; i < _hotbar.Slots.Count; i++)
            {
                _slots[i] = _poolManager.Get<HotbarWidgetSlot>(new SpawnParams() { Parent = transform });
                _slots[i].Setup(i);
            }

            OnRectTransformDimensionsChange();

            for (int i = 0; i < _hotbar.Slots.Count; i++)
            {
                HandleSlotChanged(i, _hotbar.Slots[i]);
            }

            _hotbar.OnSlotChanged += HandleSlotChanged;
        }

        private void OnDestroy()
        {
            if (_hotbar != null)
            {
                _hotbar.OnSlotChanged -= HandleSlotChanged;
            }
        }

        private void HandleSlotChanged(int index, InventoryItem item)
        {
            _slots[index].SetInventoryItem(item);
        }

        private void OnRectTransformDimensionsChange()
        {
            if (_hotbar == null)
            {
                return;
            }

            UpdateSlots();
        }

        private void UpdateSlots()
        {
            Vector2 size = new Vector2(_rectTransform.rect.width / _slots.Length, _rectTransform.rect.height);
            float pivot = (_slots.Length - 1) / 2f;

            for (int i = 0; i < _slots.Length; i++)
            {
                Vector2 position = new Vector2((i - pivot) * size.x, 0f);
                _slots[i].View.SetTransform(position, size);
            }
        }
    }
}