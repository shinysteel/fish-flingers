using FishFlingers.Inventories;
using FishFlingers.Pools;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class HotbarSlot : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;

        private PoolManager _poolManager;

        private UnitItemView _unitItemView;
        
        private void Awake()
        {
            _poolManager = GameManager.Instance.Get<PoolManager>();
        }

        public void SetInventoryItem(InventoryItem item)
        {
            if (item != null)
            {
                if (_unitItemView == null)
                {
                    _unitItemView = _poolManager.Get<UnitItemView>(new SpawnParams() { Parent = transform });
                    _unitItemView.SetSlotSize(_rectTransform.sizeDelta);
                }

                _unitItemView.Setup(item);
            }
            else
            {
                if (_unitItemView != null)
                {
                    _poolManager.Return(_unitItemView);
                    _unitItemView = null;
                }
            }
        }

        public void SetSelected(bool selected)
        {
        }
    }
}