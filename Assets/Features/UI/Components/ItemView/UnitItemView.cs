using FishFlingers.Inventories;
using FishFlingers.Pools;
using UnityEngine;

namespace FishFlingers.UI
{
    public class UnitItemView : MonoBehaviour, IPoolable
    {
        // It's not safe to expose _view for this script's use case, so we will be routing methods through here
        [SerializeField] private ItemView _view;

        public void Setup(InventoryItem item)
        {
            // Don't invoke UpdateView through ItemView.Setup, since we don't want to inherit many parts of ItemView
            _view.SetInventoryItem(item);

            UpdateView();
        }

        public void SetSlotSize(Vector2 size)
        {
            _view.SetSlotSize(size);
        }

        public void UpdateView()
        {
            UpdateRect();
            _view.UpdateImage();
            _view.UpdateCountText();
        }

        private void UpdateRect()
        {
            _view.RectTransform.sizeDelta = _view.SlotSize;
        }

        public void OnTakenFromPool()
        { }

        public void OnReturnedToPool()
        { }
    }
}